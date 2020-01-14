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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tetrahedron
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      // ウィンドウをマウスのドラッグで移動できるようにする
      this.MouseLeftButtonDown += (s, e) => this.DragMove();

      // タイトルバーとウィンドウの背景にアクセントカラーを設定
      if (SystemParameters.IsGlassEnabled)
      {
        SetWindowGlassBrush();
        SystemParameters.StaticPropertyChanged += (s, e) => {
          if (e.PropertyName == "WindowGlassBrush")
            SetWindowGlassBrush(); 
        };

        void SetWindowGlassBrush()
        {
          TitleBar.Background = SystemParameters.WindowGlassBrush;
          var accentBrush = SystemParameters.WindowGlassBrush.CloneCurrentValue();
          accentBrush.Opacity = 0.5;
          this.Background = accentBrush;
        }
      }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
      => this.Close();
  }
}
