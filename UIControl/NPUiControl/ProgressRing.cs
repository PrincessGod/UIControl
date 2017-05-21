using System.Windows;
using System.Windows.Controls;

namespace NPUiControl
{
    /// <summary>
    ///     未知时长的等待控件
    ///     资源文件中有名为 Accent 的 SolidColorBrush 控制默认前景色
    /// </summary>
    [TemplateVisualState(GroupName = GroupActiveStates, Name = StateInactive)]
    [TemplateVisualState(GroupName = GroupActiveStates, Name = StateActive)]
    public class ModernProgressRing
        : Control
    {
        private const string GroupActiveStates = "ActiveStates";
        private const string StateInactive = "Inactive";
        private const string StateActive = "Active";

        /// <summary>
        ///     是否激活属性
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive",
            typeof(bool), typeof(ModernProgressRing), new PropertyMetadata(false, OnIsActiveChanged));

        /// <summary>
        ///     初始化一个 <see cref="ModernProgressRing" /> 对象
        /// </summary>
        public ModernProgressRing()
        {
            DefaultStyleKey = typeof(ModernProgressRing);
        }

        /// <summary>
        ///     获取或设置 <see cref="ModernProgressRing" /> 的激活状态
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        private void GotoCurrentState(bool animate)
        {
            var state = IsActive ? StateActive : StateInactive;

            VisualStateManager.GoToState(this, state, animate);
        }

        /// <summary>
        ///     当应用模板时控件触发此方法
        ///     <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            GotoCurrentState(false);
        }

        private static void OnIsActiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ModernProgressRing)o).GotoCurrentState(true);
        }
    }
}
