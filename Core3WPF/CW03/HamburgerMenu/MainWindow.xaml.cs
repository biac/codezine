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
        MessageBox.Show(this, $@"""{selectedItem.Content}"" が選択されました。", "ハンバーガーメニュー",MessageBoxButton.OK, MessageBoxImage.Information);
    }
  }
}
