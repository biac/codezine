using System;

using Sample02.Tabs.MvvmLight.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Sample02.Tabs.MvvmLight.Views
{
    public sealed partial class ChartPage : Page
    {
        private ChartViewModel ViewModel
        {
            get { return ViewModelLocator.Current.ChartViewModel; }
        }

        // TODO WTS: Change the chart as appropriate to your app.
        // For help see http://docs.telerik.com/windows-universal/controls/radchart/getting-started
        public ChartPage()
        {
            InitializeComponent();
        }
    }
}
