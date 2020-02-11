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
using System.Windows.Threading;

namespace StaticVSDynamic
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private readonly DispatcherTimer _timer = new DispatcherTimer();
    private readonly Random _random = new Random();

    public MainWindow()
    {
      InitializeComponent();

      UpdateResources();
      StartTimer();
    }

    private void StartTimer()
    {
      _timer.Interval = TimeSpan.FromMilliseconds(1000);
      _timer.Tick += (s, e) => UpdateResources();
      _timer.Start();
    }

    private void UpdateResources()
    {
      this.Resources["Message"] = $"ただいま {DateTime.Now:HH:mm:ss} です";
      // 注) 文字列の更新は、通常はデータバインディングを使う

      this.Resources["RandomColorBrush"] = CreateRandomColorDarkBrush();
      this.Resources["RandomFontSize"] = _random.NextDouble() * 6.0 + 18.0;

      return;

      SolidColorBrush CreateRandomColorDarkBrush()
      {
        var buffer = new byte[] { 0xff, 0xff, 0xff };

        // RGB 各要素の平均が 128 以下になるようにする
        while(buffer.Sum(b => (int)b) > 128*3)
          _random.NextBytes(buffer);

        var randomColor = Color.FromRgb(buffer[0], buffer[1], buffer[2]);
        return new SolidColorBrush(randomColor);
      }
    }

    private void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
      // StaticResource 側 (左側) のスタイル設定を更新する

      // ※ Style は、適用された後では変更不可。
      //    以下のようなコードは動かない。
      //var ss = this.Resources["StaticStyle"] as Style;
      //var textSetter = ss.Setters.Cast<Setter>().First(s => s.Property.Name == "Text");
      //textSetter.Value = this.Resources["Message"];
      // ↑textSetter.Value は IsSealed=true になっていて、変更不可。

      // 新たに Style オブジェクトを組み立てて、TextBlock にセットする
      // (XAML のスタイル定義と同じことを C# のコードで再実行する)
      var textSetter = new Setter(TextBlock.TextProperty, this.Resources["Message"]);
      var fontSizeSetter = new Setter(TextBlock.FontSizeProperty,
                                      this.Resources["RandomFontSize"]);
      var foregroundSetter = new Setter(TextBlock.ForegroundProperty,
                                        this.Resources["RandomColorBrush"]);

      var textBlockStyle = new Style(typeof(TextBlock),
                            basedOn: this.Resources["TextBaseStyle"] as Style);
      textBlockStyle.Setters.Add(textSetter);
      textBlockStyle.Setters.Add(fontSizeSetter);
      textBlockStyle.Setters.Add(foregroundSetter);

      this.Resources["StaticStyle"] = textBlockStyle;
      TextBlock1.SetResourceReference(TextBlock.StyleProperty, "StaticStyle");
    }
  }
}
