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
    private void OnStartup(object sender, StartupEventArgs e)
    {
      MainWindow mainWindow = new MainWindow();
      mainWindow.Show();

      //二重起動をチェックする
      if (System.Diagnostics.Process.GetProcessesByName(
          System.Diagnostics.Process.GetCurrentProcess().ProcessName).Length > 1)
      {
        //すでに起動していると判断する
        //MessageBox.Show("多重起動はできません。");
      }


      if (e.Args.Length > 0
          && e.Args[0].Contains("uf05.bluewatersoft.jp-timelinetest:"))
        {
          if (Uri.TryCreate(e.Args[0], UriKind.Absolute, out var uri))
          {
            string url = uri.Query.Substring(1);
            mainWindow.Navigate(url);
          }

          // ↑これだと、どんどん新しいプロセスが起動してしまう

          // 既存プロセスが走ってたら MainWindow を捕まえて、url を表示
          // 既存プロセスが走ってないなら MainWindow を作って、url を表示

      }
      //else
      //{
      //  MainWindow mainWindow = new MainWindow();
      //  mainWindow.Show();
      //}

    }
  }
}
