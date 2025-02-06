using System.Configuration;
using System.Windows;
using System.Windows.Threading;

namespace Js_Alarm_WPF
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private readonly string _connectionStr;

        public MainWindow()
        {
            _connectionStr = ConfigurationManager.AppSettings["DbConnectionStr"]!;
            InitializeComponent();
            InitUI();
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
            if (currentTime.Second == 3)
            {
                if (LogInfo.Items.Count >= 50)
                {
                    // 删除最早UI紀錄，避免無線增長
                    LogInfo.Items.RemoveAt(0);
                }

                LogInfo.Items.Add($"{DateTime.Now}：監測維感 {_connectionStr}");
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