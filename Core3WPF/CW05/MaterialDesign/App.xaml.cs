using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MaterialDesign
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      //var m = new BundledTheme() {
      //  BaseTheme = BaseTheme.Dark,
      //  PrimaryColor = MaterialDesignColors.PrimaryColor.Blue,
      //  SecondaryColor = MaterialDesignColors.SecondaryColor.Red,
      //};
      //this.Resources.MergedDictionaries.Add(m);

      //var n = new CustomColorTheme() {
      //  BaseTheme = BaseTheme.Dark,
      //  PrimaryColor = Colors.SkyBlue,
      //SecondaryColor = Colors.Crimson,
      //};
      //this.Resources.MergedDictionaries.Add(n);
    }
  }
}
