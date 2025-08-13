using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OctopusControls
{
    public class StyledButton : Button
    {
        #region Dependency Properties

        // Normal state properties
        public static readonly DependencyProperty NormalBackgroundProperty =
            DependencyProperty.Register("NormalBackground", typeof(Brush), typeof(StyledButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(15, 255, 255, 255))));

        public static readonly DependencyProperty NormalForegroundProperty =
            DependencyProperty.Register("NormalForeground", typeof(Brush), typeof(StyledButton),
                new PropertyMetadata(Brushes.White));

        // Hover state properties
        public static readonly DependencyProperty HoverBackgroundProperty =
            DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(StyledButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(33, 255, 255, 255))));

        public static readonly DependencyProperty HoverForegroundProperty =
            DependencyProperty.Register("HoverForeground", typeof(Brush), typeof(StyledButton),
                new PropertyMetadata(Brushes.White));

        // Pressed state properties
        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(StyledButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(50, 255, 255, 255))));

        public static readonly DependencyProperty PressedForegroundProperty =
            DependencyProperty.Register("PressedForeground", typeof(Brush), typeof(StyledButton),
                new PropertyMetadata(Brushes.White));

        // Border properties
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(StyledButton),
                new PropertyMetadata(new CornerRadius(2)));

        public static readonly new DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(StyledButton),
                new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty NormalBorderBrushProperty =
            DependencyProperty.Register("NormalBorderBrush", typeof(Brush), typeof(StyledButton),
                new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty HoverBorderBrushProperty =
            DependencyProperty.Register("HoverBorderBrush", typeof(Brush), typeof(StyledButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(33, 255, 255, 255))));

        public static readonly DependencyProperty PressedBorderBrushProperty =
            DependencyProperty.Register("PressedBorderBrush", typeof(Brush), typeof(StyledButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(51, 255, 255, 255))));

        #endregion

        #region Properties

        // Normal state
        public Brush NormalBackground
        {
            get { return (Brush)GetValue(NormalBackgroundProperty); }
            set { SetValue(NormalBackgroundProperty, value); }
        }

        public Brush NormalForeground
        {
            get { return (Brush)GetValue(NormalForegroundProperty); }
            set { SetValue(NormalForegroundProperty, value); }
        }

        // Hover state
        public Brush HoverBackground
        {
            get { return (Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        public Brush HoverForeground
        {
            get { return (Brush)GetValue(HoverForegroundProperty); }
            set { SetValue(HoverForegroundProperty, value); }
        }

        // Pressed state
        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public Brush PressedForeground
        {
            get { return (Brush)GetValue(PressedForegroundProperty); }
            set { SetValue(PressedForegroundProperty, value); }
        }

        // Border properties
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public new Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public Brush NormalBorderBrush
        {
            get { return (Brush)GetValue(NormalBorderBrushProperty); }
            set { SetValue(NormalBorderBrushProperty, value); }
        }

        public Brush HoverBorderBrush
        {
            get { return (Brush)GetValue(HoverBorderBrushProperty); }
            set { SetValue(HoverBorderBrushProperty, value); }
        }

        public Brush PressedBorderBrush
        {
            get { return (Brush)GetValue(PressedBorderBrushProperty); }
            set { SetValue(PressedBorderBrushProperty, value); }
        }

        #endregion

        static StyledButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StyledButton),
                new FrameworkPropertyMetadata(typeof(StyledButton)));
        }

        public StyledButton()
        {
            // Default constructor
        }
    }
}
