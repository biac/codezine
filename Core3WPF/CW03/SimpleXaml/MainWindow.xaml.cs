using System.Windows;
using System.Windows.Controls;

namespace SimpleXaml
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    internal Button CodeButton;

    public MainWindow()
    {
      InitializeComponent();

      CodeButton = new Button {
        Name = nameof(CodeButton),
        Content = "コードで作ったボタン",
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Width = 150.0,
      };
      RootGrid.Children.Add(CodeButton);
    }
  }
}
