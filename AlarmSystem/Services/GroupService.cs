using AlarmSystem.Models;
using AlarmSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AlarmSystem.Services
{
    public class GroupService(Js_LineAlarmContext context)
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
                                                      Enable = a.Enable
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
        public async Task<bool> CreateGroup(AlarmGroup alarmGroup)
        {
            if (_context.AlarmGroup.Any(x => x.GroupId == alarmGroup.GroupId))
            {
                return false;
            }
            _context.AlarmGroup.Add(alarmGroup);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> GroupEdit(AlarmGroup group)
        {
            var item = await _context.AlarmGroup.FindAsync(group.GroupId);
            if (item == null)
            {
                return false;
            }
            item.GroupName = group.GroupName;
            item.Enable = group.Enable;

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
            var groupItems = await _context.AlarmItem.Where(x => x.GroupId == groupId).ToListAsync();
            if (groupItems.Count != 0)
            {
                foreach (var item in groupItems)
                {
                    var settings = await _context.AlarmSettings.Where(x => x.Stid == item.Stid).ToListAsync();
                    if (settings.Count != 0)
                    {
                        _context.AlarmSettings.RemoveRange(settings);
                    }
                }
                _context.AlarmItem.RemoveRange(groupItems);
            }
            _context.AlarmGroup.Remove(group);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
