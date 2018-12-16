using System;
using System.Collections.ObjectModel;

using GalaSoft.MvvmLight;

using Sample02.Tabs.MvvmLight.Models;
using Sample02.Tabs.MvvmLight.Services;

namespace Sample02.Tabs.MvvmLight.ViewModels
{
    public class DataGridViewModel : ViewModelBase
    {
        public ObservableCollection<SampleOrder> Source
        {
            get
            {
                // TODO WTS: Replace this with your actual data
                return SampleDataService.GetGridSampleData();
            }
        }
    }
}
