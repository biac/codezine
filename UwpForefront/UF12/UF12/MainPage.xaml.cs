using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
      //SetupCloseRequestedHandler();

      Window.Current.VisibilityChanged += OnFirstVisible;
      void OnFirstVisible(object sender, Windows.UI.Core.VisibilityChangedEventArgs e)
      {
        var currentApp = App.Current as App;

        //string payload = e.Parameter as string;
        if (currentApp.IsAutoStartup)
        {
          string taskId = _naviParam;
          FlyoutText.Text = $"自動起動しました。(TaskId={taskId})";
        }
        else if (currentApp.IsRestart)
        {
          ParamText.Text = _naviParam;
          FlyoutText.Text = $"再起動しました。";
        }
        else
        {
          FlyoutText.Text = $"通常起動しました。";
        }
        FlyoutBase.ShowAttachedFlyout(ContentsGrid);

        // 1回表示した後はもう表示しないようにするため、ハンドラーを外す
        Window.Current.VisibilityChanged -= OnFirstVisible;
      };

      //// [×]ボタンのイベントハンドラー (1703以降)
      //// ※マニフェストに <rescap:Capability Name="confirmAppClose"/> が必要
      //Windows.UI.Core.Preview.SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += async(s,e) => {

      //  e.Handled = true;

      //  ContentDialog subscribeDialog = new ContentDialog
      //  {
      //    Title = "アプリを終了しますか?",
      //    Content = "[×] ボタンが押されましたが、アプリを終了してもよいですか",
      //    CloseButtonText = "キャンセル",
      //    PrimaryButtonText = "終了",
      //    //SecondaryButtonText = "Try it",
      //    DefaultButton = ContentDialogButton.Primary
      //  };

      //  ContentDialogResult result = await subscribeDialog.ShowAsync();
      //  if (result == ContentDialogResult.Primary)
      //    App.Current.Exit();
      //  // ↑Exitすると、OnSuspending等は実行されないので注意!
      //};
    }



    string _naviParam;

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      _naviParam = e.Parameter as string;

      //SetupAutoStartupToggle();

      var currentApp = App.Current as App;
      if (currentApp.PrelaunchTime.HasValue)
      {
        var prelaunchTime = currentApp.PrelaunchTime.Value.ToString("HH:mm:ss");
        var launchTime = DateTimeOffset.Now.ToString("HH:mm:ss");
        PrelauchTimeText.Text = $"事前起動{prelaunchTime}⇒起動{launchTime}";
      }

      ////string payload = e.Parameter as string;
      //if (currentApp.IsAutoStartup)
      //{
      //  string taskId = e.Parameter as string;
      //  //await (new MessageDialog($"自動起動しました。(TaskId={taskId})"))
      //  //            .ShowAsync();
      //  //ダイアログ出すとハングする

      //  FlyoutText.Text = $"自動起動しました。(TaskId={taskId})";
      //  FlyoutBase.ShowAttachedFlyout(RootGrid);
      //}
      //else if (currentApp.IsRestart)
      //{
      //  string param = e.Parameter as string;
      //  ParamText.Text = param;

      //  FlyoutText.Text = $"再起動しました。";
      //  FlyoutBase.ShowAttachedFlyout(RootGrid);
      //}
      //else {
      //  FlyoutText.Text = $"通常起動しました。";
      //  FlyoutBase.ShowAttachedFlyout(RootGrid);
      //}
    }

    private void CloseFlyoutButton_Click(object sender, RoutedEventArgs e)
    {
      FlyoutBase.GetAttachedFlyout(ContentsGrid).Hide();
    }



    //private async void AutoStartupToggle_Toggled(object sender, RoutedEventArgs e)
    //{
    //  if (!_isSetupCompleted)
    //    return; // 初期化中にコードからトグルを切り替えた

    //  StartupTask startupTask = await StartupTask.GetAsync(StartUpTaskId);
    //  if ((sender as ToggleSwitch).IsOn)
    //  {
    //    // ON にする
    //    await startupTask.RequestEnableAsync();
    //  }
    //  else
    //  {
    //    // OFF にする
    //    startupTask.Disable();
    //  }

    //  // 実際の状態がトグルと異なっているかもしれないので、再度初期化する
    //  SetupAutoStartupToggle();
    //}

