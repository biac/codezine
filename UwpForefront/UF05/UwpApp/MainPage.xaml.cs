using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace UwpApp
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

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      if (e.Parameter is string url && !string.IsNullOrWhiteSpace(url))
        Navigate(url);
    }


    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (Uri.TryCreate(this.UrlTextBox.Text, UriKind.Absolute, out var uri))
        this.WebView1.Navigate(uri);
    }

    private void WebView1_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
    {
      this.WebView1.Opacity = 0.5;
      this.Progress1.IsActive = true;
    }

    private async void WebView1_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
    {
      this.WebView1.Opacity = 1.0;
      this.Progress1.IsActive = false;
      if (!args.IsSuccess)
        return;

      this.UrlTextBox.Text = args.Uri.ToString();
      await TimelineLIb.TimelineHelper.Current
              .AddToTimelineAsync(args.Uri.ToString(), GetCardType());
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
