using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
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

namespace MaterialDesign
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
  {
    private BundledTheme _currentMaterialDesignTheme;

    public MainWindow()
    {
      InitializeComponent();

      _currentMaterialDesignTheme = Application.Current.Resources.MergedDictionaries
                                      .OfType<BundledTheme>().FirstOrDefault();

      PrimaryColorListView.ItemsSource
        = Enum.GetNames(typeof(MaterialDesignColors.PrimaryColor)).OrderBy(s => s);
      SecondaryColorListView.ItemsSource
        = Enum.GetNames(typeof(MaterialDesignColors.SecondaryColor)).OrderBy(s => s);

      PrimaryColorListView.SelectedItem = "LightBlue";
      SecondaryColorListView.SelectedItem = "Indigo";
    }

    private void PrimaryColorListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems[0] is string s)
        _currentMaterialDesignTheme.PrimaryColor = (PrimaryColor)Enum.Parse(typeof(PrimaryColor), s);
    }

    private void SecondaryColorListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems[0] is string s)
        _currentMaterialDesignTheme.SecondaryColor = (SecondaryColor)Enum.Parse(typeof(SecondaryColor), s);
    }

    private void DarkToggleButton_Checked(object sender, RoutedEventArgs e)
      => _currentMaterialDesignTheme.BaseTheme = BaseTheme.Dark;
  
    private void DarkToggleButton_Unchecked(object sender, RoutedEventArgs e)
      => _currentMaterialDesignTheme.BaseTheme = BaseTheme.Light;
  }
}
