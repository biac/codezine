using System.Data.Common;

[assembly: Xamarin.Forms.Dependency(typeof(XamarinFormsSample.UWP.SqlClientFactoryDS))]
namespace XamarinFormsSample.UWP
{
  public class SqlClientFactoryDS : ISqlClientFactoryDS
  {
    public DbProviderFactory Instance 
      => System.Data.SqlClient.SqlClientFactory.Instance;

    // SqlClientFactory が実装されているアセンブリ
    #region アセンブリ System.Data.SqlClient, Version=4.3.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
    // C:\Users\{USER NAME}\.nuget\packages\microsoft.netcore.universalwindowsplatform\6.0.7\ref\uap10.0.15138\System.Data.SqlClient.dll
    #endregion

  }
}
