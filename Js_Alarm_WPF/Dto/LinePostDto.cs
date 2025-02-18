using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Js_Alarm_WPF.Dto
{
    public class LinePostDto
    {
        public string Url { get; set; }
        public Payload Payload { get; set; }
    }

    public class Payload
    {
        public string Title { get; set; }
        public string Mes { get; set; }
        public string Image { get; set; }
    }
}
