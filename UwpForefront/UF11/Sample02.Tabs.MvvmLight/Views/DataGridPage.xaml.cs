using System;

using Sample02.Tabs.MvvmLight.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Sample02.Tabs.MvvmLight.Views
{
    public sealed partial class DataGridPage : Page
    {
        private DataGridViewModel ViewModel
        {
            get { return ViewModelLocator.Current.DataGridViewModel; }
        }

        // TODO WTS: Change the grid as appropriate to your app, adjust the column definitions on DataGridPage.xaml.
        // For more details see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid
        public DataGridPage()
        {
            InitializeComponent();
        }
    }
}
