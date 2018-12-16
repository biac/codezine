using System;

using Sample02.Tabs.MvvmLight.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Sample02.Tabs.MvvmLight.Views
{
    public sealed partial class WebViewPage : Page
    {
        private WebViewViewModel ViewModel
        {
            get { return ViewModelLocator.Current.WebViewViewModel; }
        }

        public WebViewPage()
        {
            InitializeComponent();
            ViewModel.Initialize(webView);
        }
    }
}
