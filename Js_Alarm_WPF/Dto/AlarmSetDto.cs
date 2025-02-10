using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Js_Alarm_WPF.Dto
{
    public class AlarmSetDto
    {
        public string Stid { get; set; }

        public string ParameterColumn { get; set; }

        public string ParameterShow { get; set; }

        public int Threshold { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public DateTime NextCheckTime { get; set; }
    }
}
