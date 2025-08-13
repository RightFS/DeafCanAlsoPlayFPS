using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

//namespace ImageButtom
//{
//    /// <summary>
//    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
//    ///
//    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
//    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
//    /// to be used:
//    ///
//    ///     xmlns:MyNamespace="clr-namespace:ImageButtom"
//    ///
//    ///
//    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
//    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
//    /// to be used:
//    ///
//    ///     xmlns:MyNamespace="clr-namespace:ImageButtom;assembly=ImageButtom"
//    ///
//    /// You will also need to add a project reference from the project where the XAML file lives
//    /// to this project and Rebuild to avoid compilation errors:
//    ///
//    ///     Right click on the target project in the Solution Explorer and
//    ///     "Add Reference"->"Projects"->[Select this project]
//    ///
//    ///
//    /// Step 2)
//    /// Go ahead and use your control in the XAML file.
//    ///
//    ///     <MyNamespace:CustomControl1/>
//    ///
//    /// </summary>
//    public class CustomControl1 : Control
//    {
//        static CustomControl1()
//        {
//            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomControl1), new FrameworkPropertyMetadata(typeof(CustomControl1)));
//        }
//    }
//}

namespace OctopusControls
{
    public class ImageButton : Button
    {
        // 定义一个依赖属性，用于存储图片文件名前缀
        public static readonly DependencyProperty ImagePrefixProperty =
            DependencyProperty.Register(nameof(ImagePrefix), typeof(string), typeof(ImageButton),
                new PropertyMetadata(string.Empty, OnImagePrefixChanged));

        public string ImagePrefix
        {
            get => (string)GetValue(ImagePrefixProperty);
            set => SetValue(ImagePrefixProperty, value);
        }

        private static void OnImagePrefixChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageButton button)
            {
                // 更新按钮的图片
                button.UpdateImage();
            }
        }

        private void UpdateImage()
        {
            // 根据前缀和状态后缀拼接出图片路径
            string normalImagePath = $"{ImagePrefix}0.png";
            string hoverImagePath = $"{ImagePrefix}1.png";
            string clickedImagePath = $"{ImagePrefix}2.png";

            // 设置样式
            this.Template = CreateControlTemplate(normalImagePath, hoverImagePath, clickedImagePath);
        }

        private ControlTemplate CreateControlTemplate(string normalImagePath, string hoverImagePath, string clickedImagePath)
        {
            // 使用ControlTemplate定义按钮的样式
            var template = new ControlTemplate(typeof(Button));

            // 创建一个Grid来包含Image和ContentPresenter
            var gridFactory = new FrameworkElementFactory(typeof(Grid));

            // 设置 Normal 状态的图片
            var imageFactory = new FrameworkElementFactory(typeof(Image), "PART_Image");
            imageFactory.SetValue(Image.SourceProperty, new BitmapImage(new Uri(normalImagePath, UriKind.RelativeOrAbsolute)));
            imageFactory.SetValue(NameProperty, "PART_Image");

            // 创建ContentPresenter来显示按钮的内容
            var contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenterFactory.SetValue(ContentPresenter.ContentProperty, new TemplateBindingExtension(ContentProperty));
            contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            // 将Image和ContentPresenter添加到Grid中
            gridFactory.AppendChild(imageFactory);
            gridFactory.AppendChild(contentPresenterFactory);

            template.VisualTree = gridFactory;

            // 定义鼠标悬停和按下时的触发器
            template.Triggers.Add(new Trigger
            {
                Property = IsMouseOverProperty,
                Value = true,
                Setters = { new Setter(Image.SourceProperty, new BitmapImage(new Uri(hoverImagePath, UriKind.RelativeOrAbsolute)), "PART_Image") }
            });

            template.Triggers.Add(new Trigger
            {
                Property = IsPressedProperty,
                Value = true,
                Setters = { new Setter(Image.SourceProperty, new BitmapImage(new Uri(clickedImagePath, UriKind.RelativeOrAbsolute)), "PART_Image") }
            });

            return template;
        }
    }
}
