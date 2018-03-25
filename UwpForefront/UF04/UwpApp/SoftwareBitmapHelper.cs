using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace UwpApp
{
  public class SoftwareBitmapHelper
  {
    // 画像データのストリームからSoftwareBitmapを作る
    public static async Task<SoftwareBitmap> ConvertToSoftwareBitmap(IRandomAccessStream stream)
    {
      BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
      return await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
    }

    // SoftwareBitmapの画像フォーマットをImageコントロールに表示できるものに作り直す
    public static SoftwareBitmap CorrectFormat(SoftwareBitmap bitmap)
    {
      if (bitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8
            || bitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
      {
        // Imageコントロールに表示できるのは、BGRA8形式でアルファはプリマルチプライド
        var newBitmap = SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
        bitmap.Dispose();
        return newBitmap;
      }
      return bitmap;
    }

    // SoftwareBitmapからSoftwareBitmapSourceを作る（Imageコントロールでの表示用）
    public static async Task<SoftwareBitmapSource> CreateBitmapSourceAsync(SoftwareBitmap bitmap)
    {
      var source = new SoftwareBitmapSource();
      await source.SetBitmapAsync(bitmap);
      return source;
    }
  }
}
