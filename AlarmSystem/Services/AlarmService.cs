using AlarmSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AlarmSystem.Services
{
    public class AlarmService(Js_LineAlarmContext context)
    {
        private readonly Js_LineAlarmContext _context = context;

        
        public async Task<List<AlarmItem>> GetAlarmItem(string? groupId)
        {
            var result = _context.AlarmItem.Where(x => x.GroupId == groupId);
            if (groupId == null)
            {
                result = _context.AlarmItem;
            }
            return await result.ToListAsync();
        }
        public async Task<AlarmItem> GetAlarm(string stid)
        {
            var result = await _context.AlarmItem.FindAsync(stid);
            if (result == null) {
                return null!;
            }
            return result;
        }
        public async Task<List<AlarmSettings>> GetAlarmSettings(string? stid)
        {
            var result = _context.AlarmSettings.Where(x => x.Stid == stid);
            if (stid == null)
            {
                result = _context.AlarmSettings;
            }

            return await result.ToListAsync();
        }
        public async Task<bool> CreateItem(AlarmItem alarmItem)
        {
            var item = await _context.AlarmItem.FindAsync(alarmItem.Stid);
            if (item != null)
            {
                return false;
            }
            _context.AlarmItem.Add(alarmItem);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CreateSettings(AlarmSettings settings)
        {
            settings.NextCheckTime = DateTime.Now.AddMinutes(1);
            _context.AlarmSettings.Add(settings);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EditAlarmItme(AlarmItem item)
        {
            var alarmItem = await _context.AlarmItem.FindAsync(item.Stid);
            if (alarmItem == null)
            {
                return false;
            }
            alarmItem.Stid = item.Stid;
            alarmItem.Location = item.Location;
            alarmItem.DelayTime = item.DelayTime;
            alarmItem.BreakAlarm = item.BreakAlarm;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EditAlarmSet(List<AlarmSettings> alarms)
        {
            foreach (var alarm in alarms)
            {
                var alarmSet = await _context.AlarmSettings.FindAsync(alarm.Id);
                if (alarmSet == null)
                {
                    return false;
                }
                alarmSet.Stid = alarm.Stid;
                alarmSet.ParameterColumn = alarm.ParameterColumn;
                alarmSet.ParameterShow = alarm.ParameterShow;
                alarmSet.Threshold = alarm.Threshold;
                alarmSet.StartTime = alarm.StartTime;
                alarmSet.EndTime = alarm.EndTime;
                alarmSet.NextCheckTime = alarm.NextCheckTime;
            }

            await _context.SaveChangesAsync();
            return true;
        }
       
        public async Task<bool> DeleteItem(string stid)
        {
            var alarmItem = await _context.AlarmItem.FindAsync(stid);
            if (alarmItem == null)
            {
                return false;
            }
            var alarmSets = await _context.AlarmSettings.Where(x => x.Stid == stid).ToListAsync();
            if (alarmSets.Count != 0)
            {
                _context.AlarmSettings.RemoveRange(alarmSets);
            }
            _context.AlarmItem.Remove(alarmItem);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteSet(int id)
        {
            var alarmSet = await _context.AlarmSettings.FindAsync(id);
            if (alarmSet == null)
            {
                return false;
            }
            _context.AlarmSettings.Remove(alarmSet);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
