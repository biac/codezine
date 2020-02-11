using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace StaticVSDynamic
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      if (SystemParameters.IsGlassEnabled)
      {
        Resources["WindowGlassBrush"] = SystemParameters.WindowGlassBrush;
        SystemParameters.StaticPropertyChanged += (s, e) =>
        {
          if (e.PropertyName == "WindowGlassBrush")
            Resources["WindowGlassBrush"] = SystemParameters.WindowGlassBrush;
        };
      }
      else {
        Resources["WindowGlassBrush"] = SystemColors.MenuHighlightBrush;
      }
    }
  }
}
