using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace BuiltinThemes
{
  public enum ThemeResource
  { 
    None,  // テーマ無し
    Classic,  // Windows 9x/2000 look
    LunaNormalColor,  // Default blue theme on Windows XP
    LunaHomestead,    // Olive theme on Windows XP
    LunaMetallic,     // Silver theme on Windows XP
    RoyalNormalColor, // Windows XP Media Center Edition
    AeroNormalColor,  // Windows Vista/Windows 7
    AeroLiteNormalColor,  // Windows 8 hidden theme
    Aero2NormalColor, // Windows 10
  }


  /// <summary>
  /// テーマリソースを他のリソースと識別するためのマーカークラス
  /// (標準のテーマリソースを作るヘルパーメソッド付き)
  /// </summary>
  public class ThemeResourceDictionary : ResourceDictionary
  {
    public ThemeResourceDictionary() : base(){ }

    private ThemeResourceDictionary(string relativeSourceUrl) 
      => this.Source = new Uri(relativeSourceUrl, uriKind: UriKind.Relative);

    private static readonly Dictionary<ThemeResource, string> ThemeUrlDictionary
      = new Dictionary<ThemeResource, string>
        {
          {ThemeResource.None, null },
          {ThemeResource.Classic,
            "/presentationframework.Classic;component/themes/classic.xaml" },
          {ThemeResource.LunaNormalColor,
            "/presentationframework.Luna;component/themes/luna.normalcolor.xaml" },
          {ThemeResource.LunaHomestead,
            "/presentationframework.Luna;component/themes/luna.homestead.xaml" },
          {ThemeResource.LunaMetallic,
            "/presentationframework.Luna;component/themes/luna.metallic.xaml" },
          {ThemeResource.RoyalNormalColor,
            "/presentationframework.Royale;component/themes/royale.normalcolor.xaml" },
          {ThemeResource.AeroNormalColor,
            "/presentationframework.Aero;component/themes/aero.normalcolor.xaml" },
          {ThemeResource.AeroLiteNormalColor,
            "/presentationframework.AeroLite;component/themes/aerolite.normalcolor.xaml" },
          {ThemeResource.Aero2NormalColor,
            "/presentationframework.Aero2;component/themes/aero2.normalcolor.xaml" },
        };

    private static ThemeResourceDictionary _classic;
    private static ThemeResourceDictionary _lunaNormal;
    private static ThemeResourceDictionary _lunaHomestead;
    private static ThemeResourceDictionary _lunaMetallic;
    private static ThemeResourceDictionary _royalNormal;
    private static ThemeResourceDictionary _aeroNormal;
    private static ThemeResourceDictionary _aeroLiteNormal;
    private static ThemeResourceDictionary _aero2Normal;


    public static ThemeResourceDictionary GetInstance(ThemeResource theme)
      => theme switch
      {
        ThemeResource.None => null,
        ThemeResource.Classic
          => _classic ??= new ThemeResourceDictionary(ThemeUrlDictionary[theme]),
        ThemeResource.LunaNormalColor
          => _lunaNormal ??= new ThemeResourceDictionary(ThemeUrlDictionary[theme]),
        ThemeResource.LunaHomestead
          => _lunaHomestead ??= new ThemeResourceDictionary(ThemeUrlDictionary[theme]),
        ThemeResource.LunaMetallic
          => _lunaMetallic ??= new ThemeResourceDictionary(ThemeUrlDictionary[theme]),
        ThemeResource.RoyalNormalColor
          => _royalNormal ??= new ThemeResourceDictionary(ThemeUrlDictionary[theme]),
        ThemeResource.AeroNormalColor
          => _aeroNormal ??= new ThemeResourceDictionary(ThemeUrlDictionary[theme]),
        ThemeResource.AeroLiteNormalColor
          => _aeroLiteNormal ??= new ThemeResourceDictionary(ThemeUrlDictionary[theme]),
        ThemeResource.Aero2NormalColor
          => _aero2Normal ??= new ThemeResourceDictionary(ThemeUrlDictionary[theme]),

        _ => throw new InvalidOperationException()
      };
  }
}
