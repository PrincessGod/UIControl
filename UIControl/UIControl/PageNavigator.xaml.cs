using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using StoreDatabase;

namespace UIControl
{
    /// <summary>
    /// PageNavigator.xaml 的交互逻辑
    /// </summary>
    public partial class PageNavigator : Window
    {
        public NPUiControl.PageNavigator FileFilterer { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public PageNavigator()
        {
            InitializeComponent();

            var products = App.StoreDb.GetProducts();

            Products = new ObservableCollection<Product>();

            foreach (var p in products)
            {
                Products.Add(p);
            }
            this.DataContext = this;
            lstProducts.ItemsSource = Products;
            CollectionView view = CollectionViewSource.GetDefaultView(lstProducts.ItemsSource) as ListCollectionView;
            FileFilterer = new NPUiControl.PageNavigator(view, (INotifyCollectionChanged)lstProducts.ItemsSource, 10);

            FileFilterer.GoPage(1);
        }

        private void cmdNext_Click(object sender, RoutedEventArgs e)
        {
            FileFilterer.NextPage();
        }
        private void cmdPrev_Click(object sender, RoutedEventArgs e)
        {
            FileFilterer.PrePage();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Products.Add(new Product("123", DateTime.Now.ToString(), 0, ""));
        }
    }
}
