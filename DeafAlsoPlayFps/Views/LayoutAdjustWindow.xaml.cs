using DeafAlsoPlayFps.Properties;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace DeafAlsoPlayFps.Views
{
    public partial class LayoutAdjustWindow : Window
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private Border? _currentDragElement;
        private Point _dragStartPoint;
        private Dictionary<Border, Point> _originalPositions = new Dictionary<Border, Point>();

        // 获取主显示器尺寸
        private double _screenWidth = SystemParameters.PrimaryScreenWidth;
        private double _screenHeight = SystemParameters.PrimaryScreenHeight;
        // 保存DPI缩放因子
        private double _dpiScaleX = 1.0;
        private double _dpiScaleY = 1.0;

        public LayoutAdjustWindow()
        {
            InitializeComponent();
            this.Loaded += LayoutAdjustWindow_Loaded;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }

            if (e.Key == Key.Enter)
            {
                SavePositionsToSettings();
                this.Close();
            }
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            Common.Utils.Window.HideFromAltTab(this);

            // 获取DPI缩放因子
            GetDpiScale();
        }

        private void GetDpiScale()
        {
            return;
            try
            {
                // 获取窗口的DPI信息
                var dpi = VisualTreeHelper.GetDpi(this);
                _dpiScaleX = dpi.DpiScaleX;
                _dpiScaleY = dpi.DpiScaleY;
                _logger.Info($"系统DPI: {dpi.PixelsPerInchX}x{dpi.PixelsPerInchY}, DPI缩放: X={dpi.DpiScaleX}, Y={dpi.DpiScaleY}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "获取DPI缩放信息失败");
            }
        }

        private void LayoutAdjustWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Common.Utils.Window.HideFromAltTab(this);

            // 确保已获取DPI缩放因子
            GetDpiScale();
            SaveOriginalPositions();

            LoadPositionsFromSettings();
        }

        private void SaveOriginalPositions()
        {
            // 保存元素的原始Canvas位置
            _originalPositions[MainContainer] = new Point(
                Canvas.GetLeft(MainContainer),
                Canvas.GetTop(MainContainer));

            _originalPositions[LeftChannelContainer] = new Point(
                Canvas.GetLeft(LeftChannelContainer),
                Canvas.GetTop(LeftChannelContainer));

            _originalPositions[RightChannelContainer] = new Point(
                Canvas.GetLeft(RightChannelContainer),
                Canvas.GetTop(RightChannelContainer));
        }

        private void LoadPositionsFromSettings()
        {
            var settings = SettingsHelper.Instance?.Settings;
            if (settings != null)
            {
                if (settings.TopWindowPosition != null)
                {
                    // 应用DPI缩放调整 - 物理坐标转换为逻辑坐标
                    double adjustedX = settings.TopWindowPosition.X / _dpiScaleX;
                    double adjustedY = settings.TopWindowPosition.Y / _dpiScaleY;
                    if (adjustedX > _screenWidth)
                    {
                        adjustedX = _screenWidth - MainContainer.ActualWidth; // 确保不超出屏幕宽度
                    }
                    if (adjustedY > _screenHeight)
                    {
                        adjustedY = _screenHeight - MainContainer.ActualHeight; // 确保不超出屏幕高度
                    }
                    // 使用Canvas定位而非Margin
                    Canvas.SetLeft(MainContainer, adjustedX);
                    Canvas.SetTop(MainContainer, adjustedY);

                    _logger.Info($"TopWindowPosition - 原始: ({settings.TopWindowPosition.X}, {settings.TopWindowPosition.Y}), " +
                                 $"调整后: ({adjustedX}, {adjustedY})");
                }

                if (settings.LeftChannelPosition != null)
                {
                    double adjustedX = settings.LeftChannelPosition.X / _dpiScaleX;
                    double adjustedY = settings.LeftChannelPosition.Y / _dpiScaleY;

                    if (adjustedX > _screenWidth)
                    {
                        adjustedX = _screenWidth - LeftChannelContainer.ActualWidth; // 确保不超出屏幕宽度
                    }
                    if (adjustedY > _screenHeight)
                    {
                        adjustedY = _screenHeight - LeftChannelContainer.ActualHeight; // 确保不超出屏幕高度
                    }

                    // 使用Canvas定位
                    Canvas.SetLeft(LeftChannelContainer, adjustedX);
                    Canvas.SetTop(LeftChannelContainer, adjustedY);

                    _logger.Info($"LeftChannelPosition - 原始: ({settings.LeftChannelPosition.X}, {settings.LeftChannelPosition.Y}), " +
                                 $"调整后: ({adjustedX}, {adjustedY})");
                }

                if (settings.RightChannelPosition != null)
                {
                    double adjustedX = settings.RightChannelPosition.X / _dpiScaleX;
                    double adjustedY = settings.RightChannelPosition.Y / _dpiScaleY;
                    if(adjustedX> _screenWidth)
                    {
                        adjustedX = _screenWidth - RightChannelContainer.ActualWidth; // 确保不超出屏幕宽度
                    }
                    if(adjustedY > _screenHeight)
                    {
                        adjustedY = _screenHeight - RightChannelContainer.ActualHeight; // 确保不超出屏幕高度
                    }
                    // 使用Canvas定位
                    Canvas.SetLeft(RightChannelContainer, adjustedX);
                    Canvas.SetTop(RightChannelContainer, adjustedY);

                    _logger.Info($"RightChannelPosition - 原始: ({settings.RightChannelPosition.X}, {settings.RightChannelPosition.Y}), " +
                                 $"调整后: ({adjustedX}, {adjustedY})");
                }

                _logger.Info($"加载位置完成，应用DPI缩放: {_dpiScaleX}x");
            }
        }

        private void SavePositionsToSettings()
        {
            var settings = SettingsHelper.Instance?.Settings;
            if (settings != null)
            {
                // 逻辑坐标转换为物理坐标 - 乘以DPI缩放因子
                settings.TopWindowPosition = new Point(
                    Canvas.GetLeft(MainContainer) * _dpiScaleX,
                    Canvas.GetTop(MainContainer) * _dpiScaleY);

                settings.LeftChannelPosition = new Point(
                    Canvas.GetLeft(LeftChannelContainer) * _dpiScaleX,
                    Canvas.GetTop(LeftChannelContainer) * _dpiScaleY);

                settings.RightChannelPosition = new Point(
                    Canvas.GetLeft(RightChannelContainer) * _dpiScaleX,
                    Canvas.GetTop(RightChannelContainer) * _dpiScaleY);

                _logger.Info($"保存位置: TopWindowPosition ({settings.TopWindowPosition}), " +
                             $"LeftChannelContainer({settings.LeftChannelPosition}), " +
                             $"RightChannelContainer({settings.RightChannelPosition}), " +
                             $"应用DPI缩放: {_dpiScaleX}x");

                SettingsHelper.Instance?.SaveSettings();
            }
        }

