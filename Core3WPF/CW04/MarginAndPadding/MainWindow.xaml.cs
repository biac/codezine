using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MarginAndPadding
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private Rectangle _rect1, _rect2;
    private (Rectangle, Button)[] _targets;

    public MainWindow()
    {
      InitializeComponent();

      PrepareBorderRectangle();
      SizeChanged += (s, e) => ResizeBorderRectangle();
    }

    private void PrepareBorderRectangle()
    {
      _rect1 = new Rectangle();
      _rect2 = new Rectangle();
      _targets = new[] { (_rect1, Button1), (_rect2, Button2), };

      foreach (var (rect, button) in _targets)
      {
        rect.Stroke = new SolidColorBrush(Colors.Pink);
        rect.StrokeThickness = 3.0;
        rect.HorizontalAlignment = button.HorizontalAlignment;
        rect.VerticalAlignment = button.VerticalAlignment;
        RootGrid.Children.Add(rect);
      }
    }

    private void ResizeBorderRectangle()
    {
      foreach (var (rect, button) in _targets)
      {
        rect.Width = button.ActualWidth + button.Margin.Left + button.Margin.Right;
        rect.Height = button.ActualHeight + button.Margin.Top + button.Margin.Bottom;
      }
    }


    private void Button2CheckBox_Changed(object sender, RoutedEventArgs e)
    {
      if (_rect2 == null)
        return;

      bool isChecked = (sender as CheckBox)?.IsChecked ?? false;
      if (isChecked)
      {
        Button2.Visibility = Visibility.Visible;
        TextBlock2.Visibility = Visibility.Visible;
        _rect2.Visibility = Visibility.Visible;
      }
      else 
      {
        Button2.Visibility = Visibility.Hidden;
        TextBlock2.Visibility = Visibility.Hidden;
        _rect2.Visibility = Visibility.Hidden;
      }

    }

    private static readonly Action EmptyDelegate = delegate () { };

    private void WidthCheckBox_Changed(object sender, RoutedEventArgs e)
    {
      const double FixWidth = 150.0;

      bool isChecked = (sender as CheckBox)?.IsChecked ?? false;
      var newWidth = isChecked ? FixWidth : double.NaN;
      foreach (var (rect, button) in _targets)
      {
        button.Width = newWidth;
        forceRedraw(rect);
      }
      ResizeBorderRectangle();

      void forceRedraw(UIElement e)
      {
        e.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
      }
    }
  }
}
