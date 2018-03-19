using System.Data;
using System.Data.Common;

namespace UF03StdLib
{
  public class Northwind
  {
    // SQL ユーザー認証用の接続文字列
    private const string CONN_STR_SQLSERVER
      = @"Data Source=Win10VM-RS3.corp.BluewaterSoft.jp\SQLEXPRESS;"
        + "Initial Catalog=NORTHWIND;User ID=nwtestuser;Password=";
    // ※ Android / iOS では DNS による名前解決ができる必要がある。
    //    Windows は、NetBIOS による名前解決でも OK のようだ。
    //    名前解決できないときは、ホスト指定を IP アドレスにする（例☟）。
    //= @"Data Source=192.168.0.12\SQLEXPRESS;Initial Catalog=NORTHWIND;User ID=nwtestuser;Password=";
    //    なお、Windows ではローカルデータベースも使える☟ (Android / iOS は未確認)。
    // = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={AppContext.BaseDirectory}Database\northwind.mdf;Integrated Security=True;Connect Timeout=3";

    // Windows 認証用の接続文字列
    private const string CONN_STR_ENTERPRISE
      = @"Data Source=Win10VM-RS3.corp.BluewaterSoft.jp\SQLEXPRESS;"
        + "Initial Catalog=NORTHWIND;Integrated Security=SSPI";
    // ※ UWP は Windows 認証 (Active Directory によるユーザー認証) しか使えない。
    //    SQL Server は、ドメインコントローラー以外のドメイン参加 PC 上で動いていること。
    //    また、アプリを動かす PC は同じドメインに参加していて、
    //    ユーザーはドメインにログオンしていること。
    //    なお、UWP は (デバッグ時を除いて) ループバック接続が不許可なので要注意。



    // SQL Server からデータを取得
    public static DataTable GetCategories(DbProviderFactory factory, bool useEnterpriseAuthentication = false)
    {
      // DbConnection / DbCommand / DbDataAdapter などの実装はプラットフォーム依存。
      // (.NET Standard 2.0 に SqlClientFactory は入っていない)
      // なので、このメソッドの引数 factory として SqlClientFactory を渡してもらう。

      // DbProviderFactory (抽象クラス) が定義されているアセンブリ
      #region アセンブリ netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
      // C:\Users\{USER NAME}\.nuget\packages\netstandard.library\2.0.1\build\netstandard2.0\ref\netstandard.dll
      #endregion

      // ADO.NET によるデータ取得例
      // ここでは DbDataAdapter によるデータ取得しかしていないが、
      // もちろんデータの更新・削除やトランザクションなども可能。
      using (DbConnection conn = factory.CreateConnection())
      {
        conn.ConnectionString = useEnterpriseAuthentication ?
                                CONN_STR_ENTERPRISE : CONN_STR_SQLSERVER;
        
        using (DbCommand command = factory.CreateCommand())
        {
          command.Connection = conn;
          command.CommandText = "select * from Categories order by CategoryID asc";

          using (DbDataAdapter da = factory.CreateDataAdapter())
          {
            da.SelectCommand = command;

            var dt = new DataTable("Categories");
            da.Fill(dt);
            return dt;
          }
        }
      }
    }
  }
}
