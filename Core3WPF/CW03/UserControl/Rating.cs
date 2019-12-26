using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SampleNamespace
{
  public class Rating : System.Windows.Controls.UserControl
  {
    const int MaxStars = 5;
    const double DefaultFontSize = 30.0;
    static readonly SolidColorBrush DefaultBrush = new SolidColorBrush(Colors.DarkOrange);

    TextBlock _textBlock = new TextBlock();

    public Rating()
    {
      BuildUp();
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
      get => (int)GetValue(StarsProperty);
      set
      {
        if (Stars != value)
          SetValue(StarsProperty, value);
      }
    }

    private void SetStars(int stars)
    {
      _textBlock.Text = $"{new string('★', stars)}{new string('☆', MaxStars - stars)}";
    }

    private void BuildUp()
    {
      SetStars(0);
      this.FontSize = DefaultFontSize;
      this.Foreground = DefaultBrush;

      base.AddChild(_textBlock);
    }
  }
}
