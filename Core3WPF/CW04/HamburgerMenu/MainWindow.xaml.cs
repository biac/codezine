using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HamburgerMenu
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

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var selectedItem = e.AddedItems.OfType<ListBoxItem>().FirstOrDefault();
      if (selectedItem == null)
        return;

      switch ((sender as ListBox).SelectedIndex)
      {
        case 0:
          MessageBox.Show(this, $@"""{selectedItem.Content}"" が選択されました。", "ハンバーガーメニュー", MessageBoxButton.OK, MessageBoxImage.Information);
          break;
        case 1:
          ShowPopup($@"""{selectedItem.Content}"" が選択されました。", "ハンバーガーメニュー");
          break;
        case 2:
          ShowMessageGrid($@"""{selectedItem.Content}"" が選択されました。", "ハンバーガーメニュー");
          break;
      }
    }

    // Popup を表示する
    private void ShowPopup(string message, string title)
    {
      PopupTitle.Text = title;
      PopupBody.Text = message;
      Popup1.IsOpen = true;
      MainGrid.IsEnabled = false;
    }
    // Popup を非表示にする
    private void HidePopup()
    {
      MainGrid.IsEnabled = true;
      Popup1.IsOpen = false;
    }

    // ウィンドウ内のメッセージボックスを表示する
    private void ShowMessageGrid(string message, string title)
    {
      MessageTitle.Text = title;
      MessageBody.Text = message;
      MessagePanel.Visibility = Visibility.Visible;

      //// 色を作るときにα値を指定する例：
      //// 不透明度が約66%（＝0xAA÷0xFF）の赤色
      //MessagePanel.Background = new SolidColorBrush(Color.FromArgb(0xAA, 0xFF, 0x00, 0x00));

      //// 色のα値を変更する例：
      //// 不透明度が約50%（＝0x80÷0xFF）の緑色
      //Color green = Colors.Green;
      //green.A = 0x80;
      //MessagePanel.Background = new SolidColorBrush(green);

      //// BrushオブジェクトのOpacityプロパティを設定する例：
      //// 不透明度が33%の水色
      //Brush cyanBrush = new SolidColorBrush(Colors.Cyan);
      //cyanBrush.Opacity = 0.33;
      //MessagePanel.Background = cyanBrush;
    }

    // ウィンドウ内のメッセージボックスを非表示にする
    private void HideMessageGrid()
    {
      MessagePanel.Visibility = Visibility.Hidden;
    }


    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
      HidePopup();
      HideMessageGrid();
    }
  }
}
