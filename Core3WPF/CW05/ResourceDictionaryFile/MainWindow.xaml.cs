using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ResourceDictionaryFile
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      //// コードからリソースディクショナリーファイルを読み込む方法
      //// 一般的にはこのようなコードを書く。
      //// ※URIの書き方は、次を参照
      ////   https://docs.microsoft.com/ja-jp/dotnet/framework/wpf/app-development/pack-uris-in-wpf?redirectedfrom=MSDN#resource-file-pack-uris
      //// ※ここで App のリソースを ResourceDictionary1.xaml だけに置き換えるので、
      ////   実行すると ResourceDictionary1.xaml のリソースだけが適用される。
      //Uri resourceLocater = new Uri("pack://application:,,,/ResourceDictionary1.xaml");
      //// ↑↓どちらで書いてもよい
      Uri resourceLocater = new Uri("/ResourceDictionary1.xaml", UriKind.Relative);
      var resource1 = new ResourceDictionary() { Source = resourceLocater };
      App.Current.Resources = resource1;

      //// リソースディクショナリーファイルに細工をしておくと、new できる
      //// ※ここで App のリソースを置き換えるので、実行すると
      ////   ResourceDictionary2.xaml がプライマリーディクショナリーになり、
      ////   ResourceDictionary1.xaml がマージドディクショナリーに入る。
      ////   (Brush1 リソースの定義は ResourceDictionary1.xaml だけにある)
      //var resource2 = new ResourceDictionary2();
      //App.Current.Resources = resource2;
      //App.Current.Resources.MergedDictionaries.Add(resource1);


      InitializeComponent();

      // 一番下までスクロールさせる
      ScrollViewer1.ScrollToVerticalOffset(double.MaxValue);
    }
  }
}
