using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

// UWP の SoftwareBitmap
using UwpSoftwareBitmap = Windows.Graphics.Imaging.SoftwareBitmap;
using UwpBitmapDecoder = Windows.Graphics.Imaging.BitmapDecoder;
using UwpBitmapPixelFormat = Windows.Graphics.Imaging.BitmapPixelFormat;
using UwpBitmapAlphaMode = Windows.Graphics.Imaging.BitmapAlphaMode;
using UwpInMemoryRandomAccessStream = Windows.Storage.Streams.InMemoryRandomAccessStream;
using UwpDataWriter = Windows.Storage.Streams.DataWriter;

namespace WpfApp
{
  public static class UwpSoftwareBitmapHelper
  {
    public static async Task<UwpSoftwareBitmap> ConvertFrom(BitmapFrame sourceBitmap)
    {
      // BitmapFrame を BMP 形式のバイト配列に変換
      byte[] bitmap;
      var encoder = new BmpBitmapEncoder();
      encoder.Frames.Add(sourceBitmap);
      using (var memoryStream = new MemoryStream())
      {
        encoder.Save(memoryStream);
        bitmap = memoryStream.ToArray();
      }

      // バイト配列を UWP の IRandomAccessStream に変換
      var randomAccessStream = new UwpInMemoryRandomAccessStream();
      var outputStream = randomAccessStream.GetOutputStreamAt(0);
      var dw = new UwpDataWriter(outputStream);
      dw.WriteBytes(bitmap);
      await dw.StoreAsync();
      await outputStream.FlushAsync();

      // IRandomAccessStream を SoftwareBitmap に変換
      var decoder = await UwpBitmapDecoder.CreateAsync(randomAccessStream);
      var softwareBitmap = await decoder.GetSoftwareBitmapAsync(UwpBitmapPixelFormat.Bgra8, UwpBitmapAlphaMode.Premultiplied);
      return softwareBitmap;
    }
  }
}
