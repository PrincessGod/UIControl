using System.Windows;

namespace UIControl
{
    /// <summary>
    ///     MoveableControl.xaml 的交互逻辑
    /// </summary>
    public partial class MoveableControl : Window
    {
        public MoveableControl()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            A.Visibility = Visibility.Visible;
        }
    }
}