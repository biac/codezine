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

namespace EventSetter
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    // ［標準ボタン03］がクリックされたときのイベントハンドラー（Clickプロパティで設定）
    private void Button3_Click(object sender, RoutedEventArgs e)
    {
      // メッセージボックスを表示する
      MessageBox.Show("通常のイベントハンドラー", "Button3_Click");
    }

    // ［標準ボタン01～05］がクリックされたときのイベントハンドラー（スタイルで設定）
    private void StandardButton_Click(object sender, RoutedEventArgs e)
    {
      // e.SourceはクリックされたButtonコントロール
      var button = e.Source as Button;
      var buttonContent = button.Content as string;
      MessageBox.Show($"{buttonContent}", "StandardButton_Click",
                      MessageBoxButton.OK, MessageBoxImage.Information);

      // ※ボタンごとの処理に分岐するには、button.Nameやbutton.Tagなどを利用する
    }

    // ［特別なボタン］がクリックされたときのイベントハンドラー（スタイルで設定）
    private void SpecialButton_Click(object sender, RoutedEventArgs e)
    {
      var result = MessageBox.Show(
@"特別なボタンがクリックされました。
ここでキャンセルすると後続のイベントハンドラーが実行されます。",
                     "SpecialButton_Click",
                     MessageBoxButton.OKCancel, MessageBoxImage.Exclamation
                   );

      // OKボタンがクリックされたときは、後続のイベントハンドラーを止める
      e.Handled = (result == MessageBoxResult.OK);
    }
  }
}
