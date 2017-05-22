using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace NPUiControl
{
    /// <summary>
    ///     可拖动的Control
    ///     有标题栏和关闭按钮但不支持自定义
    /// </summary>
    [TemplatePart(Name = "PART_Moveable", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_Content", Type = typeof(FrameworkElement))]
    public class MoveableContent : Control
    {
        /// <summary>
        ///     标题栏字符串格式属性
        /// </summary>
        public static readonly DependencyProperty TitleStringFormatProperty;

        /// <summary>
        ///     标题栏数据模板选择器属性
        /// </summary>
        public static readonly DependencyProperty TitleDataTemplateSelectorProperty;

        /// <summary>
        ///     标题栏数据模板属性
        /// </summary>
        public static readonly DependencyProperty TitleDataTemplateProperty;

        /// <summary>
        ///     内容字符串格式属性
        /// </summary>
        public static readonly DependencyProperty ContentStringFormatProperty;

        /// <summary>
        ///     内容数据模板选择器属性
        /// </summary>
        public static readonly DependencyProperty ContentDataTemplateSelectorProperty;

        /// <summary>
        ///     内容数据模板属性
        /// </summary>
        public static readonly DependencyProperty ContentDataTemplateProperty;

        /// <summary>
        ///     隐藏控件路由命令
        /// </summary>
        public static RoutedCommand HideContentCommand = new RoutedCommand();

        /// <summary>
        ///     标题栏Content属性
        /// </summary>
        public static readonly DependencyProperty TitleContentProperty;

        /// <summary>
        ///     标题属性
        /// </summary>
        public static readonly DependencyProperty TitleProperty;

        /// <summary>
        ///     上边距
        /// </summary>
        public static readonly DependencyProperty CanvasTopProperty;

        /// <summary>
        ///     左边距
        /// </summary>
        public static readonly DependencyProperty CanvasLeftProperty;

        /// <summary>
        ///     圆角属性
        /// </summary>
        public static readonly DependencyProperty CornerRadiuProperty;

        /// <summary>
        ///     内容Style属性
        /// </summary>
        public static readonly DependencyProperty ContentProperty;

        private FrameworkElement _contentElement;
        private bool _isDragDropInEffect;
        private Point _pos;

        static MoveableContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MoveableContent),
                new FrameworkPropertyMetadata(typeof(MoveableContent)));

            TitleProperty = DependencyProperty.Register(
                "Title", typeof(string), typeof(MoveableContent), new PropertyMetadata(default(string)));
            CanvasTopProperty = DependencyProperty.Register(
                "CanvasTop", typeof(double), typeof(MoveableContent), new PropertyMetadata((double) 0));
            CanvasLeftProperty = DependencyProperty.Register(
                "CanvasLeft", typeof(double), typeof(MoveableContent), new PropertyMetadata((double) 0));
            CornerRadiuProperty = DependencyProperty.Register(
                "CornerRadiu", typeof(CornerRadius), typeof(MoveableContent),
                new PropertyMetadata(default(CornerRadius)));
            ContentProperty = DependencyProperty.Register(
                "Content", typeof(object), typeof(MoveableContent), new PropertyMetadata(default(object)));
            TitleContentProperty = DependencyProperty.Register(
                "TitleContent", typeof(object), typeof(MoveableContent), new PropertyMetadata(default(object)));
            TitleStringFormatProperty = DependencyProperty.Register(
                "TitleStringFormat", typeof(string), typeof(MoveableContent), new PropertyMetadata(default(string)));
            TitleDataTemplateSelectorProperty = DependencyProperty.Register(
                "TitleDataTemplateSelector", typeof(DataTemplateSelector), typeof(MoveableContent),
                new PropertyMetadata(default(DataTemplateSelector)));
            TitleDataTemplateProperty = DependencyProperty.Register(
                "TitleDataTemplate", typeof(DataTemplate), typeof(MoveableContent),
                new PropertyMetadata(default(DataTemplate)));
            ContentStringFormatProperty = DependencyProperty.Register(
                "ContentStringFormat", typeof(string), typeof(MoveableContent), new PropertyMetadata(default(string)));
            ContentDataTemplateSelectorProperty = DependencyProperty.Register(
                "ContentDataTemplateSelector", typeof(DataTemplateSelector), typeof(MoveableContent),
                new PropertyMetadata(default(DataTemplateSelector)));
            ContentDataTemplateProperty = DependencyProperty.Register(
                "ContentDataTemplate", typeof(DataTemplate), typeof(MoveableContent),
                new PropertyMetadata(default(DataTemplate)));
        }

        /// <summary>
        ///     拥有Title和Content两部分组成并能在一定区域内拖动的控件
        /// </summary>
        public MoveableContent()
        {
            CommandBindings.Add(new CommandBinding(HideContentCommand, HideContent));
        }

        /// <summary>
        ///     标题栏字符串内容
        /// </summary>
        public string TitleStringFormat
        {
            get { return (string) GetValue(TitleStringFormatProperty); }
            set { SetValue(TitleStringFormatProperty, value); }
        }

        /// <summary>
        ///     标题栏数据模板选择器
        /// </summary>
        public DataTemplateSelector TitleDataTemplateSelector
        {
            get { return (DataTemplateSelector) GetValue(TitleDataTemplateSelectorProperty); }
            set { SetValue(TitleDataTemplateSelectorProperty, value); }
        }

        /// <summary>
        ///     标题栏数据模板
        /// </summary>
        public DataTemplate TitleDataTemplate
        {
            get { return (DataTemplate) GetValue(TitleDataTemplateProperty); }
            set { SetValue(TitleDataTemplateProperty, value); }
        }

        /// <summary>
        ///     内容字符串格式
        /// </summary>
        public string ContentStringFormat
        {
            get { return (string) GetValue(ContentStringFormatProperty); }
            set { SetValue(ContentStringFormatProperty, value); }
        }

        /// <summary>
        ///     内容数据模板选择器
        /// </summary>
        public DataTemplateSelector ContentDataTemplateSelector
        {
            get { return (DataTemplateSelector) GetValue(ContentDataTemplateSelectorProperty); }
            set { SetValue(ContentDataTemplateSelectorProperty, value); }
        }

        /// <summary>
        ///     内容数据模板
        /// </summary>
        public DataTemplate ContentDataTemplate
        {
            get { return (DataTemplate) GetValue(ContentDataTemplateProperty); }
            set { SetValue(ContentDataTemplateProperty, value); }
        }

        /// <summary>
        ///     标题栏Content
        /// </summary>
        public object TitleContent
        {
            get { return GetValue(TitleContentProperty); }
            set { SetValue(TitleContentProperty, value); }
        }


        /// <summary>
        ///     标题
        /// </summary>
        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        ///     当前Content的ActualWidth
        /// </summary>
        public double ContentActualWidth
        {
            get
            {
                if (_contentElement != null)
                    return _contentElement.ActualWidth;
                return 0;
            }
        }

        /// <summary>
        ///     当前Content的ActualHeight
        /// </summary>
        public double ContentActualHeight
        {
            get
            {
                if (_contentElement != null)
                    return _contentElement.ActualHeight;
                return 0;
            }
        }

        /// <summary>
        ///     上边距
        /// </summary>
        public double CanvasTop
        {
            get { return (double) GetValue(CanvasTopProperty); }
            set { SetValue(CanvasTopProperty, value); }
        }

        /// <summary>
        ///     左边距
        /// </summary>
        public double CanvasLeft
        {
            get { return (double) GetValue(CanvasLeftProperty); }
            set { SetValue(CanvasLeftProperty, value); }
        }

        /// <summary>
        ///     圆角值
        /// </summary>
        public CornerRadius CornerRadiu
        {
            get { return (CornerRadius) GetValue(CornerRadiuProperty); }
            set { SetValue(CornerRadiuProperty, value); }
        }

        /// <summary>
        ///     内容Style
        /// </summary>
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        private void HideContent(object sender, ExecutedRoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        /// <summary>
        ///     应用模板时触发
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var moveable = GetTemplateChild("PART_Moveable") as ContentPresenter;
            if (moveable != null)
            {
                moveable.MouseMove += Drag_MouseMove;
                moveable.MouseLeftButtonDown += Drag_MouseLeftButtonDown;
                moveable.MouseLeftButtonUp += Drag_MouseLeftButtonUp;

                var content = moveable.Content as FrameworkElement;
                if (content != null)
                {
                    var textbloct = UiHelper.FindChild<TextBlock>(content, "PART_Title");
                    if (textbloct != null)
                    {
                        var binding = new Binding("Title");
                        binding.Source = this;
                        binding.Mode = BindingMode.OneWay;
                        textbloct.SetBinding(TextBlock.TextProperty, binding);
                    }
                }
            }

            _contentElement = GetTemplateChild("PART_Content") as FrameworkElement;
            if (_contentElement != null)
            {
                var bindingTop = new Binding("CanvasTop");
                bindingTop.Source = this;
                bindingTop.Mode = BindingMode.TwoWay;
                _contentElement.SetBinding(Canvas.TopProperty, bindingTop);

                var bindingLeft = new Binding("CanvasLeft");
                bindingLeft.Source = this;
                bindingLeft.Mode = BindingMode.TwoWay;
                _contentElement.SetBinding(Canvas.LeftProperty, bindingLeft);
            }
        }


        private void Drag_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragDropInEffect)
            {
                var ele = sender as FrameworkElement;
                _isDragDropInEffect = false;
                if (ele != null)
                    ele.ReleaseMouseCapture();
            }
        }

        private void Drag_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var ele = sender as FrameworkElement;
            _isDragDropInEffect = true;
            _pos = e.GetPosition(null);
            if (ele != null)
            {
                ele.CaptureMouse();
                ele.Cursor = Cursors.Hand;
            }
        }

        private void Drag_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragDropInEffect)
            {
                var xPos = e.GetPosition(null).X - _pos.X + (double) _contentElement.GetValue(Canvas.LeftProperty);
                var yPos = e.GetPosition(null).Y - _pos.Y + (double) _contentElement.GetValue(Canvas.TopProperty);
                xPos = xPos + _contentElement.ActualWidth > ActualWidth
                    ? ActualWidth - _contentElement.ActualWidth
                    : xPos;
                yPos = yPos + _contentElement.ActualHeight > ActualHeight
                    ? ActualHeight - _contentElement.ActualHeight
                    : yPos;
                xPos = xPos < 0 ? 0 : xPos;
                yPos = yPos < 0 ? 0 : yPos;

                _contentElement.SetValue(Canvas.LeftProperty, xPos);
                _contentElement.SetValue(Canvas.TopProperty, yPos);
                _pos = e.GetPosition(null);
            }
        }
    }
}