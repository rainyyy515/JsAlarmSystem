using isRock.LineBot;
using LineSystem.Models;
using LineSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace LineSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineWebHookController : ControllerBase
    {
        private readonly Bot bot;
        private readonly AlarmServer _alarmServer;
        public LineWebHookController(Bot bot, AlarmServer alarmServer)
        {
            this.bot = bot;
            _alarmServer = alarmServer;
        }
        [HttpGet]
        public string Get()
        {
            return "LineWebHook監聽中";
        }
        /// <summary>
        /// 處理Line事件
        /// </summary>
        /// <param name="lineEvent">事件</param>
        [HttpPost]
        public void Post([FromBody] LineEvent lineEvent)
        {
            if (lineEvent.events[0].type == "join")
            {
                var result = _alarmServer.CreateGroup(lineEvent.events[0].source.groupId);
                if (result)
                {
                    bot.ReplyMessage(lineEvent.events[0].replyToken, "群組新增成功");
                }
                else
                {
                    bot.ReplyMessage(lineEvent.events[0].replyToken, "群組新增失敗，請查看LOG檔");
                }
                return;
            }
            if (lineEvent.events[0].type == "leave")
            {
                var result = _alarmServer.DeleteGroup(lineEvent.events[0].source.groupId);
                if (result)
                {
                    Log.Information("群組刪除成功");
                }
                else
                {
                    Log.Information("群組不存在，刪除失敗");
                }
                return;
            }
            if (lineEvent.events[0].message != null && lineEvent.events[0].message.text != null)
            {
                if (lineEvent.events[0].message.text == "功能")
                {
                    var quickReply = new QuickReply();
                    var alarmItems = _alarmServer.GetAlarmItems(lineEvent.events[0].source.groupId);
                    if (alarmItems.Any())
                    {
                        var text = string.Empty;
                        foreach (var item in alarmItems)
                        {
                            text += item.Stid;
                        }
                        quickReply.items.Add(new QuickReplyMessageAction("監測測站",text));
                        var textMessage = new TextMessage("功能項目開啟")
                        {
                            quickReply = quickReply
                        };
                        bot.ReplyMessage(lineEvent.events[0].replyToken, textMessage);
                    }
                    bot.ReplyMessage(lineEvent.events[0].replyToken, "尚未添加感測器");
                }
                if (lineEvent.events[0].message.text.Contains("綁定stid:"))
                {
                    var text = lineEvent.events[0].message.text;
                    var lines = text.Split("\n");
                    var line = lines[0].Split(";");
                    var alarmItem = new AlarmItems()
                    {
                        GroupId = lineEvent.events[0].source.groupId,
                        Stid = line[0].Substring(7),
                        Location = line[1],
                    };
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var item = lines[i].Split(";");
                        alarmItem.ParameterColumn = item[0];
                        alarmItem.Threshold = Convert.ToInt32(item[1]);
                        alarmItem.ParameterShow = item[2];
                        var result = _alarmServer.CreateAlarmItem(alarmItem);
                        if (!result)
                        {
                            bot.ReplyMessage(lineEvent.events[0].replyToken, $"新增失敗，參數錯誤!!!");
                        }
                    }
                    bot.ReplyMessage(lineEvent.events[0].replyToken, $"{line[0].Substring(2)} 綁定成功");
                    return;
                }
            }

        }
        /// <summary>
        /// 發送警報訊息
        /// </summary>
        /// <param name="request"></param>
        [HttpPost("SendMessage")]
        public void Post(MessageRequest request)
        {
            Log.Information(request.GroupId!);
            Log.Information(request.Text!);
            //bot.PushMessage(request.GroupId,request.Text);
        }
    }
}
