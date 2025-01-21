using AlarmSystem.Models;
using AlarmSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AlarmSystem.Services
{
    public class AlarmService(Js_LineAlarmContext context)
    {
        private readonly Js_LineAlarmContext _context = context;

        public async Task<IEnumerable<GroupAlarmListViewModel>> GetGroupAlarmList()
        {
            var result = await (from g in _context.AlarmGroup
                                select new GroupAlarmListViewModel
                                {
                                    GroupId = g.GroupId,
                                    GroupName = g.GroupName,
                                    Enable = g.Enable,
                                    CreateDate = g.CreateDate,
                                    AlarmItems = (from a in _context.AlarmItem
                                                  where g.GroupId == a.GroupId
                                                  select new AlarmItemViewModel
                                                  {
                                                      Stid = a.Stid,
                                                      Location = a.Location,
                                                      DelayTime = a.DelayTime,
                                                  }).ToList()
                                }).ToListAsync();

            return result;
        }
        public async Task<IEnumerable<AlarmGroup>> GetGroup(string? groupId)
        {
            var result = _context.AlarmGroup.Where(x => x.GroupId == groupId);
            if (groupId == null)
            {
                result = _context.AlarmGroup;
            }
            return await result.ToListAsync();
        }
        public async Task<IEnumerable<AlarmItem>> GetAlarms(string? stid)
        {
            var result = _context.AlarmItem.Where(x => x.Stid == stid);
            if (stid == null)
            {
                result = _context.AlarmItem;
            }
            return await result.ToListAsync();
        }
        public async Task<List<AlarmSettings>> GetAlarmSettings(int? id)
        {
            var  result = _context.AlarmSettings.Where(x => x.Id == id);
            if (id == null)
            {
                result = _context.AlarmSettings;
            }
            
            return await result.ToListAsync();
        }
        //public async Task<AlarmItem> CreateAlarm(AlarmItem alarmItem)
        //{
        //    Console.WriteLine("建立警報");

        //    return alarmItem;
        //}
        public async Task<bool> EditAlarm(List<AlarmSettings> alarms)
        {
            Console.WriteLine("編輯警報");
            foreach (var alarm in alarms)
            {
                var alarmItem = await _context.AlarmSettings.FindAsync(alarm.Id);
                if (alarmItem == null)
                {
                    return false;
                }
                alarmItem.Stid = alarm.Stid;
                alarmItem.ParameterColumn = alarm.ParameterColumn;
                alarmItem.ParameterShow = alarm.ParameterShow;
                alarmItem.Threshold = alarm.Threshold;
                alarmItem.NextCheckTime = alarm.NextCheckTime;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAlarm(int id)
        {
            var alarmItem = await _context.AlarmItem.FindAsync(id);
            if (alarmItem == null)
            {
                return false;
            }
            _context.AlarmItem.Remove(alarmItem);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteGroup(string groupId)
        {
            var group = await _context.AlarmGroup.FindAsync(groupId);
            if (group == null)
            {
                return false;
            }
            _context.AlarmGroup.Remove(group);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
