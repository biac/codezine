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
  /// true を <see cref="Visibility.Visible"/> に、および false を 
  /// <see cref="Visibility.Collapsed"/> に変換する値コンバーター。
  /// </summary>
  public sealed class Bool2Visibility : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      return (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      return value is Visibility v && v == Visibility.Visible;
    }
  }
}
