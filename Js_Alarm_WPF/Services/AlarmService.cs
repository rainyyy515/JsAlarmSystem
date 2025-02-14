using System.Configuration;



using Microsoft.Data.SqlClient;
using Dapper;
using Js_Alarm_WPF.Dto;
using System.Net.Http;
using Newtonsoft.Json;

namespace Js_Alarm_WPF.Services
{
    internal class AlarmService
    {
        private readonly string _connectionStr;
        private readonly Dictionary<string, DateTime> dissconnectDict = [];
        public AlarmService()
        {
            _connectionStr = ConfigurationManager.AppSettings["DbConnectionStr_Dev"];
        }
        public List<AlarmGroupDto> GetAlarmInfo()
        {
            //var groupsDto = new List<AlarmGroupDto>();
            //using var conn = new SqlConnection(_connectionStr);
            //var groups = conn.Query<AlarmGroup>("SELECT GroupId,Enable FROM AlarmGroup Where Enable = 1");
            //foreach (var group in groups)
            //{
            //    groupsDto.Add(new AlarmGroupDto
            //    {
            //        GroupId = group.GroupId,
            //        Enable = group.Enable,
            //        AlarmItemDto = conn.Query<AlarmItemDto>("SELECT Stid,DelayTime,Enable FROM AlarmItem Where GroupId = @GroupId", new { GroupId = group.GroupId }).ToList()
            //    });
            //}
            //foreach (var group in groupsDto)
            //{
            //    foreach (var item in group.AlarmItemDto)
            //    {
            //        var settings = conn.Query<AlarmSetDto>("SELECT Stid,ParameterColumn,ParameterShow,Threshold,StartTime,EndTime,NextCheckTime FROM AlarmSettings Where Stid = @Stid", new { Stid = item.Stid }).ToList();
            //        item.AlarmSettingsDto = [.. settings];
            //    }
            //}
            //return groupsDto;
            var sql = @"
                SELECT 
                    ag.GroupId, ag.Enable,
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
                    groupDto.AlarmItemDto = new List<AlarmItemDto>();
                    alarmGroupDict.Add(groupDto.GroupId, groupDto);
                }
                if (item != null)
                {
                    var existingItem = groupDto.AlarmItemDto.FirstOrDefault(x => x.Stid == item.Stid);
                    if (existingItem == null)
                    {
                        groupDto.AlarmItemDto.Add(item);
                        item.AlarmSettingsDto = new List<AlarmSetDto>();
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
                    }
                    if (isEnabled)
                    {
                        //TODO 開發 set 與 data.vals 的比對邏輯
                        foreach (var set in item.AlarmSettingsDto)
                        {
                            if (currentTime <= set.NextCheckTime)
                            {
                                continue;
                            }
                            var val = data.vals.Where(x => x.parameter == set.ParameterColumn).FirstOrDefault();
                            if (val != null)
                            {
                                if (val.val > set.Threshold)
                                {
                                    //TODO 發送警報邏輯
                                    set.NextCheckTime = currentTime.AddMinutes(item.DelayTime);
                                    using var conn = new SqlConnection(_connectionStr);
                                    conn.Execute("UPDATE AlarmSettings SET NextCheckTime = @NextCheckTime WHERE Stid = @Stid", new { set.NextCheckTime, set.Stid });
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
                                //TODO 發送斷線警報邏輯
                                dissconnectDict[item.Stid] = currentTime;
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
                var data = JsonConvert.DeserializeObject<SensorDto>(content);
                return data;
            }
            return new SensorDto();
        }
    }
}
