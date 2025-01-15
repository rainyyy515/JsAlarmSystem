using LineSystem.Models;
using Serilog;

namespace LineSystem.Services
{
    public class AlarmServer
    {
        private readonly LineAlarmContext _alarmContext;

        public AlarmServer(LineAlarmContext alarmContext)
        {
            _alarmContext = alarmContext;
        }
        public IQueryable<AlarmItems> GetAlarmItems(string groupId)
        {
            var result = _alarmContext.AlarmItems.Where(x => x.GroupId == groupId);
            return result;
        }
        public bool CreateAlarmItem(AlarmItems alarmItem)
        {
            try
            {
                _alarmContext.AlarmItems.Add(alarmItem);
                _alarmContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return false;
            }
        }
        public bool CreateGroup(string groupId)
        {
            try
            {
                var insert = new LineGroups()
                {
                    GroupId = groupId,
                    AlarmDelay = 60,
                    Enable = true
                };
                _alarmContext.LineGroups.Add(insert);
                _alarmContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return false;
            }
        }
        public bool DeleteGroup(string groupId)
        {
            try
            {
                var group = _alarmContext.LineGroups.FirstOrDefault(x => x.GroupId == groupId);
                var alarmItems = _alarmContext.AlarmItems.Where(x => x.GroupId == groupId).ToList();
                if (group != null)
                {
                    _alarmContext.LineGroups.Remove(group);
                    _alarmContext.AlarmItems.RemoveRange(alarmItems);
                    _alarmContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return false;
            }
        }
    }
}
