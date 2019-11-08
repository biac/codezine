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

namespace CodeBehind
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      // TextBlockの前景色（＝文字色）と、水平／垂直位置
      Text1.Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0xA2, 0xE8));
      Text1.HorizontalAlignment = HorizontalAlignment.Center;
      Text1.VerticalAlignment = VerticalAlignment.Center;

      // ウィンドウのタイトルを変更
      this.Title += " - .NET CoreでWPF";
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Text1.Text = $"クリックしたのは{DateTimeOffset.Now:HH:mm}";
    }
  }
}
