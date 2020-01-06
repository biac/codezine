using System.Windows;
using System.Windows.Controls;

namespace Example
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    const double FontSizeNormal = 12.0;
    const double FontSizeLarge = 24.0;

    public MainWindow()
    {
      InitializeComponent();

      // ↓コードビハインドから初期フォーカスを設定する例
      //TextBox2.Focus();
    }

    private void LargeFontCheckBox_Changed(object sender, RoutedEventArgs e)
    {
      if ((sender as CheckBox)?.IsChecked == true)
        this.FontSize = FontSizeLarge;
      else
        this.FontSize = FontSizeNormal;
    }

  }
}
