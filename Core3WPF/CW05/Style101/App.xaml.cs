using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Style101
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      //  <Application.Resources>
      //    <ResourceDictionary>
      //      <Style TargetType="Button">
      //        <Setter Property="Background" Value="MistyRose" />
      //      </Style>
      //    </ResourceDictionary>
      //  </Application.Resources>
      // ↑上記の XAML の定義を C# のコードで書いてみる↓
      //var setter = new Setter(
      //                property: Control.BackgroundProperty,
      //                value: new SolidColorBrush(Colors.MistyRose)
      //              );
      //var style = new Style(targetType: typeof(Button));
      //style.Setters.Add(setter);
      //var resource = new ResourceDictionary();
      //// C#で書くときは、明示的にキーを指定しなければならない
      //resource.Add(key: typeof(Button), value: style);
      //this.Resources = resource;
    }
  }
}
