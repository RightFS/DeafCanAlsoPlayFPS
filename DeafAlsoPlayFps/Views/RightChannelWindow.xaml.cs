using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using DeafAlsoPlayFps.ViewModel;
using NLog;

namespace DeafAlsoPlayFps.Views
{
    public partial class RightChannelWindow : Window
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private SingleChannelViewModel _viewModel;
        // Win32 常量和函数导入
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int GWL_EXSTYLE = (-20);
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
        public RightChannelWindow()
        {
            InitializeComponent();
            _viewModel = new SingleChannelViewModel();
            DataContext = _viewModel;
            InputMethod.SetIsInputMethodEnabled(this, false);
            InputMethod.SetInputScope(this, new InputScope());
            // 设置窗口属性
            this.WindowStyle = WindowStyle.None;      // 无边框
            this.AllowsTransparency = true;           // 允许透明
            this.Background = System.Windows.Media.Brushes.Transparent;  // 背景透明
            this.Topmost = true;                      // 总在最前
            this.ShowInTaskbar = false;               // 不在任务栏显示
            // 设置窗口位置到屏幕右侧
            SetWindowPositionRight();

            Loaded += RightChannelWindow_Loaded;
        }
        private void TipsWindow_SourceInitialized()
        {
            // 获取窗口句柄
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            // 获取当前窗口样式
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

            // 添加 WS_EX_TRANSPARENT 样式使窗口点击穿透
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            Common.Utils.Window.HideFromAltTab(this);
        }
        private void SetWindowPositionRight()
        {
            try
            {
                // 获取主显示器尺寸
                var screenWidth = SystemParameters.PrimaryScreenWidth;
                var screenHeight = SystemParameters.PrimaryScreenHeight;

                Point? pos = SettingsHelper.Instance?.Settings?.RightChannelPosition;
                if (pos == null || !pos.HasValue)
                {
                    // 将窗口放置在屏幕右侧中央
                    this.Left = screenWidth - this.Width;
                    this.Top = (screenHeight - this.Height) / 2;
                }
                else
                {
                    // 使用保存的位置
                    this.Left = pos.Value.X;
                    this.Top = pos.Value.Y;
                }

                _logger.Info($"右声道窗口位置设置为: ({this.Left}, {this.Top}) - 屏幕: {screenWidth}x{screenHeight}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "设置右声道窗口位置失败");
            }
        }

        private void RightChannelWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.Info("右声道窗口已加载");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "加载右声道窗口失败");
            }

            TipsWindow_SourceInitialized();
        }

        public void UpdateLevel(float level)
        {
            _viewModel?.UpdateLevel(level);
        }
        public void UpdatePosition(double left, double top)
        {
            this.Left = left;
            this.Top = top;
        }
        public void Show(bool show)
        {
            if (show)
                this.Show();
            else
                this.Hide();
        }
    }
}
