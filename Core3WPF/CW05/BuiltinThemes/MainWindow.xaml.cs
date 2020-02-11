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

namespace BuiltinThemes
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    // テーマ選択リストビューに与えるデータ
    private class ListItem
    {
      public ThemeResource Theme { get; set; }
      public string Description { get; set; }

      public ListItem(ThemeResource res, string desc)
      {
        Theme = res;
        Description = desc;
      }
    }


    public MainWindow()
    {
      InitializeComponent();

      PrepareThemeListView();
    }


    private void PrepareThemeListView()
    {
      var list = new List<ListItem>();
      list.Add(new ListItem(ThemeResource.None, "default"));
      list.Add(new ListItem(ThemeResource.Classic, "classic (Win2k)"));
      list.Add(new ListItem(ThemeResource.LunaNormalColor, "luna.normalcolor (Xp default)"));
      list.Add(new ListItem(ThemeResource.LunaHomestead, "luna.homestead (Xp olive)"));
      list.Add(new ListItem(ThemeResource.LunaMetallic, "luna.metallic (Xp silver)"));
      list.Add(new ListItem(ThemeResource.RoyalNormalColor, "royale.normalcolor (Xp media center)"));
      list.Add(new ListItem(ThemeResource.AeroNormalColor, "aero.normalcolor (Vista/Win7)"));
      list.Add(new ListItem(ThemeResource.AeroLiteNormalColor, "aerolite.normalcolor (Win8 hidden)"));
      list.Add(new ListItem(ThemeResource.Aero2NormalColor, "aero2.normalcolor (Win10)"));

      ThemeListView.ItemsSource = list;
      ThemeListView.SelectedIndex = 0;
    }

    private void ThemeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var selectedItem = e.AddedItems[0] as ListItem;
      (App.Current as App).ChangeTheme(selectedItem.Theme);
    }
  }
}
