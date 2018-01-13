using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace UF01
{
  /// <summary>
  /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
  /// </summary>
  public sealed partial class MainPage : Page
  {
    private SampleData SampleData => (App.Current as App).SampleData;

    public MainPage()
    {
      this.InitializeComponent();
    }

    private void AppBarButton_Click(object sender, RoutedEventArgs e)
    {
      this.Frame.Navigate(typeof(Page2));
    }

    private void colorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      this.SampleData.SampleColor
        = ((SolidColorBrush)(colorComboBox.SelectedItem as FrameworkElement).Tag).Color;
    }
  }
}
