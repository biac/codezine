using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SampleNamespace
{
  public class Rating : TextBlock
  {
    const int MaxStars = 5;
    const double DefaultFontSize = 30.0;
    static readonly SolidColorBrush DefaultBrush = new SolidColorBrush(Colors.DarkOrange);

    public Rating() : base()
    {
      base.FontSize = DefaultFontSize;
      base.Foreground = DefaultBrush;
      SetStars(0);
    }

    public static readonly DependencyProperty StarsProperty =
    DependencyProperty.Register(
      "Stars",
      typeof(int),
      typeof(Rating),
      new FrameworkPropertyMetadata(defaultValue:0,
        (o, e) => (o as Rating).SetStars(Math.Min((int)e.NewValue, MaxStars))
      )
    );
    [TypeConverter(typeof(Int32Converter))]
    public int Stars
    {
      get { return (int)GetValue(StarsProperty); }
      set { SetValue(StarsProperty, value); }
    }

    private void SetStars(int stars)
    {
      base.Text = $"{new string('★', stars)}{new string('☆', MaxStars - stars)}";
    }
  }
}
