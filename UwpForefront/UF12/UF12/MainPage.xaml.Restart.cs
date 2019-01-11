using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.UI.Xaml;

namespace UF12
{
  partial class MainPage
  {
    // アプリの再起動

    // How to Restart your App Programmatically - Windows Developer Blog (2017/07/28)
    // https://blogs.windows.com/buildingapps/2017/07/28/restart-app-programmatically/


    private async void RestartButton_Click(object sender, RoutedEventArgs e)
    {
      RestartButton.IsEnabled = false;
      RestartDescriptionText.Text = "";

      using (var newSession = new ExtendedExecutionSession())
      {
        newSession.Reason = ExtendedExecutionReason.Unspecified;
        await newSession.RequestExtensionAsync();
        // ↑最小化されたときにサスペンドされるのを延期する（バックグラウンド実行）
        //   ※バッテリー駆動時は最大10分まで


        await Task.Delay(3000); // 動作確認のため、しばらく遅らせる

        // 再起動を要求する
        string param = App.RestartParamHeader + ParamText.Text;
        AppRestartFailureReason result = await CoreApplication.RequestRestartAsync(param);

        switch (result)
        {
          case AppRestartFailureReason.RestartPending:
            // 正常に再起動処理が始まった
            RestartDescriptionText.Text = "再起動中…";//この表示は見えない
            break;
          case AppRestartFailureReason.NotInForeground:
            // 失敗
            RestartDescriptionText.Text
              = "再起動失敗：アプリがフォアグラウンドになっていません。";
            break;
          //case AppRestartFailureReason.InvalidUser:
          //  // 失敗（引数にユーザーを指定した場合に発生しうる）
          //  RestartDescriptionText.Text
          //    = "AppRestartFailureReason.InvalidUser";
          //  break;
          case AppRestartFailureReason.Other:
            // 失敗
            RestartDescriptionText.Text
              = "再起動失敗：予期しない理由です。";
            break;

#if DEBUG
          default:
            throw new ArgumentOutOfRangeException($"想定外のAppRestartFailureReason({result})");
#endif
        }
      }

      RestartButton.IsEnabled = true;
    }
  }
}
