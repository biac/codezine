using System;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Sample02.Tabs.MvvmLight.Helpers;
using Sample02.Tabs.MvvmLight.Models;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sample02.Tabs.MvvmLight.ViewModels
{
    public class WebViewViewModel : ViewModelBase
    {
        // TODO WTS: Set the URI of the page to show by default
        //private const string DefaultUrl = "https://developer.microsoft.com/en-us/windows/apps";
        // bw: 初期表示URLを変更
        private const string DefaultUrl = "https://codezine.jp/article/corner/731";

        private Uri _source;

        public Uri Source
        {
            get { return _source; }
            set { Set(ref _source, value); }
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }

            set
            {
                if (value)
                {
                    IsShowingFailedMessage = false;
                }

                Set(ref _isLoading, value);
                IsLoadingVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _isLoadingVisibility;

        public Visibility IsLoadingVisibility
        {
            get { return _isLoadingVisibility; }
            set { Set(ref _isLoadingVisibility, value); }
        }

        private bool _isShowingFailedMessage;

        public bool IsShowingFailedMessage
        {
            get
            {
                return _isShowingFailedMessage;
            }

            set
            {
                if (value)
                {
                    IsLoading = false;
                }

                Set(ref _isShowingFailedMessage, value);
                FailedMesageVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _failedMesageVisibility;

        public Visibility FailedMesageVisibility
        {
            get { return _failedMesageVisibility; }
            set { Set(ref _failedMesageVisibility, value); }
        }

        private ICommand _navCompleted;

        public ICommand NavCompletedCommand
        {
            get
            {
                if (_navCompleted == null)
                {
                    _navCompleted = new RelayCommand<WebViewNavigationCompletedEventArgs>(NavCompleted);
                }

                return _navCompleted;
            }
        }

        private void NavCompleted(WebViewNavigationCompletedEventArgs e)
        {
            IsLoading = false;
            RaisePropertyChanged(nameof(BrowserBackCommand));
            RaisePropertyChanged(nameof(BrowserForwardCommand));
        }

        private ICommand _navFailed;

        public ICommand NavFailedCommand
        {
            get
            {
                if (_navFailed == null)
                {
                    _navFailed = new RelayCommand<WebViewNavigationFailedEventArgs>(NavFailed);
                }

                return _navFailed;
            }
        }

        private void NavFailed(WebViewNavigationFailedEventArgs e)
        {
            // Use `e.WebErrorStatus` to vary the displayed message based on the error reason
            IsShowingFailedMessage = true;
        }

        private ICommand _retryCommand;

        public ICommand RetryCommand
        {
            get
            {
                if (_retryCommand == null)
                {
                    _retryCommand = new RelayCommand(Retry);
                }

                return _retryCommand;
            }
        }

        private void Retry()
        {
            IsShowingFailedMessage = false;
            IsLoading = true;

            _webView?.Refresh();
        }

        private ICommand _browserBackCommand;

        public ICommand BrowserBackCommand
        {
            get
            {
                if (_browserBackCommand == null)
                {
                    _browserBackCommand = new RelayCommand(() => _webView?.GoBack(), () => _webView?.CanGoBack ?? false);
                }

                return _browserBackCommand;
            }
        }

        private ICommand _browserForwardCommand;

        public ICommand BrowserForwardCommand
        {
            get
            {
                if (_browserForwardCommand == null)
                {
                    _browserForwardCommand = new RelayCommand(() => _webView?.GoForward(), () => _webView?.CanGoForward ?? false);
                }

                return _browserForwardCommand;
            }
        }

        private ICommand _refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(() => _webView?.Refresh());
                }

                return _refreshCommand;
            }
        }

        private ICommand _openInBrowserCommand;

        public ICommand OpenInBrowserCommand
        {
            get
            {
                if (_openInBrowserCommand == null)
                {
                    _openInBrowserCommand = new RelayCommand(async () => await Windows.System.Launcher.LaunchUriAsync(Source));
                }

                return _openInBrowserCommand;
            }
        }



        // bw: 共有へ送るAPI（DataTransferManagerオブジェクト）
        private DataTransferManager dataTransferManager;

        // bw: 共有へ送るコマンド
        private ICommand _shareToCommand;
        public ICommand ShareToCommand
        {
            get
            {
                if (_shareToCommand == null)
                {
                    _shareToCommand = new RelayCommand(() =>
                    {
                        // - Step 1. Setup a DataTransferManager object in your page / view and add a DataRequested event handler
                        //   (i.e. OnDataRequested) to be called whenever the user invokes share.
                        // ステップ1：DataTransferManagerオブジェクトを取得し、
                        //            DataRequestedイベントハンドラーを設定する
                        if (dataTransferManager == null)
                        {
                            dataTransferManager = DataTransferManager.GetForCurrentView();
                            dataTransferManager.DataRequested += OnDataRequested;
                        }

                        // DataRequestedイベントハンドラー（内部関数）
                        void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
                        {
                            // 共有へ送りたいデータ
                            Uri uri = _webView.Source;
                            string pageTitle = _webView.DocumentTitle;

                            // - Step 2. Within the OnDataRequested event handler create a ShareSourceData instance and add the data you want to share.
                            // ステップ2：WTSのShareSourceDataオブジェクトを作り、共有したいデータをセットする
                            var shareSourceData = new ShareSourceData(pageTitle);
                            shareSourceData.SetWebLink(uri);

                            // - Step 3. Call the SetData extension method before leaving the event handler (i.e. args.Request.SetData(shareSourceData))
                            // ステップ3：WTSのSetData拡張メソッドを呼び出す
                            args.Request.SetData(shareSourceData);
                        }

                        // - Step 4. Call the DataTransferManager.ShowShareUI method from your command or handler to start the sharing action
                        // ステップ4：ShowShareUI静的メソッドを呼び出すと、共有ポップアップが出る
                        DataTransferManager.ShowShareUI();
                    });
                }

                return _shareToCommand;
            }
        }


        private WebView _webView;

        public WebViewViewModel()
        {
            IsLoading = true;
            Source = new Uri(DefaultUrl);
        }

        public void Initialize(WebView webView)
        {
            _webView = webView;
        }
    }
}
