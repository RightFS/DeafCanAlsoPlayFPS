﻿using System;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using NLog;

namespace DeafAlsoPlayFps.ViewModel
{
    public partial class ChannelDifferenceViewModel : ObservableObject, IDisposable
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly DispatcherTimer _smoothingTimer;
        private const double MaxBarWidth = 170.0; // 每侧最大宽度
        private const double SmoothingFactor = 0.7; // 平滑系数
        private const double CenterPosition = 180.0; // 中心位置
        [ObservableProperty]
        private double _leftBarWidth = 0;
        
        [ObservableProperty]
        private double _leftBarPosition = CenterPosition; // 中心位置
        
        [ObservableProperty]
        private double _rightBarWidth = 0;
        
        [ObservableProperty]
        private string _differenceText = "平衡";

        private double _targetLeftWidth = 0;
        private double _targetLeftPosition = CenterPosition;
        private double _targetRightWidth = 0;
        private string _targetDifferenceText = "平衡";

        public ChannelDifferenceViewModel()
        {
            // 创建平滑动画定时器
            _smoothingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // 约60FPS
            };
            _smoothingTimer.Tick += SmoothingTimer_Tick;
            _smoothingTimer.Start();
        }

        public void UpdateChannelDifference(float leftLevel, float rightLevel)
        {
            try
            {
                // 计算声道差值 (-1 到 +1，负值表示左声道更强，正值表示右声道更强)
                float difference = rightLevel - leftLevel;
                
                // 限制差值范围
                difference = Math.Max(-1.0f, Math.Min(1.0f, difference));
                
                if (Math.Abs(difference) < 0.02f)
                {
                    // 差值很小，显示平衡状态
                    _targetLeftWidth = 0;
                    _targetLeftPosition = CenterPosition;
                    _targetRightWidth = 0;
                    _targetDifferenceText = "平衡";
                }
                else if (difference < 0)
                {
                    // 左声道更强
                    var intensity = Math.Abs(difference);
                    _targetLeftWidth = intensity * MaxBarWidth;
                    _targetLeftPosition = CenterPosition - _targetLeftWidth;
                    _targetRightWidth = 0;
                    _targetDifferenceText = $"L {(intensity * 100):F0}%";
                }
                else
                {
                    // 右声道更强
                    var intensity = difference;
                    _targetLeftWidth = 0;
                    _targetLeftPosition = CenterPosition;
                    _targetRightWidth = intensity * MaxBarWidth;
                    _targetDifferenceText = $"R {(intensity * 100):F0}%";
                }
                
                // 调试输出
                System.Diagnostics.Debug.WriteLine($"声道差值: L={leftLevel:F3}, R={rightLevel:F3}, 差值={difference:F3}, 文本={_targetDifferenceText}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "更新声道差值失败");
            }
        }

        private void SmoothingTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // 平滑动画处理
                var leftWidthDiff = _targetLeftWidth - LeftBarWidth;
                var leftPosDiff = _targetLeftPosition - LeftBarPosition;
                var rightWidthDiff = _targetRightWidth - RightBarWidth;

                if (Math.Abs(leftWidthDiff) > 0.5)
                {
                    LeftBarWidth += leftWidthDiff * (1 - SmoothingFactor);
                }
                else
                {
                    LeftBarWidth = _targetLeftWidth;
                }

                if (Math.Abs(leftPosDiff) > 0.5)
                {
                    LeftBarPosition += leftPosDiff * (1 - SmoothingFactor);
                }
                else
                {
                    LeftBarPosition = _targetLeftPosition;
                }

                if (Math.Abs(rightWidthDiff) > 0.5)
                {
                    RightBarWidth += rightWidthDiff * (1 - SmoothingFactor);
                }
                else
                {
                    RightBarWidth = _targetRightWidth;
                }

                // 更新文本（不需要平滑）
                DifferenceText = _targetDifferenceText;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "声道差值平滑动画处理失败");
            }
        }

        public void Dispose()
        {
            try
            {
                if (_smoothingTimer != null)
                {
                    _smoothingTimer.Stop();
                    _smoothingTimer.Tick -= SmoothingTimer_Tick;
                }
                _logger.Info("ChannelDifferenceViewModel已释放资源");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "释放ChannelDifferenceViewModel资源时发生错误");
            }
        }
    }
}
