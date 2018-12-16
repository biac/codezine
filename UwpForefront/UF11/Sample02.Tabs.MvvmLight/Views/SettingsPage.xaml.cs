using System;

using Sample02.Tabs.MvvmLight.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sample02.Tabs.MvvmLight.Views
{
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page
    {
        private SettingsViewModel ViewModel
        {
            get { return ViewModelLocator.Current.SettingsViewModel; }
        }

        public SettingsPage()
        {
            InitializeComponent();
            ViewModel.Initialize();
        }
    }
}
