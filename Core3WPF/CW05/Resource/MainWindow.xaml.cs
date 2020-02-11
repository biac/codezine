using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Resource
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();


      // リソースディクショナリにオブジェクトを追加／変更する
      this.Resources["Uri"] = new Uri("https://codezine.jp/article/corner/805");
      // ↓追加のときは次のように書いてもよい（ただし、キーが重複するとエラー）
      //this.Resources.Add("Uri", new Uri("https://codezine.jp/article/corner/805"));

      // ※↑リソースを変更しても、StaticResourceで参照している場合は反映されない

      // リソースディクショナリからオブジェクトを取り出して使う
      this.WebView1.Source = this.Resources["Uri"] as Uri;

      // リソースを検索する
      // ※ App.xamlに
      //    <system:String x:Key="TitleString">XAMLリソースの例</system:String>
      //    とリソースが定義してある
      if (this.TryFindResource("TitleString") is string title)
      {
        // リソース TitleString を使って何かする
        this.Title = $".NET Core 3で始めるWPFアプリ開発 - {title}";
      }
    }
  }
}
