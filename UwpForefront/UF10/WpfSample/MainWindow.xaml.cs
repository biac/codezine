using System.Windows;
using uwpDataTransfer = Windows.ApplicationModel.DataTransfer;

namespace WpfSample
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      this.Loaded += async (s, e) =>
      {
        await Data.ClipboardHistoryData.TryUpdateAsync();
        this.ItemsControl.ItemsSource = Data.ClipboardHistoryData.Items; ;
      };

      uwpDataTransfer.Clipboard.HistoryChanged += async (s, e) =>
      {
        await Data.ClipboardHistoryData.TryUpdateAsync();
        // WPF では、フォーカスが無くても、最小化されていても、構わず取得できる
      };
    }
  }
}
