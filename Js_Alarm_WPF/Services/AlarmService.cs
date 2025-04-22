using System.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;
using Js_Alarm_WPF.Dto;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Serilog;

namespace Js_Alarm_WPF.Services
{
    internal class AlarmService
    {
        private readonly string _connectionStr;
        private readonly string _linePostUrl;
        private readonly Dictionary<string, DateTime> dissconnectDict = [];
        private readonly HttpClient _httpClient = new();

        public AlarmService()
        {
            _connectionStr = ConfigurationManager.AppSettings["DbConnectionStr_Dev"];
            _linePostUrl = ConfigurationManager.AppSettings["LinePostUrl_Dev"];
        }
        public List<AlarmGroupDto> GetAlarmInfo()
        {
            var sql = @"
                SELECT 
                    ag.GroupId, ag.GroupName, ag.Enable,
                    ai.GroupId,ai.Stid,ai.Location, ai.DelayTime, ai.Enable AS ItemEnable, ai.BreakAlarm,
                    aset.Stid,aset.ParameterColumn, aset.ParameterShow, aset.Threshold, aset.StartTime, aset.EndTime, aset.NextCheckTime
                FROM 
                    AlarmGroup ag
                LEFT JOIN 
                    AlarmItem ai ON ag.GroupId = ai.GroupId
                LEFT JOIN 
                    AlarmSettings aset ON ai.Stid = aset.Stid
                WHERE 
                    ag.Enable = 1";
            using var conn = new SqlConnection(_connectionStr);
            var alarmGroupDict = new Dictionary<string, AlarmGroupDto>();
            var result = conn.Query<AlarmGroupDto, AlarmItemDto, AlarmSetDto, AlarmGroupDto>(sql, (group, item, set) =>
            {
                if (!alarmGroupDict.TryGetValue(group.GroupId, out AlarmGroupDto groupDto))
                {
                    groupDto = group;
                    groupDto.AlarmItemDto = [];
                    alarmGroupDict.Add(groupDto.GroupId, groupDto);
                }
                if (item != null)
                {
                    var existingItem = groupDto.AlarmItemDto.FirstOrDefault(x => x.Stid == item.Stid);
                    if (existingItem == null)
                    {
                        groupDto.AlarmItemDto.Add(item);
                        item.AlarmSettingsDto = [];
                        existingItem = item;
                    }
                    if (set != null)
                    {
                        existingItem.AlarmSettingsDto.Add(set);
                    }
                }
                return groupDto;

            }, splitOn: "GroupId,Stid");
            return result.Distinct().ToList();
        }

