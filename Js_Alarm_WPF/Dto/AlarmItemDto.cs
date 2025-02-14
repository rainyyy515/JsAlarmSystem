using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Js_Alarm_WPF.Dto
{
    public class AlarmItemDto
    {
        public string GroupId { get; set; }

        public string Stid { get; set; }

        public string Location { get; set; }

        public int DelayTime { get; set; }

        public bool ItemEnable { get; set; }

        public bool BreakAlarm { get; set; }

        public virtual ICollection<AlarmSetDto> AlarmSettingsDto { get; set; }

    }
}
