using System.ComponentModel.DataAnnotations;

namespace AlarmSystem.Dtos
{
    public class AlarmGroupDto
    {
        [Required(ErrorMessage ="群組Token必填")]
        public string? GroupId { get; set; }

        [Required(ErrorMessage = "群組名稱必填")]
        public string? GroupName { get; set; }

        public bool Enable { get; set; }
    }
}
