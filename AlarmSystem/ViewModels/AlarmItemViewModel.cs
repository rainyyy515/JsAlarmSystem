using AlarmSystem.Models;
using System.ComponentModel;

namespace AlarmSystem.ViewModels
{
    public class AlarmItemViewModel
    {
        public string? Stid { get; set; }

        public string? GroupId { get; set; }

        public string? Location { get; set; }

        public int DelayTime { get; set; }

        public string? TimeInterval { get; set; }

    }
}
