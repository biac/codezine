using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BuiltinThemes
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    // 別のXAMLファイルで定義したリソースの型の配列
    // ※この配列の順に、テーマリソースの後ろに追加される。
    //   テーマリソースをBasedOnまたはStaticResourceで参照していないリソースは、
    //   この配列に登録せずにApp.xamlのプライマリーリソースディクショナリーに直書きでよい。
    private readonly Type[] ExternalResourceTypes = { typeof(SampleResource), };

    internal void ChangeTheme(ThemeResource theme)
    {
      var currentTheme = this.Resources.MergedDictionaries
                             .OfType<ThemeResourceDictionary>().FirstOrDefault();
      var newTheme = ThemeResourceDictionary.GetInstance(theme);
      if (newTheme == currentTheme)
        return;

      // テーマリソースの差し替え
      if (currentTheme != null)
        this.Resources.MergedDictionaries.Remove(currentTheme);

      if (newTheme != null)
        this.Resources.MergedDictionaries.Add(newTheme);


      // 外部リソースの差し替え
      // ※スタイルリソースのBasedOnはインスタンス化時に読み込まれるため、
      //   毎回インスタンス化する必要がある。
      foreach (var resType in ExternalResourceTypes)
      {
        var currentRes = this.Resources.MergedDictionaries.FirstOrDefault(dic => resType.IsInstanceOfType(dic));
        if (currentRes != null)
          this.Resources.MergedDictionaries.Remove(currentRes);

        var newRes = resType.GetConstructor(Type.EmptyTypes).Invoke(null) as ResourceDictionary;
        this.Resources.MergedDictionaries.Add(newRes);
      }
    }
  }
}
