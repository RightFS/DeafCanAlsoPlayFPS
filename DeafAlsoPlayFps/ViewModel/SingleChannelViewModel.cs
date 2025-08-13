using System;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using NLog;

namespace DeafAlsoPlayFps.ViewModel
{
    public partial class SingleChannelViewModel : ObservableObject, IDisposable
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly DispatcherTimer _smoothingTimer;
        private const double MaxBarHeight = 370.0; // 能量条最大高度
        private const double SmoothingFactor = 0.8; // 平滑系数，值越大下降越慢
        private const double MinThreshold = 0.01; // 最小阈值，低于此值认为无声音

        [ObservableProperty]
        private double _channelHeight = 0;

        private double _targetHeight = 0;

        public SingleChannelViewModel()
        {
            // 创建平滑动画定时器
            _smoothingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // 约60FPS
            };
            _smoothingTimer.Tick += SmoothingTimer_Tick;
            _smoothingTimer.Start();
        }

        public void UpdateLevel(float level)
        {
            try
            {
                // 将音频级别转换为能量条高度
                _targetHeight = ConvertLevelToHeight(level);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "更新音频级别失败");
            }
        }

        private double ConvertLevelToHeight(float level)
        {
            // 确保级别在0-1范围内
            level = Math.Max(0, Math.Min(1, level));
            
            // 应用对数缩放以更好地表示音频动态范围
            double logLevel = level > MinThreshold ? Math.Log10(level * 9 + 1) : 0;
            
            // 转换为像素高度
            return logLevel * MaxBarHeight;
        }

        private void SmoothingTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // 使用指数平滑算法实现自然的上升和下降效果
                double difference = _targetHeight - ChannelHeight;
                
                if (Math.Abs(difference) > 0.5) // 避免无限小的变化
                {
                    if (difference > 0)
                    {
                        // 上升时快速响应
                        ChannelHeight += difference * 0.3;
                    }
                    else
                    {
                        // 下降时使用平滑系数
                        ChannelHeight += difference * (1 - SmoothingFactor);
                    }
                }
                else
                {
                    ChannelHeight = _targetHeight;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "平滑动画处理失败");
            }
        }

        public void Dispose()
        {
            try
            {
                _smoothingTimer?.Stop();
                if (_smoothingTimer != null)
                    _smoothingTimer.Tick -= SmoothingTimer_Tick;
                _logger.Info("SingleChannelViewModel已释放资源");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "释放SingleChannelViewModel资源时发生错误");
            }
        }
    }
}
