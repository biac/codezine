using System;
using System.ServiceModel;
using System.Windows;


// プロセス間通信 (WCF 利用)
// System.ServiceModel アセンブリへの参照追加が必要

namespace WpfApp
{
  // サービスの定義
  [ServiceContract]
  public interface INavigationService
  {
    [OperationContract]
    void Navigate(string url);
  }

  class IpcService
  {
    public const string HostAddress = "net.pipe://localhost/UF05";
    public const string Endpoint = "NavigationService";
  }



  // サーバー側
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
  public class IpcServer : INavigationService
  {
    // サービスの実装
    public void Navigate(string url)
    {
      if (App.Current.MainWindow is MainWindow mainWindow)
      {
        // ウィンドウを最前面に持ってくる
        if (mainWindow.WindowState == WindowState.Minimized)
          mainWindow.WindowState = WindowState.Normal;
        mainWindow.Activate();
        mainWindow.Topmost = true; 
        mainWindow.Topmost = false;
        mainWindow.Focus();

        // URL の Web ページを表示
        if (!string.IsNullOrWhiteSpace(url))
          mainWindow.Navigate(url);
      }
    }

    // サーバーのインスタンスを保持する静的メンバー変数
    private static ServiceHost _host;

    public static void StartService()
    {
      if (_host != null)
        _host.Close();

      _host = new ServiceHost(new IpcServer(),
                              new Uri(IpcService.HostAddress));
      _host.AddServiceEndpoint(typeof(INavigationService),
                               new NetNamedPipeBinding(),
                              IpcService.Endpoint);
      _host.Open();
    }
  }



  // クライアント側
  public class IpcClient
  {
    public static bool RequestNavigation(string url)
    {
      try
      {
        var channelFactory
          = new ChannelFactory<INavigationService>(
              new NetNamedPipeBinding(),
              new EndpointAddress($"{IpcService.HostAddress}/{IpcService.Endpoint}"));
        var navigationService = channelFactory.CreateChannel();

        // サービスを呼び出し
        navigationService.Navigate(url);
        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}
