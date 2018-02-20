using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WinFormSample
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();

      // SQL Server からデータを取得
      DataTable dt = UF03StdLib.Northwind.GetCategories(SqlClientFactory.Instance);

      dataGridView1.DataSource = dt;
    }

    // SqlClientFactory が実装されているアセンブリ
    #region アセンブリ System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    // C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7\System.Data.dll
    #endregion

  }
}
