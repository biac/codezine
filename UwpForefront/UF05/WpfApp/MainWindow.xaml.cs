using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public MainWindow()
    {
      InitializeComponent();

      // UWP Bridge での実行でないときは、アダプティブカードの選択肢を消す
      if(!App.IsTimelineAvailable)
        this.CardTypePanel.Visibility = Visibility.Collapsed;

      // JavaScript エラーのポップアップが出るのを抑止する
      // https://stackoverflow.com/a/18289217
      this.WebView1.Loaded += (s, e) =>
      {
        dynamic activeX = this.WebView1.GetType().InvokeMember("ActiveXInstance",
                      BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                      null, this.WebView1, new object[] { });
        activeX.Silent = true;
      };
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

    private async void WebView1_Navigated(object sender, NavigationEventArgs e)
    {
      // ここは別スレッドから呼ばれることがある
      await this.Dispatcher.InvokeAsync(async () =>
      {
        this.WebView1.Visibility = Visibility.Visible;
        this.Progress1.Visibility = Visibility.Hidden;

        // https://stackoverflow.com/a/46132464/1327929
        dynamic doc = this.WebView1.Document;
        var url = doc.url as string;
        if (url != null)
        {
          this.UrlTextBox.Text = url;
          if (url.StartsWith("http"))
          {
            if (App.IsTimelineAvailable)
              await TimelineLIb.TimelineHelper.Current.AddToTimelineAsync(url, GetCardType());
          }
        }
      });
    }

    private TimelineLIb.AdaptiveCardType GetCardType()
    {
      if (this.CardByCode.IsChecked == true)
        return TimelineLIb.AdaptiveCardType.ByCode;
      if (this.CardByJson.IsChecked == true)
        return TimelineLIb.AdaptiveCardType.ByJson;
      return TimelineLIb.AdaptiveCardType.None;
    }



    internal async void Navigate(string url)
    {
      if (this.WebView1.Source?.ToString() == url)
      {
        // 表示しているのと同じ URL が指定されたので、
        // Web ページは遷移させずに、ユーザーアクティビティの更新だけ行う
        if (App.IsTimelineAvailable)
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
