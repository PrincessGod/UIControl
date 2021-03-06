﻿using System;
using System.Linq;
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
    [TemplatePart(Name = "PART_Canvas", Type = typeof(Panel))]
    [TemplateVisualState(Name = "Normal", GroupName = "ViewStates")]
    [TemplateVisualState(Name = "Active", GroupName = "ViewStates")]
    public class FlowMessageBox : Control
    {
        /// <summary>
        ///     消息框底边距属性 底边距属性优先级高于顶边距 最好设置最小宽高<see cref="PanelMinHeight" /><see cref="PanelMinHeight" />
        /// </summary>
        public static readonly DependencyProperty CanvasBottomProperty;

        /// <summary>
        ///     消息框右边距属性 右边距属性优先级高于左边距 最好设置最小宽高<see cref="PanelMinHeight" /><see cref="PanelMinHeight" />
        /// </summary>
        public static readonly DependencyProperty CanvasRightProperty;

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
        ///     是否自动隐藏边框和标题栏属性
        /// </summary>
        public static readonly DependencyProperty IsAutoHideTitleAndBorderProperty;

        /// <summary>
        ///     消息框外边距属性
        /// </summary>
        public static readonly DependencyProperty PanelMarginProperty;

        /// <summary>
        ///     消息框最大高度属性
        /// </summary>
        public static readonly DependencyProperty PanelMaxHeightProperty;

        /// <summary>
        ///     消息框最大宽度属性
        /// </summary>
        public static readonly DependencyProperty PanelMaxWidthProperty;

        /// <summary>
        ///     消息框最小高度属性
        /// </summary>
        public static readonly DependencyProperty PanelMinHeightProperty;

        /// <summary>
        ///     消息框最小宽度属性
        /// </summary>
        public static readonly DependencyProperty PanelMinWidthProperty;

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
                "PanelHeight", typeof(double), typeof(FlowMessageBox), new PropertyMetadata(double.NaN));
            PanelWidthProperty = DependencyProperty.Register(
                "PanelWidth", typeof(double), typeof(FlowMessageBox), new PropertyMetadata(double.NaN));
            IsAnimationEnableProperty = DependencyProperty.Register(
                "IsAnimationEnable", typeof(bool), typeof(FlowMessageBox), new PropertyMetadata(true));
            AccentColorProperty = DependencyProperty.Register(
                "AccentColor", typeof(Color), typeof(FlowMessageBox), new PropertyMetadata(Colors.Aqua));
            AccentFontSizeRatioProperty = DependencyProperty.Register(
                "AccentFontSizeRatio", typeof(double), typeof(FlowMessageBox), new PropertyMetadata((double) 4));
            CornerRadiuProperty = DependencyProperty.Register(
                "CornerRadiu", typeof(CornerRadius), typeof(FlowMessageBox),
                new PropertyMetadata(default(CornerRadius)));
            IsAutoHideTitleAndBorderProperty = DependencyProperty.Register(
                "IsAutoHideTitleAndBorder", typeof(bool), typeof(FlowMessageBox), new PropertyMetadata(default(bool)));
            PanelMarginProperty = DependencyProperty.Register(
                "PanelMargin", typeof(Thickness), typeof(FlowMessageBox),
                new PropertyMetadata(new Thickness(-1, -1, -1, -1)));
            PanelMaxHeightProperty = DependencyProperty.Register(
                "PanelMaxHeight", typeof(double), typeof(FlowMessageBox), new PropertyMetadata(double.NaN));
            PanelMaxWidthProperty = DependencyProperty.Register(
                "PanelMaxWidth", typeof(double), typeof(FlowMessageBox), new PropertyMetadata(double.NaN));
            PanelMinHeightProperty = DependencyProperty.Register(
                "PanelMinHeight", typeof(double), typeof(FlowMessageBox), new PropertyMetadata(double.NaN));
            PanelMinWidthProperty = DependencyProperty.Register(
                "PanelMinWidth", typeof(double), typeof(FlowMessageBox), new PropertyMetadata(double.NaN));
            CanvasBottomProperty = DependencyProperty.Register(
                "CanvasBottom", typeof(double), typeof(FlowMessageBox), new PropertyMetadata(double.NaN));
            CanvasRightProperty = DependencyProperty.Register(
                "CanvasRight", typeof(double), typeof(FlowMessageBox), new PropertyMetadata(double.NaN));

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
        ///     消息框右边距
        /// </summary>
        public double CanvasRight
        {
            get { return (double) GetValue(CanvasRightProperty); }
            set { SetValue(CanvasRightProperty, value); }
        }

        /// <summary>
        ///     消息框底边距
        /// </summary>
        public double CanvasBottom
        {
            get { return (double) GetValue(CanvasBottomProperty); }
            set { SetValue(CanvasBottomProperty, value); }
        }

        /// <summary>
        ///     消息框最小宽度
        /// </summary>
        public double PanelMinWidth
        {
            get { return (double) GetValue(PanelMinWidthProperty); }
            set { SetValue(PanelMinWidthProperty, value); }
        }

        /// <summary>
        ///     消息框最小高度
        /// </summary>
        public double PanelMinHeight
        {
            get { return (double) GetValue(PanelMinHeightProperty); }
            set { SetValue(PanelMinHeightProperty, value); }
        }

        /// <summary>
        ///     消息框最大宽度
        /// </summary>
        public double PanelMaxWidth
        {
            get { return (double) GetValue(PanelMaxWidthProperty); }
            set { SetValue(PanelMaxWidthProperty, value); }
        }

        /// <summary>
        ///     消息框最大高度
        /// </summary>
        public double PanelMaxHeight
        {
            get { return (double) GetValue(PanelMaxHeightProperty); }
            set { SetValue(PanelMaxHeightProperty, value); }
        }

        /// <summary>
        ///     消息框外边距
        /// </summary>
        public Thickness PanelMargin
        {
            get { return (Thickness) GetValue(PanelMarginProperty); }
            set { SetValue(PanelMarginProperty, value); }
        }

        /// <summary>
        ///     是否自动隐藏边框和标题
        /// </summary>
        public bool IsAutoHideTitleAndBorder
        {
            get { return (bool) GetValue(IsAutoHideTitleAndBorderProperty); }
            set { SetValue(IsAutoHideTitleAndBorderProperty, value); }
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

        private void FlowMessageBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //If set PanelMargin , then use it to rerender size when size changed, eles use CanvaLeft、CanvasTop...
            if (PanelMargin != new Thickness(-1, -1, -1, -1))
            {
                CanvasLeft = PanelMargin.Left;
                CanvasTop = PanelMargin.Top;

                var width = ActualWidth - PanelMargin.Left - PanelMargin.Right;
                if (!double.IsNaN(PanelMinWidth))
                    width = width < PanelMinWidth ? PanelMinWidth : width;
                if (!double.IsNaN(PanelMaxWidth))
                    width = width > PanelMaxWidth ? PanelMaxWidth : width;
                width = width < 0 ? 0 : width;

                var height = ActualHeight - PanelMargin.Top - PanelMargin.Bottom;
                if (!double.IsNaN(PanelMinHeight))
                    height = height < PanelMinHeight ? PanelMinHeight : height;
                if (!double.IsNaN(PanelMaxHeight))
                    height = height > PanelMaxHeight ? PanelMaxHeight : height;
                height = height < 0 ? 0 : height;

                PanelWidth = width;
                PanelHeight = height;
            }
            else
            {
                //use Right
                if (!double.IsNaN(CanvasRight))
                    CanvasLeft = ActualWidth - PanelWidth - CanvasRight;
                //use Bottom
                if (!double.IsNaN(CanvasBottom))
                    CanvasTop = ActualHeight - PanelHeight - CanvasBottom;

                //use Top and Left
                CanvasLeft = CanvasLeft >= ActualWidth
                    ? double.IsNaN(CanvasRight)
                        ? 0
                        : ActualWidth - PanelWidth
                    : CanvasLeft;
                CanvasLeft = CanvasLeft < 0 ? 0 : CanvasLeft;
                CanvasTop = CanvasTop >= ActualHeight
                    ? double.IsNaN(CanvasBottom)
                        ? 0
                        : ActualHeight - PanelHeight
                    : CanvasTop;
                CanvasTop = CanvasTop < 0 ? 0 : CanvasTop;

                var width = CanvasLeft + PanelWidth > ActualWidth ? ActualWidth - CanvasLeft : PanelWidth;
                if (!double.IsNaN(PanelMinWidth))
                    width = width < PanelMinWidth ? PanelMinWidth : width;
                if (!double.IsNaN(PanelMaxWidth))
                    width = width > PanelMaxWidth ? PanelMaxWidth : width;
                width = width < 0 ? 0 : width;

                var height = CanvasTop + PanelHeight > ActualHeight ? ActualHeight - CanvasTop : PanelHeight;
                if (!double.IsNaN(PanelMinHeight))
                    height = height < PanelMinHeight ? PanelMinHeight : height;
                if (!double.IsNaN(PanelMaxHeight))
                    height = height > PanelMaxHeight ? PanelMaxHeight : height;
                height = height < 0 ? 0 : height;

                PanelWidth = width;
                PanelHeight = height;
            }
        }

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
        /// <param name="isNeedLineBreak">是否需要换行</param>
        public void AddMessage(string message, bool isNeedLineBreak = true)
        {
            if (Foreground is SolidColorBrush)
                AddMessage(message, ((SolidColorBrush) Foreground).Color, isNeedLineBreak);
            else
                AddMessage(message, Colors.Black, isNeedLineBreak);
        }

        /// <summary>
        ///     添加消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="fontSize">字号</param>
        /// <param name="isNeedLineBreak">是否需要换行</param>
        public void AddMessage(string message, double fontSize, bool isNeedLineBreak = true)
        {
            if (Foreground is SolidColorBrush)
                AddMessage(message, ((SolidColorBrush) Foreground).Color, isNeedLineBreak);
            else
                AddMessage(message, Colors.Black, fontSize, isNeedLineBreak);
        }

        /// <summary>
        ///     添加消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="color">文字颜色</param>
        /// <param name="isNeedLineBreak">是否需要换行</param>
        public void AddMessage(string message, Color color, bool isNeedLineBreak = true)
        {
            AddMessage(message, color, FontSize, isNeedLineBreak);
        }

        /// <summary>
        ///     添加消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="color">文字颜色</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="isNeedLineBreak">是否需要换行</param>
        public void AddMessage(string message, Color color, double fontSize, bool isNeedLineBreak = true)
        {
            AddMessage(message, color, fontSize, FontFamily, isNeedLineBreak);
        }

        /// <summary>
        ///     添加消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="color">文字颜色</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="fontFamily">字体</param>
        /// <param name="isNeedLineBreak">是否需要换行</param>
        public void AddMessage(string message, Color color, double fontSize, FontFamily fontFamily,
            bool isNeedLineBreak = true)
        {
            string lastMessage;
            var run = CreateRun(out lastMessage, message, color, fontSize, fontFamily, isNeedLineBreak);
            var newMessage = isNeedLineBreak ? message : lastMessage + message;

            Paragraph.Inlines.Add(run);
            if(ScrollViewer!=null)
                ScrollViewer.ScrollToEnd();
            ExcuteAnimation(run);
            RaiseEvent(newMessage, lastMessage);
        }

        /// <summary>
        ///     添加链接
        /// </summary>
        /// <param name="message">链接内容</param>
        /// <param name="fontSize">字号</param>
        /// <param name="clickEventHandler">点击事件</param>
        /// <param name="isNeedLineBreak">是否需要换行</param>
        public void AddHyperLink(string message, double fontSize, RoutedEventHandler clickEventHandler,
            bool isNeedLineBreak = true)
        {
            AddHyperLink(message, Colors.DodgerBlue, fontSize, clickEventHandler, isNeedLineBreak);
        }

        /// <summary>
        ///     添加链接
        /// </summary>
        /// <param name="message">链接内容</param>
        /// <param name="clickEventHandler">点击事件</param>
        /// <param name="isNeedLineBreak">是否需要换行</param>
        public void AddHyperLink(string message, RoutedEventHandler clickEventHandler,
            bool isNeedLineBreak = true)
        {
            AddHyperLink(message, Colors.DodgerBlue, clickEventHandler, isNeedLineBreak);
        }

        /// <summary>
        ///     添加链接
        /// </summary>
        /// <param name="message">链接内容</param>
        /// <param name="color">链接颜色</param>
        /// <param name="clickEventHandler">点击事件</param>
        /// <param name="isNeedLineBreak">是都需要换行</param>
        public void AddHyperLink(string message, Color color, RoutedEventHandler clickEventHandler,
            bool isNeedLineBreak = true)
        {
            AddHyperLink(message, color, FontSize, clickEventHandler, isNeedLineBreak);
        }

        /// <summary>
        ///     添加链接
        /// </summary>
        /// <param name="message">链接内容</param>
        /// <param name="color">链接颜色</param>
        /// <param name="fontSize">字号</param>
        /// <param name="clickEventHandler">点击事件</param>
        /// <param name="isNeedLineBreak">是否需要换行</param>
        public void AddHyperLink(string message, Color color, double fontSize, RoutedEventHandler clickEventHandler,
            bool isNeedLineBreak = true)
        {
            AddHyperLink(message, color, fontSize, FontFamily, clickEventHandler, isNeedLineBreak);
        }

        /// <summary>
        ///     添加链接
        /// </summary>
        /// <param name="message">链接信息</param>
        /// <param name="color">链接颜色</param>
        /// <param name="fontSize">字号</param>
        /// <param name="fontFamily">字体</param>
        /// <param name="clickEventHandler">点击事件</param>
        /// <param name="isNeedLineBreak">是否需要换行</param>
        public void AddHyperLink(string message, Color color, double fontSize, FontFamily fontFamily,
            RoutedEventHandler clickEventHandler,
            bool isNeedLineBreak = true)
        {
            string lastMessage;
            var run = CreateRun(out lastMessage, message, null, fontSize, fontFamily, isNeedLineBreak);
            var hyper = new Hyperlink(run);
            hyper.Click += clickEventHandler;
            var newMessage = isNeedLineBreak ? message : lastMessage + message;

            var hyperStyle = new Style(typeof(Hyperlink), hyper.Style);
            var hoverTrigger = new Trigger
            {
                Property = IsMouseOverProperty,
                Value = true,
                Setters = {new Setter(ForegroundProperty, new SolidColorBrush(Colors.Red))}
            };
            hyperStyle.Triggers.Add(hoverTrigger);
            hyperStyle.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(color)));
            hyper.Style = hyperStyle;

            Paragraph.Inlines.Add(hyper);
            if(ScrollViewer!=null)
                ScrollViewer.ScrollToEnd();
            RaiseEvent(newMessage, lastMessage);
        }

        private Run CreateRun(out string lastMessage, string message, Color? color, double fontSize,
            FontFamily fontFamily,
            bool isNeedLineBreak)
        {
            lastMessage = null;
            if (Paragraph.Inlines.Count != 0)
            {
                if (Paragraph.Inlines.OfType<LineBreak>().Any())
                {
                    var pointer = Paragraph.Inlines.OfType<LineBreak>().Last().ElementStart;
                    lastMessage = GetTextInContext(pointer);
                }
                else
                {
                    var pointer = Paragraph.ContentStart;
                    lastMessage = GetTextInContext(pointer);
                }

                if (isNeedLineBreak)
                    Paragraph.Inlines.Add(new LineBreak());
            }

            var run = new Run(message);

            if (color != null)
            {
                var colort = (Color) color;
                if (!IsAnimationEnable)
                {
                    if (LastBrush == null || LastColor != colort)
                    {
                        LastBrush = new SolidColorBrush(colort);
                        LastColor = colort;
                    }
                    run.Foreground = LastBrush;
                }
                else
                {
                    run.Foreground = new SolidColorBrush(colort);
                }
            }

            run.FontSize = fontSize;
            run.FontFamily = fontFamily;
            return run;
        }

        private void RaiseEvent(string message, string lastMessage)
        {
            var args =
                new RoutedPropertyChangedEventArgs<string>(lastMessage, message);
            args.RoutedEvent = MessageAddEvent;
            RaiseEvent(args);
        }

        private void ExcuteAnimation(TextElement run)
        {
            if (IsAnimationEnable)
            {
                var brush = run.Foreground;
                var fontColorAnimation = new ColorAnimation();
                fontColorAnimation.From = AccentColor;
                fontColorAnimation.Duration = TimeSpan.FromSeconds(2);
                brush.BeginAnimation(SolidColorBrush.ColorProperty, fontColorAnimation);

                var fontSizeAnimation = new DoubleAnimation();
                fontSizeAnimation.From = run.FontSize + AccentFontSizeRatio;
                fontSizeAnimation.Duration = TimeSpan.FromSeconds(.5);
                fontSizeAnimation.EasingFunction = new SineEase {EasingMode = EasingMode.EaseOut};
                run.BeginAnimation(TextElement.FontSizeProperty, fontSizeAnimation);
            }
        }

        private string GetTextInContext(TextPointer position)
        {
            var text = "";
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                {
                    if (position.Parent is Run)
                        text += position.GetTextInRun(LogicalDirection.Forward);
                }

                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }

            return text;
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

            var canvas = GetTemplateChild("PART_Canvas") as Panel;
            if (canvas != null)
            {
                SizeChanged += FlowMessageBox_SizeChanged;
            }

            FlowDocumentScrollViewer = GetTemplateChild("PART_FlowDocumentScrollViewer") as FlowDocumentScrollViewer;
            if (FlowDocumentScrollViewer != null)
            {
                FlowDocumentScrollViewer.Document = Document;
                FlowDocumentScrollViewer.SizeChanged += FlowDocumentScrollViewer_SizeChanged;
            }

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

        private void FlowDocumentScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_isDragDropInEffect || _isResizeEffect)
                return;
            if(ScrollViewer!=null)
                ScrollViewer.ScrollToEnd();
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

                xAdjust = xAdjust > ActualWidth - CanvasLeft ? ActualWidth - CanvasLeft : xAdjust;
                yAdjust = yAdjust > ActualHeight - CanvasTop ? ActualHeight - CanvasTop : yAdjust;

                xAdjust = _messageView.ActualWidth + xAdjust > _messageView.MinWidth ? xAdjust : _messageView.MinWidth;
                yAdjust = _messageView.ActualHeight + yAdjust > _messageView.MinHeight
                    ? yAdjust
                    : _messageView.MinHeight;

                xAdjust = xAdjust > _messageView.MaxWidth ? _messageView.MaxWidth : xAdjust;
                yAdjust = yAdjust > _messageView.MaxHeight ? _messageView.MaxHeight : yAdjust;

                xAdjust = xAdjust < 0 ? 0 : xAdjust;
                yAdjust = yAdjust < 0 ? 0 : yAdjust;

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
                if(ScrollViewer!=null)
                    ScrollViewer.ScrollToEnd();
            }
        }
    }
}