using System;

using Sample02.Tabs.MvvmLight.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sample02.Tabs.MvvmLight.Views
{
    public sealed partial class MapPage : Page
    {
        private MapViewModel ViewModel
        {
            get { return ViewModelLocator.Current.MapViewModel; }
        }

        public MapPage()
        {
            InitializeComponent();
            Loaded += MapPage_Loaded;
            Unloaded += MapPage_Unloaded;
        }

        private async void MapPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.InitializeAsync(mapControl);
        }

        private void MapPage_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Cleanup();
        }
    }
}
