using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    private DesktopBridge.Helpers _desktopBridgeHelpers 
      = new DesktopBridge.Helpers();

    public MainWindow()
    {
      InitializeComponent();

      if (!_desktopBridgeHelpers.IsRunningAsUwp())
        this.CardTypePanel.Visibility = Visibility.Collapsed;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (Uri.TryCreate(this.UrlTextBox.Text, UriKind.Absolute, out var uri))
        this.WebView1.Navigate(uri);
    }

    private void WebView1_Navigating(object sender, NavigatingCancelEventArgs e)
    {
      this.WebView1.Visibility = Visibility.Hidden;
      this.Progress1.Visibility = Visibility.Visible;
    }

    private async void WebView1_NavigationCompleted(object sender, NavigationEventArgs e)
    {
      this.WebView1.Visibility = Visibility.Visible;
      this.Progress1.Visibility = Visibility.Hidden;
      this.UrlTextBox.Text = e.Uri.ToString();

      if (_desktopBridgeHelpers.IsRunningAsUwp())
        await TimelineLIb.TimelineHelper.Current.AddToTimelineAsync(e.Uri.ToString(), GetCardType());
    }

    private TimelineLIb.AdaptiveCardType GetCardType()
    {
      if (this.CardByCode.IsChecked == true)
        return TimelineLIb.AdaptiveCardType.ByCode;
      if (this.CardByJson.IsChecked == true)
        return TimelineLIb.AdaptiveCardType.ByJson;
      return TimelineLIb.AdaptiveCardType.None;
    }

    private void WebView1_NavigationFailed(object sender, NavigationFailedEventArgs e)
    {
      e.Handled = true;

      this.WebView1.Visibility = Visibility.Visible;
      this.Progress1.Visibility = Visibility.Hidden;
    }

    internal async void Navigate(string url)
    {
      if (this.WebView1.Source?.ToString() == url)
      {
        await TimelineLIb.TimelineHelper.Current.AddToTimelineAsync(url, GetCardType());
      }
      else
      {
        this.UrlTextBox.Text = url;
        this.WebView1.Navigate(new Uri(url));
      }
    }
  }
}
