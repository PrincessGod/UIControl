using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace UIControl
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Menu : Window
    {

        public Menu()
        {
            InitializeComponent();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {            
            // Get the current button.
            Button cmd = (Button)e.OriginalSource;
                
            // Create an instance of the window named
            // by the current button.
            Type type = this.GetType();
            Assembly assembly = type.Assembly;                       
            Window win = (Window)assembly.CreateInstance(
                type.Namespace + "." + cmd.Tag);

            // Show the window.
            win.ShowDialog();
        }
    }
}