using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AlarmSystem.Dtos
{
    public class AlarmItemDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "編號是必填的")]
        [DisplayName("編號")]
        public string? Stid { get; set; }

        [Required(ErrorMessage = "群組ID是必填的")]
        public string? GroupId { get; set; }

        [Required(ErrorMessage = "地點是必填的")]
        [DisplayName("地點")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "欄位是必填的")]
        [DisplayName("欄位")]
        public string? ParameterColumn { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "閾值必須是非負整數")]
        [DisplayName("閾值")]
        public int Threshold { get; set; }

        [Required(ErrorMessage = "欄位名稱是必填的")]
        [DisplayName("欄位名稱")]
        public string? ParameterShow { get; set; }

        [DisplayName("開始時間")]
        public TimeOnly StartTime { get; set; }

        [DisplayName("結束時間")]
        public TimeOnly EndTime { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "延遲時間必須是非負整數")]
        [DisplayName("延遲時間(分)")]
        public int DelayTime { get; set; }

        public DateTime? NextCheckTime { get; set; }
    }
}
