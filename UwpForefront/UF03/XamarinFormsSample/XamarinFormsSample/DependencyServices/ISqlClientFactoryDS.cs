using System.Data.Common;

namespace XamarinFormsSample
{
  public interface ISqlClientFactoryDS // DS = Dependency Service
  {
    DbProviderFactory Instance { get; }
  }
}