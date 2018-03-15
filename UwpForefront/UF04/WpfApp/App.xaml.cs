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
    private static readonly Version _osVersion
      = (new Func<Version>(() => {
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

    [STAThread]
    public static void Main()
    {
      if (_osVersion.Major < 10)
      {
        MessageBox.Show("このアプリは、Windows 10 でなければ動作しません。",
          "動作不可", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      App app = new App();
      app.InitializeComponent();
      app.Run();
    }
  }
}
