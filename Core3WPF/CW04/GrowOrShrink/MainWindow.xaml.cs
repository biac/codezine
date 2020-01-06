using System.Windows;
using System.Windows.Controls;

namespace GrowOrShrink
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private FrameworkElement[] _controls;

    public MainWindow()
    {
      InitializeComponent();

      _controls = new FrameworkElement[]
      {
        Button1, TextBox1, ListBox1, CheckBox1, Slider1, Image1,
        Button2, TextBox2, ListBox2, CheckBox2, Slider2, Image2,
      };
    }

    private void RadioButton_Checked(object sender, RoutedEventArgs e)
    {
      if ((e.Source as RadioButton)?.Tag is string tag)
        switch (tag)
        {
          case "D":
            Stretch();
            break;
          case "L":
            LeftTop();
            break;
          case "C":
            Center();
            break;
          case "R":
            RightBottom();
            break;
        }
    }

    void Stretch()
    {
      foreach (var ctl in _controls)
      {
        ctl.HorizontalAlignment = HorizontalAlignment.Stretch;
        ctl.VerticalAlignment = VerticalAlignment.Stretch;
      }
    }
    void LeftTop()
    {
      foreach (var ctl in _controls)
      {
        ctl.HorizontalAlignment = HorizontalAlignment.Left;
        ctl.VerticalAlignment = VerticalAlignment.Top;
      }
    }
    void Center()
    {
      foreach (var ctl in _controls)
      {
        ctl.HorizontalAlignment = HorizontalAlignment.Center;
        ctl.VerticalAlignment = VerticalAlignment.Center;
      }
    }
    void RightBottom()
    {
      foreach (var ctl in _controls)
      {
        ctl.HorizontalAlignment = HorizontalAlignment.Right;
        ctl.VerticalAlignment = VerticalAlignment.Bottom;
      }
    }

    private void RadioButton2_Checked(object sender, RoutedEventArgs e)
    {
      if ((e.Source as RadioButton)?.Tag is string tag)
        switch (tag)
        {
          case "G":
            StackPanel1.Visibility = Visibility.Collapsed;
            break;
          case "H":
            StackPanel1.Visibility = Visibility.Visible;
            StackPanel1.Orientation = Orientation.Horizontal;
            break;
          case "V":
            StackPanel1.Visibility = Visibility.Visible;
            StackPanel1.Orientation = Orientation.Vertical;
            break;
        }
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
      const double MinWidth = 85.0;
      const double MinHeight = 30.0;

      foreach (var ctl in _controls)
      {
        ctl.MinWidth = MinWidth;
        ctl.MinHeight = MinHeight;
      }
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
      foreach (var ctl in _controls)
      {
        ctl.MinWidth = 0.0;
        ctl.MinHeight = 0.0;
      }
    }
  }
}
