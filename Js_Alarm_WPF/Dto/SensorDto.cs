namespace Js_Alarm_WPF.Dto
{
    public class SensorDto
    {
        public Val[] vals { get; set; }
        public DateTime time { get; set; }
        public string stid { get; set; }
    }

    public class Val
    {
        public string parameter { get; set; }
        public string unit { get; set; }
        public float val { get; set; }
    }

}
