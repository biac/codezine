using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfApp
{
  public partial class MainWindow
  {
    #region acquiring images

    private BitmapFrame GetBitmapFromClipboard()
    {
      // クリップボードから画像を取り込む
      BitmapSource image = Clipboard.GetImage();
      if (image == null)
      {
        MessageBox.Show("クリップボードに画像がありません", "画像なし", MessageBoxButton.OK, MessageBoxImage.Information);
        return null;
      }

      // BitmapSource を BitmapFrame に変換して返す
      BitmapFrame bitmapFrame;
      var encoder = new BmpBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(image));
      using (var stream = new MemoryStream())
      {
        encoder.Save(stream);
        stream.Seek(0, SeekOrigin.Begin);
        var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        bitmapFrame = decoder.Frames[0];
      }
      if (bitmapFrame == null)
      {
        MessageBox.Show("クリップボードの画像は認識できないフォーマットです", "不明なフォーマット", MessageBoxButton.OK, MessageBoxImage.Information);
      }
      return bitmapFrame;
    }

    private BitmapFrame GetBitmapFromFile()
    {
      var dialog = new OpenFileDialog();
      dialog.Title = "画像ファイルを開く";
      dialog.Filter = "画像ファイル(*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp";
      if (dialog.ShowDialog() != true)
      {
        return null;
      }

      var source = new BitmapImage(new Uri(dialog.FileName));
      var encoder = new BmpBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(source));
      return encoder.Frames[0];
    }

    #endregion
  }
}
