using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UwpApp.ValueConverters
{
  /// <summary>
  /// true を 1.0 に、および false を 0.0 に変換する値コンバーター。
  /// </summary>
  public sealed class Bool2Opacity : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      
      return (value is bool b && b) ? 1.0 : 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      return value is double d && d == 1.0;
    }
  }
}
