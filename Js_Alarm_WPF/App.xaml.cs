using Serilog;
using System.Windows;
using System.Windows.Threading;

namespace Js_Alarm_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(path: "Logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
                .CreateLogger();

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        // 處理未處理的例外
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Fatal(e.Exception, "發生未處理的異常");
            e.Handled = true;
        }

        // 應用程序啟動事件
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Log.Information("警報程序啟動");
        }

        // 應用程序退出事件
        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("警報程序關閉");
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }

}
