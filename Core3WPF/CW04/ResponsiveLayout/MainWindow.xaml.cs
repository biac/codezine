using System.Printing;
using System.Windows;
using System.Windows.Controls;

namespace ResponsiveLayout
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }


    private PageOrientation _lastOrientation; //直前のオリエンテーションを保持

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      var newOrientation = CalcOrientation(this.ActualWidth, this.ActualHeight);
      if (_lastOrientation == newOrientation)
        return;

      _lastOrientation = newOrientation;
      if (newOrientation == PageOrientation.Landscape)
        SetLandscape();
      else
        SetPortrait();

      return;

      // レイアウトが横長／縦長のどちらであるべきかを計算する
      static PageOrientation CalcOrientation(double windowWidth, double windowHeight)
      {
        const double ThresholdRatio = 1.2; //正方形付近のときは縦長レイアウトにする

        var ratio = windowWidth  / windowHeight;
        return (ratio > ThresholdRatio) ? PageOrientation.Landscape : PageOrientation.Portrait;
      }

      // 横長レイアウトに変更する
      void SetLandscape()
      {
        // 説明パネルを右側に（Column=1, Row=0）
        Grid.SetColumn(DescriptionPanel, 1);
        Grid.SetRow(DescriptionPanel, 0);

        // その他、細部の調整
        DescriptionPanel.ExpandDirection = ExpandDirection.Left;
        DescriptionScrollViewer.Width = 200.0;
        DescriptionScrollViewer.Height = double.NaN;
      }

      // 縦長レイアウトに変更する
      void SetPortrait()
      {
        // 説明パネルを左下に（Column=0, Row=1）
        Grid.SetColumn(DescriptionPanel, 0);
        Grid.SetRow(DescriptionPanel, 1);

        // その他、細部の調整
        DescriptionPanel.ExpandDirection = ExpandDirection.Up;
        DescriptionScrollViewer.Width = double.NaN;
        DescriptionScrollViewer.Height = 150.0;
      }
    }
  }
}
