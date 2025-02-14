using AlarmSystem.Dtos;
using AlarmSystem.Models;
using AlarmSystem.Services;
using AlarmSystem.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AlarmSystem.Controllers
{
    public class AlarmController(AlarmService alarmService, IMapper mapper, GroupService groupService) : Controller
    {
        private readonly AlarmService _alarmService = alarmService;
        private readonly GroupService _groupService = groupService;
        private readonly IMapper _mapper = mapper;

        public async Task<IActionResult> Index()
        {
            IEnumerable<AlarmGroup> alarmGroups = await _groupService.GetGroup(null);
            return View(alarmGroups);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGroup(AlarmGroupDto groupDto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var group = _mapper.Map<AlarmGroupDto, AlarmGroup>(groupDto);
            var result = await _groupService.CreateGroup(group);
            if (!result)
            {
                TempData["Error"] = $"{DateTime.Now}：Token已存在新增失敗";
                return RedirectToAction("Index");
            }
            TempData["Result"] = $"{DateTime.Now}：{group.GroupName} 新增成功";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem(AlarmItem item)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var result = await _alarmService.CreateItem(item);
            if (!result)
            {
                TempData["Error"] = "新增失敗 Stid 已存在";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CreateEdit(string stid)
        {
            var alarmItem = await _alarmService.GetAlarm(stid);
            var vm = _mapper.Map<AlarmItem, AlarmItemViewModel>(alarmItem);
            var alarmSettings = await _alarmService.GetAlarmSettings(stid);
            vm.Settings = alarmSettings;
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSettings(AlarmSettingsDto settingsDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "驗證失敗";
                return RedirectToAction("CreateEdit", new { stid = settingsDto.Stid });
            }
            var settings = _mapper.Map<AlarmSettingsDto, AlarmSettings>(settingsDto);
            var result = await _alarmService.CreateSettings(settings);
            if (!result)
            {
                TempData["Error"] = "新增失敗";
                return RedirectToAction("CreateEdit", new { stid = settingsDto.Stid });
            }
            return RedirectToAction("CreateEdit", new { stid = settingsDto.Stid });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEdit(AlarmItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "驗證失敗";
                return RedirectToAction("CreateEdit", new { stid = model.Stid });
            }
            if (model.Settings == null)
            {
                TempData["Error"] = "至少要添加一筆監測參數";
                return RedirectToAction("CreateEdit", new { stid = model.Stid });
            }
            foreach (var item in model.Settings!)
            {
                item.NextCheckTime = DateTime.Now.AddMinutes(5);
            }
            var edit = _mapper.Map<AlarmItemViewModel, AlarmItem>(model);
            var editedAlarmItem = await _alarmService.EditAlarmItme(edit);
            var editedAlarmSet = await _alarmService.EditAlarmSet(model.Settings);
            if (!editedAlarmSet || !editedAlarmItem)
            {
                TempData["Error"] = "編輯失敗";
                return RedirectToAction("CreateEdit", new { stid = model.Stid });
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GroupEdit(AlarmGroupDto groupDto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var group = _mapper.Map<AlarmGroupDto, AlarmGroup>(groupDto);
            var editedAlarmItem = await _groupService.GroupEdit(group);
            if (!editedAlarmItem)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string groupId)
        {
            await _groupService.DeleteGroup(groupId);
            return RedirectToAction("Index", "Alarm");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSet(int id, string stid)
        {
            await _alarmService.DeleteSet(id);
            return RedirectToAction("CreateEdit", new { stid });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(string stid)
        {
            await _alarmService.DeleteItem(stid);
            return RedirectToAction("Index");
        }
    }
}
