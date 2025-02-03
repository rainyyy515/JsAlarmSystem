using System.ComponentModel.DataAnnotations;

namespace AlarmSystem.Dtos
{
    public class AlarmSettingsDto
    {
        public string? Stid { get; set; }

        [Required(ErrorMessage = "欄位為必填")]
        public string? ParameterColumn { get; set; }

        [Required(ErrorMessage = "欄位名稱為必填")]
        public string? ParameterShow { get; set; }

        [Required(ErrorMessage = "欄位名稱為必填")]
        public int Threshold { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
    }
}
