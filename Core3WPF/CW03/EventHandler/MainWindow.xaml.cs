using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EventHandler
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      Button1.Click += Button1_Click;
      TextBox1.TextChanged += TextBox1_TextChanged;

      foreach (var ctl in new UIElement[] { Border1, Grid1, Button1, TextBlock1, TextBox1, })
      {
        ctl.PreviewMouseDown += MouseButtonEventHandler;
        ctl.MouseDown += MouseButtonEventHandler;
        ctl.PreviewMouseUp += MouseButtonEventHandler;
        ctl.MouseUp += MouseButtonEventHandler;

        ctl.PreviewKeyDown += KeyEventHandler;
        ctl.KeyDown += KeyEventHandler;
        ctl.PreviewKeyUp += KeyEventHandler;
        ctl.KeyUp += KeyEventHandler;
      }

      //// 「添付イベント」として親要素でイベントを拾うことも可能。
      //// XAML では次のように書く。
      //// <Border x:Name="Border1" Button.Click="Button1_Click">
      //// C# では、以下のように
      //Border1.AddHandler(Button.ClickEvent, new RoutedEventHandler(Button1_Click));
      //Grid1.AddHandler(Button.ClickEvent, new RoutedEventHandler(Button1_Click));

      // テスト用: 他アプリをアクティブにしてからこのアプリをアクティブにすると、表示をクリア
      this.Activated += (s, e) => Text1.Clear();
    }

    private void Button1_Click(object sender, RoutedEventArgs e)
    {
      var t = sender.GetType();
      var org = e.OriginalSource.GetType();
      Text1.Text += $"Button1_Click: sender={t.Name}, OriginalSource={org.Name}\r";
    }

    private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
    {
      var t = sender.GetType();
      var org = e.OriginalSource.GetType();
      Text1.Text += $"TextBox1_TextChanged: sender={t.Name}, OriginalSource={org.Name}, Text={TextBox1.Text}\r";
    }

    private void KeyEventHandler(object sender, KeyEventArgs e)
    {
      
      var t = sender.GetType();
      var org = e.OriginalSource.GetType();
      Text1.Text += $"{e.RoutedEvent.Name}: sender={t.Name}, OriginalSource={org.Name}, Key={e.Key}\r";
    }

    private void MouseButtonEventHandler(object sender, MouseButtonEventArgs e)
    {
      var t = sender.GetType();
      var org = e.OriginalSource.GetType();
      Text1.Text += $"{e.RoutedEvent.Name}: sender={t.Name}, OriginalSource={org.Name}\r";

      if(HandleCheck.IsChecked == true)
        if (sender is Grid && e.RoutedEvent.Name == "PreviewMouseDown")
        {
          e.Handled = true;
          Text1.Text += $"PreviewMouseDown: e.Handled=true @Grid\r";
        }
    }
  }
}
