using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core.Preview;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UF12
{
  /// <summary>
  /// 既定の Application クラスを補完するアプリケーション固有の動作を提供します。
  /// </summary>
  sealed partial class App : Application
  {
    /// <summary>
    /// 単一アプリケーション オブジェクトを初期化します。これは、実行される作成したコードの
    ///最初の行であるため、main() または WinMain() と論理的に等価です。
    /// </summary>
    public App()
    {
      this.InitializeComponent();
      this.Suspending += OnSuspending;
    }



    // 再起動時のパラメーターには、この文字列を先頭に付ける
    public const string RestartParamHeader = "$RestartParam$";
    public bool IsRestart { get; private set; }

    /// <summary>
    /// アプリケーションがエンド ユーザーによって正常に起動されたときに呼び出されます。他のエントリ ポイントは、
    /// アプリケーションが特定のファイルを開くために起動されたときなどに使用されます。
    /// </summary>
    /// <param name="e">起動の要求とプロセスの詳細を表示します。</param>
    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
      Frame rootFrame = Window.Current.Content as Frame;
      string argument = e.Arguments as string; // パラメーター文字列

      // ウィンドウに既にコンテンツが表示されている場合は、アプリケーションの初期化を繰り返さずに、
      // ウィンドウがアクティブであることだけを確認してください
      if (rootFrame == null)
      {
        // ナビゲーション コンテキストとして動作するフレームを作成し、最初のページに移動します
        rootFrame = new Frame();

        rootFrame.NavigationFailed += OnNavigationFailed;

        if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
        {
          //TODO: 以前中断したアプリケーションから状態を読み込みます


          if (!string.IsNullOrEmpty(argument)
              && argument.StartsWith(RestartParamHeader))
          {
            // 再起動された
            IsRestart = true;
            argument = argument.Replace(RestartParamHeader, string.Empty);
          }

        }

        // フレームを現在のウィンドウに配置します
        Window.Current.Content = rootFrame;
        this.SetupCloseRequestedHandler();

        if (e.PrelaunchActivated)
        {
          // プレランチされた時刻を記録
          PrelaunchTime = DateTimeOffset.Now;

          //// テスト：試しにプレランチ時に画面を出してみる
          ////         ⇒ エラーにはならないが、画面は出ない
          //if (rootFrame.Content == null)
          //{
          //  bool result = rootFrame.Navigate(typeof(MainPage), argument);
          //}
          //Window.Current.Activate();
        }
      }

      if (e.PrelaunchActivated == false)
      {
        // プレランチを「申請」します
        CoreApplication.EnablePrelaunch(true);

        if (rootFrame.Content == null)
        {
          // ナビゲーション スタックが復元されない場合は、最初のページに移動します。
          // このとき、必要な情報をナビゲーション パラメーターとして渡して、新しいページを
          //構成します
          rootFrame.Navigate(typeof(MainPage), argument);
        }


        // 現在のウィンドウがアクティブであることを確認します
        Window.Current.Activate();
      }
      //else // PrelaunchActivated
      //{
      //  //時刻を記録
      //  PrelaunchTime = DateTimeOffset.Now;
      //}
    }






    public DateTimeOffset? PrelaunchTime { get; set; }
    public bool IsAutoStartup { get; private set; }

    protected override void OnActivated(IActivatedEventArgs args)
    {
      Frame rootFrame = Window.Current.Content as Frame;
      if (rootFrame == null)
      {
        rootFrame = new Frame();
        rootFrame.NavigationFailed += OnNavigationFailed;
        Window.Current.Content = rootFrame;

        this.SetupCloseRequestedHandler();
      }

      string payload = string.Empty;
      if (args.Kind == ActivationKind.StartupTask)
      {
        // 自動起動された場合
        IsAutoStartup = true;
        var startupArgs = args as StartupTaskActivatedEventArgs;
        payload = startupArgs.TaskId; //ActivationKind.StartupTask.ToString();
      }
      else if (args.Kind == ActivationKind.Launch)
      {
        // 自動再起動時には、ここには来ない!!!
        throw new InvalidProgramException();
        //var launchArgs = args as LaunchActivatedEventArgs;
        ////string argString = launchArgs.Arguments;
        //if (args.PreviousExecutionState == ApplicationExecutionState.Terminated
        //    //&& !string.IsNullOrEmpty(argString))
        //    && launchArgs.Arguments is string argString
        //    && argString.StartsWith(RestartParamHeader))
        //{
        //  // 自動再起動
        //  IsRestart = true;
        //  payload = argString.Replace(RestartParamHeader, string.Empty);
        //}
      }

      rootFrame.Navigate(typeof(MainPage), payload);
      Window.Current.Activate();
    }



    /// <summary>
    /// 特定のページへの移動が失敗したときに呼び出されます
    /// </summary>
    /// <param name="sender">移動に失敗したフレーム</param>
    /// <param name="e">ナビゲーション エラーの詳細</param>
    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
      throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }

    /// <summary>
    /// アプリケーションの実行が中断されたときに呼び出されます。
    /// アプリケーションが終了されるか、メモリの内容がそのままで再開されるかに
    /// かかわらず、アプリケーションの状態が保存されます。
    /// </summary>
    /// <param name="sender">中断要求の送信元。</param>
    /// <param name="e">中断要求の詳細。</param>
    private void OnSuspending(object sender, SuspendingEventArgs e)
    {
      var deferral = e.SuspendingOperation.GetDeferral();
      //TODO: アプリケーションの状態を保存してバックグラウンドの動作があれば停止します
      deferral.Complete();
    }




    //void SetupCloseRequestedHandler()
    //{
    //  // [×]ボタンのイベントハンドラー (1703以降)
    //  // ※マニフェストに <rescap:Capability Name="confirmAppClose"/> が必要
    //  SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += async (s, e) =>
    //  {
    //    // 閉じられるのをキャンセルする
    //    e.Handled = true;

    //    ContentDialog dialog = new ContentDialog
    //    {
    //      Title = "アプリを終了しますか?",
    //      Content = "[×] ボタンが押されましたが、アプリを終了してもよいですか",
    //      CloseButtonText = "キャンセル",
    //      PrimaryButtonText = "終了",
    //      //SecondaryButtonText = "Try it",
    //      DefaultButton = ContentDialogButton.Primary
    //    };
    //    ContentDialogResult result = await dialog.ShowAsync();
    //    if (result == ContentDialogResult.Primary)
    //    {
    //      // キャンセルしてしまっているので、
    //      // 必要ならばあらためてアプリを終了させる。
    //      App.Current.Exit();
    //      // ↑Exitすると、OnSuspending等は実行されないので注意!
    //    }
    //  };
    //}

  }
}
