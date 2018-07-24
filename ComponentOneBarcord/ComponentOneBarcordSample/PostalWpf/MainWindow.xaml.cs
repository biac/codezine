using Microsoft.Win32;
using System;
using System.Windows;

namespace PostalWpf
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
      // ファイル保存ダイアログを出す
      var saveFileDialog = new SaveFileDialog();
      saveFileDialog.Filter = "PNG Image|*.png";
      saveFileDialog.Title = "Save an Image File";
      saveFileDialog.DefaultExt = ".png";
      saveFileDialog.FileName = "BarCodeControlSample.png";
      saveFileDialog.InitialDirectory
        = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      if (saveFileDialog.ShowDialog() ?? false)
      {
        // 画像をファイルに保存する
        using (var st = saveFileDialog.OpenFile())
        {
          this.BarCode1.Save(st, C1.WPF.BarCode.ImageFormat.Png);
        }
      }
    }
  }
}
