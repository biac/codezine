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
      if (selectedItem != null)
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
