using isRock.LineBot;
using LineSystem.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LineSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineAlarmController : ControllerBase
    {
        private readonly Bot bot;
        private readonly AlarmServer _alarmServer;

        public LineAlarmController(Bot bot, AlarmServer alarmServer)
        {
            this.bot = bot;
            _alarmServer = alarmServer;
        }

        // GET: api/<LineAlarmController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            
            return new string[] { "value1", "value2" };
        }

        // GET api/<LineAlarmController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LineAlarmController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<LineAlarmController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LineAlarmController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
