using Js_Alarm_WPF.Services;
using System.Configuration;
using System.Windows;
using System.Windows.Threading;

namespace Js_Alarm_WPF
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private readonly AlarmService _alarmService;

        public MainWindow()
        {
            InitializeComponent();
            InitUI();
            _alarmService = new AlarmService();
        }
        private void StartMonitoring()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick!;
            _timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            //資料會有延遲到資料庫，所以需抓前3分鐘的值
            var currentTime = DateTime.Now.AddMinutes(-3);
            if (currentTime.Second <= 60)
            {
                _alarmService.CheckAlarmState();
                if (LogInfo.Items.Count >= 100)
                {
                    LogInfo.Items.Add($"{DateTime.Now}：大於100筆");
                    var removeCount = LogInfo.Items.Count - 100;
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
                LogInfo.ScrollIntoView(LogInfo.Items[^1]);
            }
        }
        private void InitUI()
        {
            LogInfo.Items.Clear();
            LogInfo.Items.Add($"{DateTime.Now}：系統啟動");
            StartMonitoring();
        }
        protected override void OnClosed(EventArgs e)
        {
            _timer.Stop();
            base.OnClosed(e);
        }
    }
}