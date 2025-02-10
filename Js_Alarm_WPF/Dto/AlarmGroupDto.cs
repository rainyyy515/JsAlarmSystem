using AlarmSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Js_Alarm_WPF.Dto
{
    public class AlarmGroupDto
    {
        public string GroupId { get; set; }

        public bool Enable { get; set; }

        public ICollection<AlarmItemDto> AlarmItemDto { get; set; }
    }
}
