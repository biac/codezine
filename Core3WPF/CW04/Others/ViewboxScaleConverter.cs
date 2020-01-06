using System;
using System.Globalization;
using System.Windows.Data;

namespace Others
{
  internal class ViewboxScaleConverter : IMultiValueConverter
  {
    /// <summary>
    /// Viewbox コントロールとその中に置いたコンテンツから、 表示しているスケールを計算します。 スケールを表す double 型の数値を返します。
    /// </summary>
    /// <param name="values">
    /// 4つの長さをバインドします。
    /// 1: Viewbox の ActualWidth、 2: Viewbox の ActualHeight、
    /// 3: コンテンツの ActualWidth、 4: コンテンツの ActualHeight、
    /// </param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values[0] is double viewboxWidth
          && values[1] is double viewboxHeight
          && values[2] is double contentWidth
          && values[3] is double contentHeight)
      {
        if (contentWidth == 0.0 || contentHeight == 0.0)
          return double.NaN;

        var xScale = viewboxWidth / contentWidth;
        var yScale = viewboxHeight / contentHeight;
        return Math.Min(xScale, yScale);
      }
      return double.NaN;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
