using Mono.Options;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DeafAlsoPlayFps
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // 创建类级别的logger实例供整个应用使用
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        // 获取当前程序集名称用于日志路径
        private static readonly string moduleName = Assembly.GetExecutingAssembly().GetName().Name;

        private Mutex _mutex;
        private static string _uniqueEventName = $"{moduleName}_UniqueEventName";
        private static string _uniqueMutexName = $"{moduleName}_UniqueMutexName";
        private EventWaitHandle _exitEvent;

        private EventWaitHandle _switchOnEvent;
        private static string _switchOnEventName = $"{moduleName}_switchOnEventName";
        private EventWaitHandle _switchOffEvent;
        private static string _switchOffEventName = $"{moduleName}_switchOffEventName";
        private EventWaitHandle _switchPopupEvent;
        private static string _switchPopupEventName = $"{moduleName}_switchPopupEventName";
        public int SwitchValue { get; private set; } = -1;
        //leishenSdk.exe
        protected override void OnStartup(StartupEventArgs e)
        {

            // 在应用启动时配置NLog
            ConfigureNLog();

            bool exit = false;
            int gameId = 0;
            var options = new OptionSet {
                { "quit", v => exit = v != null },
                { "switch=", "切换参数，后接整数值", (int v) => SwitchValue = v },
                { "gameid=", "游戏ID参数，后接整数值", (int v) => gameId = v }
            };
            try
            {
                options.Parse(e.Args);
            }
            catch (OptionException ex)
            {
                MessageBox.Show("参数错误: " + ex.Message);
                Environment.Exit(1);
            }
            if (gameId != SettingsHelper.Instance?.Settings?.GameId)
            {
                _logger.Info($"GameId: {gameId}");

                // 退出旧实例
                using EventWaitHandle exitEvent = new(false, EventResetMode.AutoReset, _uniqueEventName);
                exitEvent.Set(); // Signal the event
                // 等待旧实例处理完毕
                Thread.Sleep(200); // 等待1秒，确保旧实例有时间处理退出信号
            }
            SettingsHelper.Instance!.Settings!.GameId = gameId;

            if (exit)
            {
                using (EventWaitHandle exitEvent = new(false, EventResetMode.AutoReset, _uniqueEventName))
                {
                    exitEvent.Set(); // Signal the event
                }

                // Exit current instance as well
                Environment.Exit(0);
                return;
            }
            bool isNewInstance;
            _mutex = new Mutex(true, _uniqueMutexName, out isNewInstance);
            // 使用 switchValue
            {
                if (SettingsHelper.Instance.Settings == null)
                {
                    _logger.Error("SettingsHelper.Instance.Settings is null");
                    return;
                }
                SettingsHelper.Instance.Settings.SwitchValue = SwitchValue;
                switch (SwitchValue)
                {
                    case -1:
                        using (EventWaitHandle switchPopupEvent = new EventWaitHandle(false, EventResetMode.AutoReset, _switchPopupEventName))
                        {
                            switchPopupEvent.Set(); // Signal the popup event
                        }
                        break;
                    case 0:
                        using (EventWaitHandle switchOffEvent = new EventWaitHandle(false, EventResetMode.AutoReset, _switchOffEventName))
                        {
                            switchOffEvent.Set(); // Signal the switch off event
                        }
                        break;
                    case 1:
                        using (EventWaitHandle switchOnEvent = new EventWaitHandle(false, EventResetMode.AutoReset, _switchOnEventName))
                        {
                            switchOnEvent.Set(); // Signal the switch on event
                        }
                        break;
                }

                if (!isNewInstance)
                {
                    Environment.Exit(0);
                }
            }

            if (!isNewInstance)
            {
                MessageBox.Show("程序已经在运行中");
                Environment.Exit(0);
            }

            // Create and listen for exit event
            _exitEvent = new EventWaitHandle(false, EventResetMode.AutoReset, _uniqueEventName);

            // Start a thread to listen for exit signal
             Task.Factory.StartNew(() =>
            {
                if (_exitEvent.WaitOne())
                {
                    // Exit signal received
                    Dispatcher.Invoke(() =>
                    {
                        Shutdown();
                    });
                }
            });

            _switchOffEvent = new EventWaitHandle(false, EventResetMode.AutoReset, _switchOffEventName);
            _switchOnEvent = new EventWaitHandle(false, EventResetMode.AutoReset, _switchOnEventName);
            _switchPopupEvent = new EventWaitHandle(false, EventResetMode.AutoReset, _switchPopupEventName);

            Task.Factory.StartNew(() =>
            {
                var handles = new WaitHandle[] { _switchOffEvent, _switchOnEvent, _switchPopupEvent };
                while (true)
                {
                    int index = WaitHandle.WaitAny(handles);
                    // 用 BeginInvoke 避免 UI 阻塞
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        switch (index)
                        {
                            case 0:
                                // 处理 switch off
                                if (this.MainWindow is MainWindow mainWindow)
                                {
                                    mainWindow.MainSwitch.IsChecked = false;
                                }
                                break;
                            case 1:
                                // 处理 switch on
                                if (this.MainWindow is MainWindow mainWindowOn)
                                {
                                    mainWindowOn.MainSwitch.IsChecked = true;
                                }
                                break;
                            case 2:
                                // 处理 switch popup
                                if (this.MainWindow is MainWindow mainWindowPop)
                                {
                                    mainWindowPop.BringWindowToFront(mainWindowPop);
                                    //mainWindowPop.Show();
                                    ////如果是最小化也要恢复窗口
                                    //mainWindowPop.WindowState = WindowState.Normal;
                                    //mainWindowPop.Activate();
                                    //mainWindowPop.ShowInTaskbar = true;
                                }
                                //MessageBox.Show("收到 switch popup 信号");
                                break;
                        }
                    }));
                }
            }, TaskCreationOptions.LongRunning);
            if (gameId != 0)
            {
                SettingsHelper.Instance.Settings.GameId = gameId;
            }
            base.OnStartup(e);

        }
        protected override void OnExit(ExitEventArgs e)
        {
            SettingsHelper.Instance.SaveSettings();
            // 记录应用退出
            _logger.Info("应用程序退出");

            // 确保所有日志都被写入
            LogManager.Shutdown();
            _mutex?.ReleaseMutex();
            _exitEvent?.Dispose();
            base.OnExit(e);
            // 在应用程序退出时执行清理操作
            // 例如：保存设置、释放资源等
        }
        private void ConfigureNLog()
        {
            try
            {
                // 创建NLog配置
                var config = new LoggingConfiguration();

                // 定义日志文件路径 (用户数据文件夹下)
                string logPath = Path.Combine(
                    "log",
                    $"{moduleName}");

                // 确保日志目录存在
                Directory.CreateDirectory(logPath);

                // 创建文件目标
                var fileTarget = new FileTarget("file")
                {
                    FileName = Path.Combine(logPath, "${shortdate}.log"),
                    Layout = "${longdate} | ${uppercase:${level}} | ${logger} | ${message} ${exception:format=ToString}",
                    ArchiveFileName = Path.Combine(logPath, "archive", "log.{#}.txt"),
                    ArchiveSuffixFormat = "yyyy-MM-dd",
                    ArchiveEvery = FileArchivePeriod.Day,
                    MaxArchiveFiles = 7, // 保留一周的日志
                    KeepFileOpen = true,
                    AutoFlush = true
                };

                // 添加文件目标到配置
                config.AddTarget(fileTarget);

                // 设置日志规则
                config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget); // 生产环境只记录Info及以上级别
                                                                           // 可选：添加调试输出目标 (在DEBUG模式下)
#if DEBUG
                var debugTarget = new DebugSystemTarget("debug")
                {
                    Layout = "${time} | ${level:uppercase=true} | ${message}"
                };
                config.AddTarget(debugTarget);
                config.AddRule(LogLevel.Trace, LogLevel.Fatal, debugTarget);
#endif

                // 应用配置
                LogManager.Configuration = config;
            }
            catch (Exception ex)
            {
                // 如果日志配置失败，至少在调试控制台输出错误
                Debug.WriteLine($"配置日志系统失败: {ex.Message}");
                //MessageBox.Show($"配置日志系统失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
