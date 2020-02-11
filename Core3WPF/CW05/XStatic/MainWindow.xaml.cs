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

namespace XStatic
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public static string NowTime => $"{DateTimeOffset.Now:HH:mm:ss}";

    public static string NowTime1
    {
      get 
      {
        return $"{DateTimeOffset.Now:HH:mm:ss}";
      }
    }


    public static string FullPrimaryScreenSize
      => $"{SystemParameters.PrimaryScreenWidth}px × {SystemParameters.PrimaryScreenHeight}px";

    public MainWindow()
    {
      InitializeComponent();
    }
  }
}
