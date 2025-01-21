namespace AlarmSystem.ViewModels
{
    public class GroupAlarmListViewModel
    {
        public string? GroupId { get; set; }

        public string? GroupName { get; set; }

        public bool Enable { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<AlarmItemViewModel>? AlarmItems{ get; set; }

    }
}
