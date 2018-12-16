using System;

using Sample02.Tabs.MvvmLight.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Sample02.Tabs.MvvmLight.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel
        {
            get { return ViewModelLocator.Current.MainViewModel; }
        }

        public MainPage()
        {
            InitializeComponent();

            // bw: ビューモデルを初期化
            ViewModel.Initialize();
        }
    }
}
