using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace CalendarDatePickerTest
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CalendarSelector.Items.Add(CalendarIdentifiers.Gregorian);
            CalendarSelector.Items.Add(CalendarIdentifiers.Japanese);
            CalendarSelector.Items.Add(CalendarIdentifiers.Persian);
            CalendarSelector.SelectedIndex = 0;

            DatePicker.Date = DateTimeOffset.Now;

            m_PageLoaded = true;
        }
        private bool m_PageLoaded;

        private void CalendarSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!m_PageLoaded)
                return;

            DatePicker.CalendarIdentifier = e.AddedItems[0] as string;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.IsEnabled = false;
            await Task.Delay(1000);

            // change CalendarIdentifier (first time)
            DatePicker.CalendarIdentifier = CalendarIdentifiers.Thai;

            // open and close CalendarDatePicker
            DatePicker.IsCalendarOpen = true;
            await Task.Delay(100);
            DatePicker.IsCalendarOpen = false;

            // change CalendarIdentifier (second time)
            DatePicker.CalendarIdentifier = CalendarIdentifiers.Gregorian;

            this.IsEnabled = true;
            // from now on, when you open CalendarDatePicker, a memory leak occurs!
        }
    }
}
