using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace UF12
{
  /// <summary>
  /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();

      SetupAutoStartupToggle();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      string naviParam = e.Parameter as string;
      var currentApp = App.Current as App;
      if (currentApp.PrelaunchTime.HasValue)
      {
        var prelaunchTime = currentApp.PrelaunchTime.Value.ToString("HH:mm:ss");
        var launchTime = DateTimeOffset.Now.ToString("HH:mm:ss");
        PrelauchTimeText.Text = $"事前起動{prelaunchTime}⇒起動{launchTime}";
      }

      Window.Current.VisibilityChanged += OnFirstVisible;
      void OnFirstVisible(object sender, Windows.UI.Core.VisibilityChangedEventArgs ea)
      {
        if (currentApp.IsAutoStartup)
        {
          string taskId = naviParam;
          FlyoutText.Text = $"自動起動しました。(TaskId={taskId})";
        }
        else if (currentApp.IsRestart)
        {
          ParamText.Text = naviParam;
          FlyoutText.Text = $"再起動しました。";
        }
        else if (currentApp.IsCommandLineLaunch)
        {
          string taskId = naviParam;
          FlyoutText.Text = $"コマンドラインから起動されました。\n引数=\"{naviParam}\"";
        }
        else
        {
          FlyoutText.Text = $"通常起動しました。";
        }
        FlyoutBase.ShowAttachedFlyout(ContentsGrid);

        // 1回表示した後はもう表示しないようにするため、ハンドラーを外す
        Window.Current.VisibilityChanged -= OnFirstVisible;
      };
    }

    private void CloseFlyoutButton_Click(object sender, RoutedEventArgs e)
    {
      FlyoutBase.GetAttachedFlyout(ContentsGrid).Hide();
    }
  }
}
