using Sample01.Hamburger.CodeBehind.Helpers;
using Sample01.Hamburger.CodeBehind.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sample01.Hamburger.CodeBehind.Views
{
    public sealed partial class WebViewPage : Page, INotifyPropertyChanged
    {
        // TODO WTS: Set the URI of the page to show by default
        // private const string DefaultUrl = "https://developer.microsoft.com/en-us/windows/apps";
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

        private void OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            IsLoading = false;
            OnPropertyChanged(nameof(IsBackEnabled));
            OnPropertyChanged(nameof(IsForwardEnabled));
        }

        private void OnNavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            // Use `e.WebErrorStatus` to vary the displayed message based on the error reason
            IsShowingFailedMessage = true;
        }

        private void OnRetry(object sender, RoutedEventArgs e)
        {
            IsShowingFailedMessage = false;
            IsLoading = true;

            webView.Refresh();
        }

        public bool IsBackEnabled
        {
            get { return webView.CanGoBack; }
        }

        public bool IsForwardEnabled
        {
            get { return webView.CanGoForward; }
        }

        private void OnGoBack(object sender, RoutedEventArgs e)
        {
            webView.GoBack();
        }

        private void OnGoForward(object sender, RoutedEventArgs e)
        {
            webView.GoForward();
        }

        private void OnRefresh(object sender, RoutedEventArgs e)
        {
            webView.Refresh();
        }

        private async void OnOpenInBrowser(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(webView.Source);
        }

        public WebViewPage()
        {
            Source = new Uri(DefaultUrl);
            InitializeComponent();
            IsLoading = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



        // bw: 共有ボタンのイベントハンドラー
        private void OnShareTo(object sender, RoutedEventArgs e)
        {
            ShareUrl();
        }

        // bw: 共有へ送るAPI（DataTransferManagerオブジェクト）
        private DataTransferManager dataTransferManager;

        // bw: 表示しているWebページの情報を共有へ送る
        // 以下のusingが必要
        //   using Sample01.Hamburger.CodeBehind.Helpers;
        //   using Sample01.Hamburger.CodeBehind.Models;
        //   using Windows.ApplicationModel.DataTransfer;
        private void ShareUrl()
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
                Uri uri = this.webView.Source;
                string pageTitle = this.webView.DocumentTitle;

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
        }
    }
}
