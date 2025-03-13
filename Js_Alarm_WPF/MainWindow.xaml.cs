using Js_Alarm_WPF.Services;
using System.Windows;
using System.Windows.Threading;

namespace Js_Alarm_WPF
{
    public partial class MainWindow : Window
    {
        private Timer _timer;
        private readonly AlarmService _alarmService;

        public MainWindow()
        {
            InitializeComponent();
            InitUI();
            _alarmService = new AlarmService();
        }
        private void StartMonitoring()
        {
            var now = DateTime.Now;
            int msToNextMinute = (60 - now.Second) * 1000 + 3000;
            LogInfo.Items.Add($"{DateTime.Now}：警報系統在{63 - now.Second}秒後 啟動");

            _timer = new Timer(TimerCallback, null, msToNextMinute, 1000);
        }
        private void TimerCallback(object state)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Timer_Tick();
            });
        }
        private void Timer_Tick()
        {
            var currentTime = DateTime.Now;
            if (LogInfo.Items.Count >= 50)
            {
                var removeCount = LogInfo.Items.Count - 50;
                // 删除最早UI紀錄，避免無線增長
                for (int i = 0; i < removeCount; i++)
                {
                    if (LogInfo.Items.Count == 0)
                    {
                        break;
                    }
                    LogInfo.Items.RemoveAt(0);
                }
            }
            if (currentTime.Second == 3)
            {
                LogInfo.Items.Add($"{currentTime}：執行監測");
                _alarmService.SendAlarmMessage();
            }
            LogInfo.ScrollIntoView(LogInfo.Items[^1]);
        }
        private void InitUI()
        {
            LogInfo.Items.Clear();
            StartMonitoring();
        }
        protected override void OnClosed(EventArgs e)
        {
            _timer?.Dispose();
            base.OnClosed(e);
        }
    }
}