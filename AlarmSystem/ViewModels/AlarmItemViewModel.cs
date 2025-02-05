using AlarmSystem.Dtos;
using AlarmSystem.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AlarmSystem.ViewModels
{
    public class AlarmItemViewModel
    {
        [Required(ErrorMessage = "編號必填")]
        public string? Stid { get; set; }

        [Required(ErrorMessage ="地點必填")]
        [DisplayName("地點")]
        public string? Location { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "延遲時間必須大於0")]
        [DisplayName("延遲時間(分)")]
        public int DelayTime { get; set; }

        public List<AlarmSettings>? Settings { get; set; }
    }
}
