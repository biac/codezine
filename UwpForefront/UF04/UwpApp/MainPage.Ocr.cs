using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace UwpApp
{
  partial class MainPage
  {
    private SoftwareBitmap _lastRecognizedBitmap;

    // SoftwareBitmap を OCR に掛ける
    private async Task RecognizeBitmapAsync(SoftwareBitmap bitmap)
    {
      this.RecognizedTextTextBox.Text = string.Empty;
      this.OverlayCanvas.Children.Clear();

      // 認識言語を変えて再認識させたいときのために保持
      if (_lastRecognizedBitmap != null && _lastRecognizedBitmap != bitmap)
        _lastRecognizedBitmap.Dispose();
      _lastRecognizedBitmap = bitmap;

      // 言語を指定してOCRエンジンのインスタンスを作る
      var ocrEngine = OcrEngine.TryCreateFromLanguage(this.LangComboBox.SelectedItem as Language);
      // OCRエンジンにSoftwareBitmapオブジェクトを渡して文字認識させる
      OcrResult ocrResult = await ocrEngine.RecognizeAsync(bitmap);

      DisplayOcrResult(ocrResult);
    }

    private async Task ReRecognizeAsync()
    {
      if (_lastRecognizedBitmap != null)
        await RecognizeBitmapAsync(_lastRecognizedBitmap);
    }

    private void DisplayOcrResult(OcrResult result)
    {
      // 認識結果を1行ずつテキストボックスに表示する
      foreach (var ocrLine in result.Lines)
        this.RecognizedTextTextBox.Text += (ocrLine.Text + "\n");

      // 認識した領域をキャンバスに描画する
      foreach (var ocrLine in result.Lines)
        foreach (var word in ocrLine.Words)
        {
          Rect r = word.BoundingRect;
          Rectangle rect = new Rectangle()
          {
            Width = r.Width,
            Height = r.Height,
            Fill = new SolidColorBrush(Color.FromArgb(0x40, 0, 0xff, 0)),
            Stroke = new SolidColorBrush(Color.FromArgb(0x60, 0, 0xff, 0)),
            StrokeThickness = 1.0,
          };
          this.OverlayCanvas.Children.Add(rect);
          Canvas.SetLeft(rect, r.Left);
          Canvas.SetTop(rect, r.Top);
        }

      // 領域を描画したキャンバスを回転させる
      this.OverlayCanvas.RenderTransform = new RotateTransform()
      {
        Angle = result.TextAngle ?? 0.0,
        CenterX = this.OverlayCanvas.Width / 2.0,
        CenterY = this.OverlayCanvas.Height / 2.0
      };
    }
  }
}
