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

namespace XamlAndCode
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      var button2 = new Button // ボタンのインスタンス化
        {
          // プロパティを設定
          Content = "C#で組み立てたボタン",
          Margin = new Thickness(  0.0, 0.0,  0.0, 5.0),
          Padding = new Thickness(10.0, 5.0, 10.0, 5.0),
        };
      button2.Click += Button2_Click; // イベントハンドラーを設定
      Grid1.Children.Add(button2); // Gridの中に配置
      Grid.SetRow(button2, 1); // Grid内で配置するRowを指定
    }

    private void Button1_Click(object sender, RoutedEventArgs e)
     => Text1.Text = "XAML";

    private void Button2_Click(object sender, RoutedEventArgs e)
     => Text1.Text = "C#";
  }
}