#if DEBUG
        private void LogPosition()
        {
            // 使用Canvas.GetLeft/GetTop替代Margin
            var TopWindowPosition = new Point(
               Canvas.GetLeft(MainContainer) * _dpiScaleX,
               Canvas.GetTop(MainContainer) * _dpiScaleY);

            var LeftChannelPosition = new Point(
                Canvas.GetLeft(LeftChannelContainer) * _dpiScaleX,
                Canvas.GetTop(LeftChannelContainer) * _dpiScaleY);

            var RightChannelPosition = new Point(
                Canvas.GetLeft(RightChannelContainer) * _dpiScaleX,
                Canvas.GetTop(RightChannelContainer) * _dpiScaleY);

            _logger.Info($"当前位置: TopWindowPosition ({TopWindowPosition}), " +
                         $"LeftChannelContainer({LeftChannelPosition}), " +
                         $"RightChannelContainer({RightChannelPosition}), " +
                         $"应用DPI缩放: {_dpiScaleX}x");

        }
#endif

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                _currentDragElement = border;
                _dragStartPoint = e.GetPosition(this);
                border.CaptureMouse();
                e.Handled = true;
            }
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (_currentDragElement != null && _currentDragElement.IsMouseCaptured)
            {
                // 获取当前鼠标位置（相对于窗口）
                Point currentMousePosition = e.GetPosition(this);

                // 计算元素应该移动到的位置
                double newLeft = currentMousePosition.X - _dragStartPoint.X + Canvas.GetLeft(_currentDragElement);
                double newTop = currentMousePosition.Y - _dragStartPoint.Y + Canvas.GetTop(_currentDragElement);

                // 更新鼠标起始点为当前位置，用于下一次计算
                _dragStartPoint = currentMousePosition;

                // 确保元素不会移出窗口边界
                newLeft = Math.Max(0, Math.Min(newLeft, this.ActualWidth - _currentDragElement.ActualWidth));
                newTop = Math.Max(0, Math.Min(newTop, this.ActualHeight - _currentDragElement.ActualHeight));

                // 使用Canvas定位
                Canvas.SetLeft(_currentDragElement, newLeft);
                Canvas.SetTop(_currentDragElement, newTop);

                e.Handled = true;

                // 根据主容器位置调整提示文本的位置
                double mainContainerTop = Canvas.GetTop(MainContainer);
                if (mainContainerTop > _screenHeight / 3 * 2)
                {
                    Tips.VerticalAlignment = VerticalAlignment.Top;
                }
                else if (mainContainerTop < _screenHeight / 3)
                {
                    Tips.VerticalAlignment = VerticalAlignment.Bottom;
                }
            }
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentDragElement != null)
            {
                _currentDragElement.ReleaseMouseCapture();
                //SavePositionsToSettings(); // 注释掉了，只在按Enter时保存
                _currentDragElement = null;
                e.Handled = true;
#if DEBUG
                LogPosition();
#endif
            }
        }
    }
}