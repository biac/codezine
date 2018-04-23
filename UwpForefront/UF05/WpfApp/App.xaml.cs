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

      string GetProtocolActivationUrl()
      {
        if (args.Length > 0
            && args[0].Contains("uf05.bluewatersoft.jp-timelinetest:")
            && Uri.TryCreate(args[0], UriKind.Absolute, out var uri))
          return uri.Query.Substring(1);
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
