using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

// UWP の OCR
using UwpOcrEngine = Windows.Media.Ocr.OcrEngine;
//using UwpOcrResult = Windows.Media.Ocr.OcrResult;
using UwpLanguage = Windows.Globalization.Language;

// UWP の SoftwareBitmap
using UwpSoftwareBitmap = Windows.Graphics.Imaging.SoftwareBitmap;

namespace WpfApp
{
  public partial class MainWindow
  {
    #region OCR

    private UwpSoftwareBitmap _lastRecognizedBitmap;

    private async Task RecognizeImageAsync()
    {
      while (_isRunning)
        await Task.Delay(100);

      if (this.Image1.Source == null)
        return;

      // 表示している画像を SoftwareBitmap として取得
      UwpSoftwareBitmap bitmap
        = await UwpSoftwareBitmapHelper.ConvertFrom(this.Image1.Source as BitmapFrame);
      if (bitmap == null)
        return;

      // 認識言語を変えて再認識させたいときのために保持
      _lastRecognizedBitmap = bitmap;

      // SoftwareBitmap を OCR に掛ける
      await RecognizeBitmapAsync(bitmap);
    }

    private async Task ReRecognizeAsync()
    {
      UwpSoftwareBitmap bitmap = _lastRecognizedBitmap;
      if (bitmap == null)
        return;

      await RecognizeBitmapAsync(bitmap);
    }

    // SoftwareBitmap を OCR に掛ける
    private async Task RecognizeBitmapAsync(UwpSoftwareBitmap bitmap)
    {
      this.RecognizedTextTextBox.Text = string.Empty;

      var ocrEngine = UwpOcrEngine.TryCreateFromLanguage(this.LangComboBox.SelectedItem as UwpLanguage);
      var ocrResult = await ocrEngine.RecognizeAsync(bitmap);

      foreach (var ocrLine in ocrResult.Lines)
        this.RecognizedTextTextBox.Text += (ocrLine.Text + "\n");
    }

    #endregion
  }
}
