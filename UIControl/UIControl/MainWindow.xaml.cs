using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UIControl
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        Random rd = new Random();
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var color = Colors.Black;
            var size = 0;
            if (CheckBox1.IsChecked == true)
            {
                color = Color.FromRgb((byte)rd.Next(256), (byte)rd.Next(256), (byte)rd.Next(256));
            }
            if (CheckBox2.IsChecked == true)
            {
                size = rd.Next(12, 18);
            }

            if (size == 0)
                FlowMessageBox1.AddMessage(rd.Next(50000).ToString("C"), color);
            else
                FlowMessageBox1.AddMessage(rd.Next(50000).ToString("C"), color, size);
        }

        private void FlowMessageBox1_OnMessageAdded(object sender, RoutedPropertyChangedEventArgs<string> e)
        {
            TextBlock.Text = $"旧值 {e.OldValue}   新值 {e.NewValue}";
        }

        private void ButtonBase1_OnClick(object sender, RoutedEventArgs e)
        {
            FlowMessageBox1.ClearMessage();
        }
    }
}
