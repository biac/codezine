using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp
{
  /// <summary>
  /// App.xaml の相互作用ロジック
  /// </summary>
  public partial class App : Application
  {
    // UWP 上で動作しているか?
    // ※ NuGet パッケージ DesktopBridge.Helpers が必要
    private static bool _isRunningAsUwp { get; }
      = (new DesktopBridge.Helpers()).IsRunningAsUwp();

    // Windows のバージョン
    // ※ System.Management アセンブリへの参照追加が必要
    private static Version _osVersion { get; }
      = (new Func<Version>(() =>
      {
        using (var mc = new System.Management.ManagementClass("Win32_OperatingSystem"))
        using (var moc = mc.GetInstances())
          foreach (System.Management.ManagementObject mo in moc)
          {
            var v = mo["Version"] as string;
            if (!string.IsNullOrWhiteSpace(v))
              return new Version(v);
          }
        return new Version("0.0.0.0");
      }))();

    // 「タイムライン」が利用可能か?
    // 「タイムライン」 API が使えるのは、Win10 1709 (16299) 以降
    // 「タイムライン」から呼び出してもらえるのは、今回は UWP 上での動作時のみ
    public static bool IsTimelineAvailable { get; }
      = _isRunningAsUwp && (_osVersion >= new Version("10.0.16299.0"));



    private static string _protocolActivationUrl;

    // http://www.atmarkit.co.jp/ait/articles/1511/04/news027.html
    [STAThread]
    public static void Main(string[] args)
    {
      _protocolActivationUrl = GetProtocolActivationUrl();
      if (IpcClient.RequestNavigation(_protocolActivationUrl))
      {
        // サーバーとの通信に成功した == 二重起動である
        // 二重起動の場合は、アプリを終了する
        return;
      }

      App app = new App();
      app.InitializeComponent();
      app.Run();

      return; // 以下はローカル関数のみ

      string GetProtocolActivationUrl()
      {
        if (args.Length > 0
            && args[0].StartsWith("uf05.bluewatersoft.jp-timelinetest:")
            && Uri.TryCreate(args[0], UriKind.Absolute, out var uri))
          return uri.Query.Substring(1);
        else
          return null;
      }
    }


    private void OnStartup(object sender, StartupEventArgs e)
    {
      MainWindow mainWindow = new MainWindow();
      mainWindow.Show();
      if (_protocolActivationUrl != null)
        mainWindow.Navigate(_protocolActivationUrl);
    }
  }
}
