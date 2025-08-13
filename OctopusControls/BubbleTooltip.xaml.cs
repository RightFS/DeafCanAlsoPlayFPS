using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OctopusControls
{
    public enum TailPosition
    {
        None,
        Top,
        Bottom,
        Left,
        Right
    }
    public partial class BubbleTooltip : UserControl
    {
        public BubbleTooltip()
        {
            InitializeComponent();
            UpdatePathData();
        }

        // width
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(BubbleTooltip),
                new PropertyMetadata(200.0, OnSizeChanged));
        public double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        // height
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(BubbleTooltip),
                new PropertyMetadata(100.0, OnSizeChanged));
        public double Height
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        // Content属性
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(BubbleTooltip));

        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        // ContentTemplate属性
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(BubbleTooltip));

        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        // 尖角位置
        public static readonly DependencyProperty TailPositionProperty =
            DependencyProperty.Register("TailPosition", typeof(TailPosition), typeof(BubbleTooltip),
                new PropertyMetadata(TailPosition.Bottom, OnTailPositionChanged));

        public TailPosition TailPosition
        {
            get => (TailPosition)GetValue(TailPositionProperty);
            set => SetValue(TailPositionProperty, value);
        }

        // 圆角半径
        public static readonly DependencyProperty BubbleCornerRadiusProperty =
            DependencyProperty.Register("BubbleCornerRadius", typeof(double), typeof(BubbleTooltip),
                new PropertyMetadata(8.0, OnSizeChanged));

        public double BubbleCornerRadius
        {
            get => (double)GetValue(BubbleCornerRadiusProperty);
            set => SetValue(BubbleCornerRadiusProperty, value);
        }

        // 尖角大小
        public static readonly DependencyProperty TailSizeProperty =
            DependencyProperty.Register("TailSize", typeof(double), typeof(BubbleTooltip),
                new PropertyMetadata(8.0, OnSizeChanged));

        public double TailSize
        {
            get => (double)GetValue(TailSizeProperty);
            set => SetValue(TailSizeProperty, value);
        }

        // 气泡背景
        public static readonly DependencyProperty BubbleBackgroundProperty =
            DependencyProperty.Register("BubbleBackground", typeof(Brush), typeof(BubbleTooltip),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(45, 45, 45))));

        public Brush BubbleBackground
        {
            get => (Brush)GetValue(BubbleBackgroundProperty);
            set => SetValue(BubbleBackgroundProperty, value);
        }

        // 气泡边框
        public static readonly DependencyProperty BubbleBorderBrushProperty =
            DependencyProperty.Register("BubbleBorderBrush", typeof(Brush), typeof(BubbleTooltip),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(85, 85, 85))));

        public Brush BubbleBorderBrush
        {
            get => (Brush)GetValue(BubbleBorderBrushProperty);
            set => SetValue(BubbleBorderBrushProperty, value);
        }

        // 内边距
        public static readonly DependencyProperty BubblePaddingProperty =
            DependencyProperty.Register("BubblePadding", typeof(Thickness), typeof(BubbleTooltip),
                new PropertyMetadata(new Thickness(12, 8, 12, 8)));

        public Thickness BubblePadding
        {
            get => (Thickness)GetValue(BubblePaddingProperty);
            set => SetValue(BubblePaddingProperty, value);
        }

        // 尖角边框厚度
        public static readonly DependencyProperty TailStrokeThicknessProperty =
            DependencyProperty.Register("TailStrokeThickness", typeof(double), typeof(BubbleTooltip),
                new PropertyMetadata(1.0));

        public double TailStrokeThickness
        {
            get => (double)GetValue(TailStrokeThicknessProperty);
            set => SetValue(TailStrokeThicknessProperty, value);
        }

        // 事件处理方法
        private static void OnTailPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BubbleTooltip tooltip)
            {
                tooltip.UpdatePathData();
            }
        }

        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BubbleTooltip tooltip)
            {
                tooltip.UpdatePathData();
            }
        }

        private void UpdateTailVisibility()
        {
            // 不再需要这个方法，因为只有一个Path
        }

        private void UpdatePathData()
        {
            if (BubblePath == null) return; // 设计时可能为null

            var pathData = GenerateBubblePath(Width, Height, BubbleCornerRadius, TailSize, TailPosition);
            BubblePath.Data = Geometry.Parse(pathData);
        }

        /// <summary>
        /// 动态生成气泡路径
        /// </summary>
        private string GenerateBubblePath(double width, double height, double cornerRadius, double tailSize, TailPosition position)
        {
            // 确保圆角半径不超过宽高的一半
            var maxRadius = System.Math.Min(width, height) / 2;
            var radius = System.Math.Min(cornerRadius, maxRadius);

            switch (position)
            {
                case TailPosition.Right:
                    return GenerateRightTailBubble(width, height, radius, tailSize);
                case TailPosition.Left:
                    return GenerateLeftTailBubble(width, height, radius, tailSize);
                case TailPosition.Top:
                    return GenerateTopTailBubble(width, height, radius, tailSize);
                case TailPosition.Bottom:
                    return GenerateBottomTailBubble(width, height, radius, tailSize);
                default:
                    return GenerateRoundedRectangle(width, height, radius);
            }
        }

        /// <summary>
        /// 生成右侧尖角气泡
        /// </summary>
        private string GenerateRightTailBubble(double width, double height, double radius, double tailSize)
        {
            var centerY = height / 2;
            var tailStart = centerY - tailSize;
            var tailEnd = centerY + tailSize;
            var tailTip = width + tailSize;

            return $"M {radius},0 " +
                   $"L {width - radius},0 " +
                   $"Q {width},0 {width},{radius} " +
                   $"L {width},{tailStart} " +
                   $"L {tailTip},{centerY} " +
                   $"L {width},{tailEnd} " +
                   $"L {width},{height - radius} " +
                   $"Q {width},{height} {width - radius},{height} " +
                   $"L {radius},{height} " +
                   $"Q 0,{height} 0,{height - radius} " +
                   $"L 0,{radius} " +
                   $"Q 0,0 {radius},0 Z";
        }

        /// <summary>
        /// 生成左侧尖角气泡
        /// </summary>
        private string GenerateLeftTailBubble(double width, double height, double radius, double tailSize)
        {
            var centerY = height / 2;
            var tailStart = centerY - tailSize;
            var tailEnd = centerY + tailSize;

            return $"M {radius},0 " +
                   $"L {width - radius},0 " +
                   $"Q {width},0 {width},{radius} " +
                   $"L {width},{height - radius} " +
                   $"Q {width},{height} {width - radius},{height} " +
                   $"L {radius},{height} " +
                   $"Q 0,{height} 0,{height - radius} " +
                   $"L 0,{tailEnd} " +
                   $"L {-tailSize},{centerY} " +
                   $"L 0,{tailStart} " +
                   $"L 0,{radius} " +
                   $"Q 0,0 {radius},0 Z";
        }

        /// <summary>
        /// 生成顶部尖角气泡
        /// </summary>
        private string GenerateTopTailBubble(double width, double height, double radius, double tailSize)
        {
            var centerX = width / 2;
            var tailStart = centerX - tailSize;
            var tailEnd = centerX + tailSize;

            return $"M {radius},{tailSize} " +
                   $"L {tailStart},{tailSize} " +
                   $"L {centerX},0 " +
                   $"L {tailEnd},{tailSize} " +
                   $"L {width - radius},{tailSize} " +
                   $"Q {width},{tailSize} {width},{tailSize + radius} " +
                   $"L {width},{height - radius} " +
                   $"Q {width},{height} {width - radius},{height} " +
                   $"L {radius},{height} " +
                   $"Q 0,{height} 0,{height - radius} " +
                   $"L 0,{tailSize + radius} " +
                   $"Q 0,{tailSize} {radius},{tailSize} Z";
        }

        /// <summary>
        /// 生成底部尖角气泡
        /// </summary>
        private string GenerateBottomTailBubble(double width, double height, double radius, double tailSize)
        {
            var centerX = width / 2;
            var tailStart = centerX - tailSize;
            var tailEnd = centerX + tailSize;
            var bubbleBottom = height - tailSize;

            return $"M {radius},0 " +
                   $"L {width - radius},0 " +
                   $"Q {width},0 {width},{radius} " +
                   $"L {width},{bubbleBottom - radius} " +
                   $"Q {width},{bubbleBottom} {width - radius},{bubbleBottom} " +
                   $"L {tailEnd},{bubbleBottom} " +
                   $"L {centerX},{height} " +
                   $"L {tailStart},{bubbleBottom} " +
                   $"L {radius},{bubbleBottom} " +
                   $"Q 0,{bubbleBottom} 0,{bubbleBottom - radius} " +
                   $"L 0,{radius} " +
                   $"Q 0,0 {radius},0 Z";
        }

        /// <summary>
        /// 生成纯圆角矩形（无尖角）
        /// </summary>
        private string GenerateRoundedRectangle(double width, double height, double radius)
        {
            return $"M {radius},0 " +
                   $"L {width - radius},0 " +
                   $"Q {width},0 {width},{radius} " +
                   $"L {width},{height - radius} " +
                   $"Q {width},{height} {width - radius},{height} " +
                   $"L {radius},{height} " +
                   $"Q 0,{height} 0,{height - radius} " +
                   $"L 0,{radius} " +
                   $"Q 0,0 {radius},0 Z";
        }

        private void UpdateBubbleMargin()
        {
            // 可以根据需要调整内容边距
        }
    }
}