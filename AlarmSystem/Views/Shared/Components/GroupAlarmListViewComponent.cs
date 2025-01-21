using AlarmSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlarmSystem.Views.Shared.Components
{
    public class GroupAlarmListViewComponent(AlarmService alarmService) : ViewComponent
    {
        private readonly AlarmService _alarmService = alarmService;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var alarms = await _alarmService.GetGroupAlarmList();
            return View(alarms);
        }
    }
}