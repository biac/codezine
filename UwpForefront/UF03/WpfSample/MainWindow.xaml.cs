using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace WpfSample
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      // SQL Server からデータを取得
      DataTable dt = UF03StdLib.Northwind.GetCategories(SqlClientFactory.Instance);

      DataGrid1.ItemsSource = dt.Rows;
    }

    // SqlClientFactory が実装されているアセンブリ
    #region アセンブリ System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    // C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7\System.Data.dll
    #endregion
  }
}