        public async void SendAlarmMessage()
        {
            var alarmGroupInfo = GetAlarmInfo();
            var currentTime = DateTime.Now;
            foreach (var group in alarmGroupInfo)
            {
                if (group.Enable)
                {
                    foreach (var item in group.AlarmItemDto)
                    {
                        dissconnectDict.TryAdd(item.Stid, DateTime.MinValue);

                        var apiUrl = $"https://www.jsene.com/ConstructionSite/FuTsu/TSMC/getReal?stid={item.Stid}&list=1,2,3,4,5,6,7,8";
                        var data = await GetSensorDtoAsync(apiUrl);
                        var isEnabled = data.vals.Length != 0;
                        if (isEnabled != item.ItemEnable)
                        {
                            var updateSql = "UPDATE AlarmItem SET Enable = @Enable WHERE Stid = @Stid";
                            using var conn = new SqlConnection(_connectionStr);
                            conn.Execute(updateSql, new { Enable = isEnabled, item.Stid });
                            Log.Information($"UPDATE AlarmItem SET Enable = {isEnabled} WHERE Stid = {item.Stid}");
                        }
                        if (isEnabled)
                        {
                            //set 與 data.vals 的比對邏輯
                            foreach (var set in item.AlarmSettingsDto)
                            {
                                if (currentTime <= set.NextCheckTime)
                                {
                                    continue;
                                }
                                var val = data.vals.Where(x => x.parameter == set.ParameterColumn).FirstOrDefault();
                                if (val != null)
                                {
                                    if (val.val > set.Threshold && currentTime.TimeOfDay > set.StartTime && currentTime.TimeOfDay < set.EndTime)
                                    {
                                        // 發送閾值警報
                                        var linePost = new LinePostDto()
                                        {
                                            Url = _linePostUrl,
                                        };
                                        //判斷有無風向
                                        var ws = data.vals.Where(x => x.parameter == "WS").FirstOrDefault();
                                        var wd = data.vals.Where(x => x.parameter == "WD").FirstOrDefault();
                                        if (ws != null && wd != null && set.ParameterColumn != "WS")
                                        {
                                            string[] direction = ["北風","東北風","東風","東南風","南風","西南風","西風","西北風","北風"];
                                            var windDirection = (wd.val > 360) ? "資料錯誤" : direction[(int)Math.Round(wd.val / 45)];
                                            linePost.Payload = new Payload
                                            {
                                                Title = item.GroupId,
                                                Mes = $"【{group.GroupName}】\r\n【STID】： {item.Stid}\r\n【位置】： {item.Location}\r\n【時間】： {data.time}\r\n【屬性】： {set.ParameterShow}\r\n【風向】： {windDirection}\r\n【風速】： {ws.val} m/s\r\n【數值】： {val.val}\r\n【狀態】： 超過閾值({set.Threshold})",
                                                Image = ""
                                            };
                                        }
                                        else
                                        {
                                            linePost.Payload = new Payload
                                            {
                                                Title = item.GroupId,
                                                Mes = $"【{group.GroupName}】\r\n【STID】： {item.Stid}\r\n【位置】： {item.Location}\r\n【時間】： {data.time}\r\n【屬性】： {set.ParameterShow}\r\n【數值】： {val.val}\r\n【狀態】： 超過閾值({set.Threshold})",
                                                Image = ""
                                            };
                                        }
                                        SendLineMessage(linePost);
                                        set.NextCheckTime = currentTime.AddMinutes(item.DelayTime);
                                        using var conn = new SqlConnection(_connectionStr);
                                        conn.Execute("UPDATE AlarmSettings SET NextCheckTime = @NextCheckTime WHERE Stid = @Stid AND ParameterColumn = @ParameterColumn Threshold = @Threshold", new { set.NextCheckTime, set.Stid, set.ParameterColumn, set.Threshold });
                                        Log.Information($"UPDATE AlarmSettings SET NextCheckTime = {set.NextCheckTime} WHERE Stid = {set.Stid} AND ParameterColumn = {set.ParameterColumn} AND Threshold = {set.Threshold}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (item.BreakAlarm)
                            {
                                if (currentTime - dissconnectDict[item.Stid] > TimeSpan.FromHours(12))
                                {
                                    // 發送斷線警報
                                    var linePost = new LinePostDto()
                                    {
                                        Url = _linePostUrl,
                                        Payload = new Payload
                                        {
                                            Title = item.GroupId,
                                            Mes = $"【{group.GroupName}】\r\n【STID】： {item.Stid}\r\n【位置】： {item.Location}\r\n【時間】： {currentTime}\r\n【狀態】： 感測器斷線",
                                            Image = ""
                                        }
                                    };
                                    SendLineMessage(linePost);
                                    dissconnectDict[item.Stid] = currentTime;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 取得項目資料
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <returns>感測器分鐘值資料</returns>
        private static async Task<SensorDto> GetSensorDtoAsync(string apiUrl)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    var data = JsonConvert.DeserializeObject<SensorDto>(content);
                    return data;
                }
                catch (Exception e)
                {
                    Log.Error($"{e.ToString}，content：{content}");
                }
            }
            return new SensorDto();
        }

        public async Task SendLineMessage(LinePostDto postDto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(postDto.Payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(postDto.Url, content);
                response.EnsureSuccessStatusCode();
                var result = response.Content.ReadAsStringAsync();
                Log.Information($"Line Notify Response: {result.Result}");
            }
            catch (Exception)
            {
                Log.Error("Line Notify Error");
            }
        }
    }
}
