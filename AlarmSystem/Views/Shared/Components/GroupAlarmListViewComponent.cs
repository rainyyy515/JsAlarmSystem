using AlarmSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlarmSystem.Views.Shared.Components
{
    public class GroupAlarmListViewComponent(GroupService groupService) : ViewComponent
    {
        private readonly GroupService _groupService = groupService;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var alarms = await  _groupService.GetGroupAlarmList();
            return View(alarms);
        }
    }
}