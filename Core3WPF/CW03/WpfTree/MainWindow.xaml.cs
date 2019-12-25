using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using target = SimpleXaml;

namespace WpfTree
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    target.MainWindow _targetWindow;

    public MainWindow()
    {
      InitializeComponent();

      // ウィンドウをマウスのドラッグで移動できるようにする
      this.MouseLeftButtonDown += (s, e) => this.DragMove();

      // タイトルバーとウィンドウの背景にアクセントカラーを設定
      if (SystemParameters.IsGlassEnabled)
      {
        TitleBar.Background = SystemParameters.WindowGlassBrush;
        var accentBrush = SystemParameters.WindowGlassBrush.CloneCurrentValue();
        accentBrush.Opacity = 0.5;
        this.Background = accentBrush;
      }

      // このウィンドウがロードされたときの処理
      this.Loaded += (s, e) =>
      {
        // ターゲットのウィンドウを表示する
        _targetWindow = new target.MainWindow
        {
          Owner = this,
          Left = this.Left,
          Top = this.Top + 250.0,
        };
        _targetWindow.Show();

        // ターゲットのウィンドウのツリー構造を表示する
        AddElementToLogicalTree(_targetWindow, null);
        AddElementToVisualTree(_targetWindow, null);
      };
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
      => this.Close();



    private void AddElementToVisualTree(DependencyObject parent, TreeViewItem treeViewItem)
    {
      TreeViewItem newTreeViewItem = new TreeViewItem
      {
        Header = CreateItemHeader(parent)
      };
      if (treeViewItem == null)
        VisualTreeView.Items.Add(newTreeViewItem);
      else
        treeViewItem.Items.Add(newTreeViewItem);

      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
      {
        var childElement = VisualTreeHelper.GetChild(parent, i);
        AddElementToVisualTree(childElement, newTreeViewItem);
      }
    }

    private void AddElementToLogicalTree(DependencyObject parent, TreeViewItem treeViewItem)
    {
      TreeViewItem newTreeViewItem = new TreeViewItem
      {
        Header = CreateItemHeader(parent)
      };
      if (treeViewItem == null)
        LogicalTreeView.Items.Add(newTreeViewItem);
      else
        treeViewItem.Items.Add(newTreeViewItem);

      foreach (var child in LogicalTreeHelper.GetChildren(parent))
      {
        if(child is DependencyObject childElement)
          AddElementToLogicalTree(childElement, newTreeViewItem);
      }
    }

    private string CreateItemHeader(DependencyObject item)
    {
      string itemName = (item as FrameworkElement)?.Name ?? string.Empty;
      string itemSeparator = (itemName.Length > 0) ? " " : string.Empty;
      string typeName = item.GetType().Name;
      string contentText = (item as TextBlock)?.Text ?? string.Empty;
      string contentTextString = (contentText.Length > 0) ? $" \"{contentText}\"" : string.Empty;
      return $"{itemName}{itemSeparator}[{typeName}]{contentTextString}";
    }
  }
}
