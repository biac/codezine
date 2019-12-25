using System.Windows;
using System.Windows.Controls;

namespace AttachedProperty
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

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      var menuItem = e.Source as MenuItem;
      MessageBox.Show($"{menuItem.Header as string} が選択されました",
                      "Context Menu");
    }
  }
}
