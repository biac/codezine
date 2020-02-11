using System.Windows;

namespace ResourceDictionaryFile
{
  // ※ ResourceDictionary2.xaml の先頭タグに次の1行を追加する
  //    x:Class="ResourceDictionaryFile.ResourceDictionary2"


  partial class ResourceDictionary2 : ResourceDictionary
  {
    public ResourceDictionary2()
      => InitializeComponent();

    // ※ ResourceDictionary は IComponentConnector を実装している。
    //    InitializeComponent メソッドは、IComponentConnector のメンバー
  }
}
