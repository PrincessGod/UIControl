using System.Windows;
using StoreDatabase;

namespace UIControl
{
    /// <summary>
    ///     App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static StoreDb StoreDb { get; } = new StoreDb();

        public static StoreDbDataSet StoreDbDataSet { get; } = new StoreDbDataSet();
    }
}