//    private async void RestartButton_Click(object sender, RoutedEventArgs e)
//    {
//      using (var newSession = new ExtendedExecutionSession())
//      {
//        newSession.Reason = ExtendedExecutionReason.Unspecified;
//        await newSession.RequestExtensionAsync();
//        // ↑最小化されたときにサスペンドされるのを延期する


//        RestartButton.IsEnabled = false;
//        RestartDescriptionText.Text = "";
//        await Task.Delay(3000); // 動作確認のため、しばらく遅らせる

//        // 再起動を要求する
//        string param = App.RestartParamHeader + ParamText.Text;
//        AppRestartFailureReason result =
//              await CoreApplication.RequestRestartAsync(param);

//        switch (result)
//        {
//          case AppRestartFailureReason.RestartPending:
//            // 正常に再起動処理が始まった
//            RestartDescriptionText.Text = "再起動中…";//この表示は見えない
//            break;
//          case AppRestartFailureReason.NotInForeground:
//            // 失敗
//            RestartDescriptionText.Text
//              = "再起動失敗：アプリがフォアグラウンドになっていません。";
//            break;
//          case AppRestartFailureReason.InvalidUser:
//            // 失敗（引数にユーザーを指定した場合に発生しうる）
//            RestartDescriptionText.Text
//              = "AppRestartFailureReason.InvalidUser";
//            break;
//          case AppRestartFailureReason.Other:
//            // 失敗
//            RestartDescriptionText.Text
//              = "再起動失敗：予期しない理由です。";
//            break;

//#if DEBUG
//          default:
//            throw new ArgumentOutOfRangeException($"想定外のAppRestartFailureReason({result})");
//#endif
//        }
//        RestartButton.IsEnabled = true;
//      }
//    }





//    const string StartUpTaskId = "UF12StartupId";
//    //bool _isInSetupAutoStartup;
//    bool _isSetupCompleted;

//    private async void SetupAutoStartupToggle()
//    {
//      //_isInSetupAutoStartup = true;
//      _isSetupCompleted = false;

//      StartupTask startupTask = await StartupTask.GetAsync(StartUpTaskId);
//      // StartupTask を使うには、Package.appxmanifest に windows.startupTask の宣言が必要

//      switch (startupTask.State)
//      {
//        case StartupTaskState.Disabled:
//          // トグル OFF、変更可能
//          AutoStartupToggle.IsOn = false;
//          AutoStartupToggle.IsEnabled = true;
//          AutoStartupDescriptionText.Text = string.Empty;
//          break;
//        case StartupTaskState.DisabledByUser:
//          // トグル OFF、変更不可
//          AutoStartupToggle.IsOn = false;
//          AutoStartupToggle.IsEnabled = false;
//          AutoStartupDescriptionText.Text
//            = "タスクマネージャーのスタートアップタブで有効にできます";
//          break;
//        case StartupTaskState.DisabledByPolicy:
//          // トグル OFF、変更不可
//          AutoStartupToggle.IsOn = false;
//          AutoStartupToggle.IsEnabled = false;
//          AutoStartupDescriptionText.Text
//            = "グループポリシーで無効にされています";
//          break;
//        case StartupTaskState.Enabled:
//          // トグル ON、変更可能
//          AutoStartupToggle.IsOn = true;
//          AutoStartupToggle.IsEnabled = true;
//          AutoStartupDescriptionText.Text = string.Empty;
//          break;
//        case StartupTaskState.EnabledByPolicy:
//          // トグル ON、変更不可
//          AutoStartupToggle.IsOn = true;
//          AutoStartupToggle.IsEnabled = false;
//          AutoStartupDescriptionText.Text
//            = "グループポリシーで有効にされています";
//          break;
//#if DEBUG
//        default:
//          throw new ArgumentOutOfRangeException($"想定外のstartupTask.State({startupTask.State})");
//#endif
//      }

//      //_isInSetupAutoStartup = false;
//      _isSetupCompleted = true;
//    }

  }
}
