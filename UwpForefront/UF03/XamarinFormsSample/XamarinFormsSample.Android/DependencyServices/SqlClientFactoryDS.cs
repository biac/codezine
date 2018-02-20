using System.Data.Common;

[assembly: Xamarin.Forms.Dependency(typeof(XamarinFormsSample.Droid.SqlClientFactoryDS))]
namespace XamarinFormsSample.Droid
{
  public class SqlClientFactoryDS : ISqlClientFactoryDS
  {
    public DbProviderFactory Instance
      => System.Data.SqlClient.SqlClientFactory.Instance;

    // SqlClientFactory が実装されているアセンブリ
    #region アセンブリ System.Data, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    // C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v1.0\System.Data.dll
    #endregion

  }
}