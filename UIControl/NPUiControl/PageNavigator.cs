using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
using NPUiControl.Annotations;

namespace NPUiControl
{
    /// <summary>
    ///     页面导航器
    ///     用于对 ItemsControl 进行页面导航
    /// </summary>
    public class PageNavigator : INotifyPropertyChanged
    {
        private int _currentNum;
        private int _endNum;
        private int _maxPage;
        private bool _nextEnable;
        private int _numPerPage;
        private int _page;
        private string _pageInfo;
        private bool _preEnable;
        private int _startNum;
        private CollectionView _view;

        /// <summary>
        ///     导航信息格式化字符串 {0} 当前页  {1} 总页数
        /// </summary>
        public string StringFormat = "Page {0} of {1}";

        /// <summary>
        ///     页面导航器
        /// </summary>
        /// <param name="view">数据视图</param>
        /// <param name="collection">ItemSource 绑定对象</param>
        /// <param name="numPerPage">每页条目数</param>
        public PageNavigator(CollectionView view, INotifyCollectionChanged collection, int numPerPage)
        {
            var collection1 = collection;
            View = view;
            NumPerPage = numPerPage;
            if (collection1 != null)
                collection1.CollectionChanged += collection1_CollectionChanged;

            Measure();
        }

        /// <summary>
        ///     手动管理刷新
        /// </summary>
        public bool RefreshBySelf { get; set; }

        /// <summary>
        ///     当前页
        /// </summary>
        public int Page
        {
            get { return _page; }
            set
            {
                _page = value;
                OnPropertyChanged("Page");
            }
        }

        /// <summary>
        ///     每页条目数
        /// </summary>
        public int NumPerPage
        {
            get { return _numPerPage; }
            set
            {
                _numPerPage = value;
                OnPropertyChanged("NumPerPage");
            }
        }

        /// <summary>
        ///     数据视图
        /// </summary>
        public CollectionView View
        {
            get { return _view; }
            set
            {
                _view = value;
                OnPropertyChanged("View");
            }
        }

        /// <summary>
        ///     最大页数
        /// </summary>
        public int MaxPage
        {
            get { return _maxPage; }
            set
            {
                _maxPage = value;
                OnPropertyChanged("MaxPage");
            }
        }

        /// <summary>
        ///     导航信息
        /// </summary>
        public string PageInfo
        {
            get { return _pageInfo; }
            set
            {
                _pageInfo = value;
                OnPropertyChanged("PageInfo");
                PreEnable = Page > 1;
                NextEnable = Page < MaxPage;
            }
        }

        /// <summary>
        ///     是否能向前导航
        /// </summary>
        public bool PreEnable
        {
            get { return _preEnable; }
            set
            {
                _preEnable = value;
                OnPropertyChanged("PreEnable");
            }
        }

        /// <summary>
        ///     是否能向后导航
        /// </summary>
        public bool NextEnable
        {
            get { return _nextEnable; }
            set
            {
                _nextEnable = value;
                OnPropertyChanged("NextEnable");
            }
        }

        /// <summary>
        ///     INotifyPropertyChanged 实现
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void collection1_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Measure();
            if (!RefreshBySelf)
                Refrash();
        }

        private void Measure()
        {
            if (View != null)
            {
                View.Filter = null;
                MaxPage = View.Count / NumPerPage;
                MaxPage += View.Count % NumPerPage != 0 ? 1 : 0;
                PageInfo = string.Format(StringFormat, Page, MaxPage);
            }
        }

        /// <summary>
        ///     导航到前一页 超出范围导航到第一页
        /// </summary>
        public void PrePage()
        {
            Measure();
            if (Page > 1)
                GoPage(--Page);
            else
                GoPage(1);
        }

        /// <summary>
        ///     导航到后一页 超出范围导航到最后一页
        /// </summary>
        public void NextPage()
        {
            Measure();
            if (Page < MaxPage)
                GoPage(++Page);
            else
                GoPage(MaxPage);
        }

        /// <summary>
        ///     到达某一页  根据超限值选择导航到首页或尾页
        /// </summary>
        /// <param name="page"></param>
        public void GoPage(int page)
        {
            Measure();
            page = page <= 0 ? 1 : page;
            page = page > MaxPage ? MaxPage : page;
            if (View != null)
            {
                _currentNum = 0;
                _startNum = (page - 1) * NumPerPage;
                _endNum = _startNum + NumPerPage;
                View.Filter = FilterItem;
                Page = page;
                PageInfo = string.Format(StringFormat, Page, MaxPage);
            }
        }

        /// <summary>
        ///     当集合改变后调用此接口可刷新集合
        /// </summary>
        public void Refrash()
        {
            GoPage(Page);
        }

        private bool FilterItem(object item)
        {
            if (_currentNum >= _startNum && _currentNum < _endNum)
            {
                _currentNum++;
                return true;
            }
            _currentNum++;
            return false;
        }

        /// <summary>
        ///     INotifyPropertyChanged 通知
        /// </summary>
        /// <param name="propertyName"></param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}