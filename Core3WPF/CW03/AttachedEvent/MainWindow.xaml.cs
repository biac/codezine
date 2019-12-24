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

namespace AttachedEvent
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

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      // 注意! この場合、sender は Grid です。e.Source が、実際にクリックされた Button です。

      object content = (e.Source as Button)?.Content;
      switch (content)
      {
        case null:
        case string key when (key.Length == 0):
          return;

        // クリア キー
        case "C":
          ResultText.Text = "0";
          break;

        // 数字キー
        case string key when ('0' <= key[0] && key[0] <= '9'):
          if (ResultText.Text == "0")
          {
            if (key != "0")
              ResultText.Text = key;
          }
          else
            ResultText.Text += key;
          break;
      }
    }
  }
}
