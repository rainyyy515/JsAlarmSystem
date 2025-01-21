using AlarmSystem.Models;
using AlarmSystem.Services;
using AlarmSystem.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AlarmSystem.Controllers
{
    public class AlarmController(AlarmService alarmService, IMapper mapper) : Controller
    {
        private readonly AlarmService _alarmService = alarmService;
        private readonly IMapper _mapper = mapper;

        public async Task<IActionResult> Index()
        {
            IEnumerable<AlarmGroup> alarmGroups = await _alarmService.GetGroup(null);
            //var alarmItemViewModels = _mapper.Map<IEnumerable<AlarmItemViewModel>>(alarmItems);
            return View(alarmGroups);
        }
        public IActionResult CreateGroup(string groupId)
        {
            var result = _alarmService.GetAlarms(groupId);
            if (result == null)
            {
                ViewData["groupId"] = groupId;
                return View();
            }
            return NotFound();
        }
        //public IActionResult CreateItem(string groupId)
        //{
        //    var result = _alarmService.GetAlarms(groupId);
        //    if (result == null)
        //    {
        //        ViewData["groupId"] = groupId;
        //        return View();
        //    }
        //    return NotFound();
        //}
        public async Task<IActionResult> Edit(string groundId)
        {
            var alarmItem = await _alarmService.GetAlarms(groundId);
            return View(alarmItem);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(List<AlarmSettings> model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "≈Á√“•¢±—";
                return View(model);
            }
            var editedAlarmItem = await _alarmService.EditAlarm(model);
            if (!editedAlarmItem)
            {
                ViewData["Error"] = "ΩsøË•¢±—";
                return View(model);
            }
            return RedirectToAction("Index", "Alarm");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string groupId)
        {
            await _alarmService.DeleteGroup(groupId);
            return RedirectToAction("Index","Alarm");
        }
    }
}
