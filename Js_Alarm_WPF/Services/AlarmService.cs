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
                    ai.GroupId,ai.Stid,ai.Location, ai.DelayTime, ai.Enable AS ItemEnable,
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
        public async void CheckAlarmState()
        {
            var alarmInfo = GetAlarmInfo();

            using var conn = new SqlConnection(_connectionStr);
            var sql = @"SELECT Stid, Enable FROM AlarmItem";
            var items = conn.Query<AlarmItemDto>(sql).ToList();
            foreach (var item in items)
            {
                var apiUrl = $"https://www.jsene.com/ConstructionSite/FuTsu/TSMC/getReal?stid={item.Stid}&list=1,2,3,4,5,6,7,8";
                var data = await CheckItemState(apiUrl); //0:停用 1:啟用i
                var isEnabled = data.vals.Length != 0;
                if (isEnabled != item.Enable)
                {
                    var updateSql = "UPDATE AlarmItem SET Enable = @Enable WHERE Stid = @Stid";
                    conn.Execute(updateSql, new { Enable = isEnabled, item.Stid });
                }
                //TODO 開發 alarmInfo 與 data.vals 的比對邏輯
            }
        }
        /// <summary>
        /// Check the state of the API
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <returns>True 表示啟用 false反之</returns>
        private static async Task<SensorDto> CheckItemState(string apiUrl)
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
