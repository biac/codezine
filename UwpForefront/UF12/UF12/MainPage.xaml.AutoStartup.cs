using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UF12
{
  partial class MainPage
  {
    // アプリの自動起動

    // Configure your app to start at log-in - Windows Developer Blog (2017/08/01)
    // https://blogs.windows.com/buildingapps/2017/08/01/configure-app-start-log/

    // StartupTask を使うには、Package.appxmanifest に windows.startupTask の宣言が必要

    // マニフェストで指定したTaskId
    const string StartUpTaskId = "UF12StartupId";

    bool _isSetupCompleted; // ToggleSwitchの設定が完了したかどうかのフラグ

    // ToggleSwitchの状態を設定する
    private async void SetupAutoStartupToggle()
    {
      _isSetupCompleted = false;

      // StartupTaskオブジェクトを得る
      StartupTask startupTask = await StartupTask.GetAsync(StartUpTaskId);

      // StartupTaskの状態に応じてToggleSwitchの状態を設定する
      switch (startupTask.State)
      {
        case StartupTaskState.Disabled:
          // トグル OFF、変更可能
          AutoStartupToggle.IsOn = false;
          AutoStartupToggle.IsEnabled = true;
          AutoStartupDescriptionText.Text = string.Empty;
          break;
        case StartupTaskState.DisabledByUser:
          // トグル OFF、変更不可
          AutoStartupToggle.IsOn = false;
          AutoStartupToggle.IsEnabled = false;
          AutoStartupDescriptionText.Text
            = "タスクマネージャーのスタートアップタブで有効にできます";
          break;
        case StartupTaskState.DisabledByPolicy:
          // トグル OFF、変更不可
          AutoStartupToggle.IsOn = false;
          AutoStartupToggle.IsEnabled = false;
          AutoStartupDescriptionText.Text
            = "グループポリシーで無効にされています";
          break;
        case StartupTaskState.Enabled:
          // トグル ON、変更可能
          AutoStartupToggle.IsOn = true;
          AutoStartupToggle.IsEnabled = true;
          AutoStartupDescriptionText.Text = string.Empty;
          break;
        case StartupTaskState.EnabledByPolicy:
          // トグル ON、変更不可
          AutoStartupToggle.IsOn = true;
          AutoStartupToggle.IsEnabled = false;
          AutoStartupDescriptionText.Text
            = "グループポリシーで有効にされています";
          break;
#if DEBUG
        default:
          throw new ArgumentOutOfRangeException($"想定外のstartupTask.State({startupTask.State})");
#endif
      }

      _isSetupCompleted = true;
    }

    // ToggleSwitchを切り替えたときのイベントハンドラー
    private async void AutoStartupToggle_Toggled(object sender, RoutedEventArgs e)
    {
      if (!_isSetupCompleted)
        return; // 初期化中にコードからトグルを切り替えた

      // StartupTaskオブジェクトを得る
      StartupTask startupTask = await StartupTask.GetAsync(StartUpTaskId);

      if ((sender as ToggleSwitch).IsOn)
      {
        // 自動起動を要求する
        StartupTaskState state = await startupTask.RequestEnableAsync();
        // 返されたstateを見て、実際に自動起動が有効になったかどうか判定できる
      }
      else
      {
        // 自動起動をOFFにする
        startupTask.Disable();
      }

      // 実際の状態がトグルと異なっているかもしれないので、再度初期化する
      SetupAutoStartupToggle();
    }
  }
}
