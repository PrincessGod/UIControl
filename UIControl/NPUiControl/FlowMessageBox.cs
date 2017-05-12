using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace NPUiControl
{
    /// <summary>
    ///     流文本消息框
    ///     可用于动态消息提示
    /// </summary>
    [TemplatePart(Name = "PART_FlowDocumentScrollViewer", Type = typeof(FlowDocumentScrollViewer))]
    [TemplatePart(Name = "PART_TitleBlock", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_MessageView", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_ResizeThumb", Type = typeof(FrameworkElement))]
    public class FlowMessageBox : Control
    {
        /// <summary>
        ///     标题属性
        /// </summary>
        public static readonly DependencyProperty TitleProperty;

        /// <summary>
        ///     消息框左边距属性
        /// </summary>
        public static readonly DependencyProperty CanvasLeftProperty;

        /// <summary>
        ///     消息框上边距属性
        /// </summary>
        public static readonly DependencyProperty CanvasTopProperty;

        /// <summary>
        ///     消息框高度属性
        /// </summary>
        public static readonly DependencyProperty PanelHeightProperty;

        /// <summary>
        ///     消息框宽度属性
        /// </summary>
        public static readonly DependencyProperty PanelWidthProperty;

        /// <summary>
        ///     标题栏样式属性
        /// </summary>
        public static readonly DependencyProperty TitleStyleProperty;

        /// <summary>
        ///     缩放Thumb样式属性
        /// </summary>
        public static readonly DependencyProperty ThumbStyleProperty;

        /// <summary>
        ///     当新消息产生是否开启动画强调属性 （默认开启）
        /// </summary>
        public static readonly DependencyProperty IsAnimationEnableProperty;

        /// <summary>
        ///     动画强调颜色属性
        /// </summary>
        public static readonly DependencyProperty AccentColorProperty;

        /// <summary>
        ///     动画强调字体增率属性
        /// </summary>
        public static readonly DependencyProperty AccentFontSizeRatioProperty;

        /// <summary>
        ///     圆角半径属性
        /// </summary>
        public static readonly DependencyProperty CornerRadiuProperty;

        /// <summary>
        ///     新增信息时触发的路由事件
        /// </summary>
        public static readonly RoutedEvent MessageAddEvent;

        private bool _isDragDropInEffect;
        private bool _isResizeEffect;
        private FrameworkElement _messageView;
        private Point _pos;
        private ScrollViewer _scrollViewer;

        static FlowMessageBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlowMessageBox),
                new FrameworkPropertyMetadata(typeof(FlowMessageBox)));

            TitleProperty = DependencyProperty.Register(
                "Title", typeof(string), typeof(FlowMessageBox), new PropertyMetadata("Title"));
            TitleStyleProperty = DependencyProperty.Register(
                "TitleStyle", typeof(object), typeof(FlowMessageBox), new PropertyMetadata(default(object)));
            ThumbStyleProperty = DependencyProperty.Register(
                "ThumbStyle", typeof(object), typeof(FlowMessageBox), new PropertyMetadata(default(object)));
            CanvasLeftProperty = DependencyProperty.Register(
                "CanvasLeft", typeof(double), typeof(FlowMessageBox), new PropertyMetadata((double) 0));
            CanvasTopProperty = DependencyProperty.Register(
                "CanvasTop", typeof(double), typeof(FlowMessageBox), new PropertyMetadata((double) 0));
            PanelHeightProperty = DependencyProperty.Register(
                "PanelHeight", typeof(double), typeof(FlowMessageBox), new PropertyMetadata((double) 200));
            PanelWidthProperty = DependencyProperty.Register(
                "PanelWidth", typeof(double), typeof(FlowMessageBox), new PropertyMetadata((double) 200));
            IsAnimationEnableProperty = DependencyProperty.Register(
                "IsAnimationEnable", typeof(bool), typeof(FlowMessageBox), new PropertyMetadata(true));
            AccentColorProperty = DependencyProperty.Register(
                "AccentColor", typeof(Color), typeof(FlowMessageBox), new PropertyMetadata(Colors.Aqua));
            AccentFontSizeRatioProperty = DependencyProperty.Register(
                "AccentFontSizeRatio", typeof(double), typeof(FlowMessageBox), new PropertyMetadata((double) 0));
            CornerRadiuProperty = DependencyProperty.Register(
                "CornerRadiu", typeof(CornerRadius), typeof(FlowMessageBox),
                new PropertyMetadata(default(CornerRadius)));

            MessageAddEvent = EventManager.RegisterRoutedEvent("MessageAdded", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<string>), typeof(FlowMessageBox));
        }

        /// <summary>
        ///     流式消息框
        /// </summary>
        public FlowMessageBox()
        {
            Paragraph = new Paragraph();
            Document = new FlowDocument(Paragraph);
            Document.PagePadding = new Thickness(0, 0, 0, 0);
        }

        /// <summary>
        ///     圆角半径
        /// </summary>
        public CornerRadius CornerRadiu
        {
            get { return (CornerRadius) GetValue(CornerRadiuProperty); }
            set { SetValue(CornerRadiuProperty, value); }
        }

        /// <summary>
        ///     缩放Thumb样式
        /// </summary>
        public object ThumbStyle
        {
            get { return GetValue(ThumbStyleProperty); }
            set { SetValue(ThumbStyleProperty, value); }
        }

        /// <summary>
        ///     标题栏样式
        /// </summary>
        public object TitleStyle
        {
            get { return GetValue(TitleStyleProperty); }
            set { SetValue(TitleStyleProperty, value); }
        }

        /// <summary>
        ///     动画强调字体大小增幅
        /// </summary>
        public double AccentFontSizeRatio
        {
            get { return (double) GetValue(AccentFontSizeRatioProperty); }
            set { SetValue(AccentFontSizeRatioProperty, value); }
        }

        /// <summary>
        ///     动画强调字体颜色
        /// </summary>
        public Color AccentColor
        {
            get { return (Color) GetValue(AccentColorProperty); }
            set { SetValue(AccentColorProperty, value); }
        }

        /// <summary>
        ///     是否开始新消息动画强调
        /// </summary>
        public bool IsAnimationEnable
        {
            get { return (bool) GetValue(IsAnimationEnableProperty); }
            set { SetValue(IsAnimationEnableProperty, value); }
        }

        /// <summary>
        ///     消息框宽度
        /// </summary>
        public double PanelWidth
        {
            get { return (double) GetValue(PanelWidthProperty); }
            set { SetValue(PanelWidthProperty, value); }
        }

        /// <summary>
        ///     消息框高度
        /// </summary>
        public double PanelHeight
        {
            get { return (double) GetValue(PanelHeightProperty); }
            set { SetValue(PanelHeightProperty, value); }
        }

        /// <summary>
        ///     消息框上边距
        /// </summary>
        public double CanvasTop
        {
            get { return (double) GetValue(CanvasTopProperty); }
            set { SetValue(CanvasTopProperty, value); }
        }

        /// <summary>
        ///     消息框左边距
        /// </summary>
        public double CanvasLeft
        {
            get { return (double) GetValue(CanvasLeftProperty); }
            set { SetValue(CanvasLeftProperty, value); }
        }

        /// <summary>
        ///     消息框标题
        /// </summary>
        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        ///     FlowDocumentScrollViewer的ScrollViewer
        /// </summary>
        public ScrollViewer ScrollViewer
        {
            get
            {
                if (_scrollViewer == null)
                {
                    if (FlowDocumentScrollViewer != null)
                    {
                        DependencyObject obj = FlowDocumentScrollViewer;
                        do
                        {
                            if (VisualTreeHelper.GetChildrenCount(obj) > 0)
                                obj = VisualTreeHelper.GetChild(obj as Visual, 0);
                            else
                                return null;
                        } while (!(obj is ScrollViewer));
                        _scrollViewer = obj as ScrollViewer;
                    }
                }
                return _scrollViewer;
            }
        }

        /// <summary>
        ///     FlowDocumentScrollViewer控件
        /// </summary>
        public FlowDocumentScrollViewer FlowDocumentScrollViewer { get; private set; }

        /// <summary>
        ///     FlowDocument对象
        /// </summary>
        public FlowDocument Document { get; private set; }

        /// <summary>
        ///     Paragraph对象
        /// </summary>
        public Paragraph Paragraph { get; private set; }

        /// <summary>
        ///     上一个文字画刷
        /// </summary>
        public SolidColorBrush LastBrush { get; private set; }

        /// <summary>
        ///     上一个文字颜色
        /// </summary>
        public Color LastColor { get; private set; }

        /// <summary>
        ///     路由事件消息增加
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> MessageAdded
        {
            add { AddHandler(MessageAddEvent, value); }
            remove { RemoveHandler(MessageAddEvent, value); }
        }

        /// <summary>
        ///     添加消息
        /// </summary>
        /// <param name="message">消息内容</param>
        public void AddMessage(string message)
        {
            AddMessage(message, Colors.Black);
        }

        /// <summary>
        ///     添加消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="color">文字颜色</param>
        public void AddMessage(string message, Color color)
        {
            AddMessage(message, color, FontSize);
        }

        /// <summary>
        ///     添加消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="color">文字颜色</param>
        /// <param name="fontSize">字体大小</param>
        public void AddMessage(string message, Color color, double fontSize)
        {
            AddMessage(message, color, fontSize, FontFamily);
        }

        /// <summary>
        ///     添加消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="color">文字颜色</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="fontFamily">字体</param>
        public void AddMessage(string message, Color color, double fontSize, FontFamily fontFamily)
        {
            string lastMessage = null;
            if (Paragraph.Inlines.Count != 0)
            {
                lastMessage = ((Run) Paragraph.Inlines.LastInline).Text;
                Paragraph.Inlines.Add(new LineBreak());
            }

            var run = new Run(message);
            if (!IsAnimationEnable)
            {
                if (LastBrush == null || LastColor != color)
                {
                    LastBrush = new SolidColorBrush(color);
                    LastColor = color;
                }
                run.Foreground = LastBrush;
            }
            else
            {
                run.Foreground = new SolidColorBrush(color);
            }
            run.FontSize = fontSize;
            run.FontFamily = fontFamily;
            Paragraph.Inlines.Add(run);
            ScrollViewer.ScrollToEnd();

            if (IsAnimationEnable)
            {
                var brush = run.Foreground;
                var fontColorAnimation = new ColorAnimation();
                fontColorAnimation.From = AccentColor;
                fontColorAnimation.Duration = TimeSpan.FromSeconds(2);
                brush.BeginAnimation(SolidColorBrush.ColorProperty, fontColorAnimation);

                var fontSizeAnimation = new DoubleAnimation();
                fontSizeAnimation.From = fontSize + AccentFontSizeRatio;
                fontSizeAnimation.Duration = TimeSpan.FromSeconds(2);
                run.BeginAnimation(TextElement.FontSizeProperty, fontSizeAnimation);
            }

            var args =
                new RoutedPropertyChangedEventArgs<string>(lastMessage, message);
            args.RoutedEvent = MessageAddEvent;
            RaiseEvent(args);
        }

        /// <summary>
        ///     清空消息
        /// </summary>
        public void ClearMessage()
        {
            Paragraph.Inlines.Clear();
        }

        /// <summary>
        ///     应用模板时触发
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            FlowDocumentScrollViewer = GetTemplateChild("PART_FlowDocumentScrollViewer") as FlowDocumentScrollViewer;
            if (FlowDocumentScrollViewer != null)
                FlowDocumentScrollViewer.Document = Document;

            var titleBlock = GetTemplateChild("PART_TitleBlock") as FrameworkElement;
            if (titleBlock != null)
            {
                var binding = new Binding("Title");
                binding.Source = this;
                binding.Mode = BindingMode.OneWay;
                titleBlock.SetBinding(TagProperty, binding);
            }

            _messageView = GetTemplateChild("PART_MessageView") as FrameworkElement;
            if (_messageView != null)
            {
                var bindingTop = new Binding("CanvasTop");
                bindingTop.Source = this;
                bindingTop.Mode = BindingMode.TwoWay;
                _messageView.SetBinding(Canvas.TopProperty, bindingTop);

                var bindingLeft = new Binding("CanvasLeft");
                bindingLeft.Source = this;
                bindingLeft.Mode = BindingMode.TwoWay;
                _messageView.SetBinding(Canvas.LeftProperty, bindingLeft);

                var bindingWidth = new Binding("PanelWidth");
                bindingWidth.Source = this;
                bindingWidth.Mode = BindingMode.TwoWay;
                _messageView.SetBinding(WidthProperty, bindingWidth);

                var bindingHeight = new Binding("PanelHeight");
                bindingHeight.Source = this;
                bindingHeight.Mode = BindingMode.TwoWay;
                _messageView.SetBinding(HeightProperty, bindingHeight);

                var thumb = GetTemplateChild("PART_ResizeThumb") as FrameworkElement;
                if (thumb != null)
                {
                    thumb.MouseLeftButtonUp += Thumb_LeftButtonUp;
                    thumb.MouseLeftButtonDown += Thumb_LeftButtonDown;
                    thumb.MouseMove += Thumb_MouseMove;
                }

                var drag = GetTemplateChild("PART_TitleBlock") as FrameworkElement;
                if (drag != null)
                {
                    drag.MouseMove += Drag_MouseMove;
                    drag.MouseLeftButtonDown += Drag_MouseLeftButtonDown;
                    drag.MouseLeftButtonUp += Drag_MouseLeftButtonUp;
                }
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
                var xPos = e.GetPosition(null).X - _pos.X + (double) _messageView.GetValue(Canvas.LeftProperty);
                var yPos = e.GetPosition(null).Y - _pos.Y + (double) _messageView.GetValue(Canvas.TopProperty);
                xPos = xPos + _messageView.ActualWidth > ActualWidth
                    ? ActualWidth - _messageView.ActualWidth
                    : xPos;
                yPos = yPos + _messageView.ActualHeight > ActualHeight
                    ? ActualHeight - _messageView.ActualHeight
                    : yPos;
                xPos = xPos < 0 ? 0 : xPos;
                yPos = yPos < 0 ? 0 : yPos;

                _messageView.SetValue(Canvas.LeftProperty, xPos);
                _messageView.SetValue(Canvas.TopProperty, yPos);
                _pos = e.GetPosition(null);
            }
        }

        private void Thumb_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isResizeEffect)
            {
                var xAdjust = _messageView.Width + e.GetPosition(null).X - _pos.X;
                var yAdjust = _messageView.Height + e.GetPosition(null).Y - _pos.Y;

                xAdjust = _messageView.ActualWidth + xAdjust > _messageView.MinWidth ? xAdjust : _messageView.MinWidth;
                yAdjust = _messageView.ActualHeight + yAdjust > _messageView.MinHeight
                    ? yAdjust
                    : _messageView.MinHeight;

                xAdjust = xAdjust > ActualWidth ? ActualWidth - 1 : xAdjust;
                yAdjust = yAdjust > ActualHeight ? ActualHeight - 1 : yAdjust;

                _messageView.Width = xAdjust;
                _messageView.Height = yAdjust;
                _pos = e.GetPosition(null);
            }
        }

        private void Thumb_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var ele = sender as FrameworkElement;
            _isResizeEffect = true;
            _pos = e.GetPosition(null);
            if (ele != null)
            {
                ele.CaptureMouse();
                ele.Cursor = Cursors.SizeNWSE;
            }
        }

        private void Thumb_LeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isResizeEffect)
            {
                var ele = sender as FrameworkElement;
                _isResizeEffect = false;
                if (ele != null)
                    ele.ReleaseMouseCapture();
            }
        }
    }
}