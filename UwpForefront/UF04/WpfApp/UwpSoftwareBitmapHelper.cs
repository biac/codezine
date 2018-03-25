using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

// UWPのSoftwareBitmap関連のエイリアス（UWP APIの使用箇所を明確にするため）
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
      // BitmapFrameをBMP形式のバイト配列に変換
      byte[] bitmapBytes;
      var encoder = new BmpBitmapEncoder(); // ここは.NET用のエンコーダーを使う
      encoder.Frames.Add(sourceBitmap);
      using (var memoryStream = new MemoryStream())
      {
        encoder.Save(memoryStream);
        bitmapBytes = memoryStream.ToArray();
      }

      // バイト配列をUWPのIRandomAccessStreamに変換
      using (var randomAccessStream = new UwpInMemoryRandomAccessStream())
      {
        using (var outputStream = randomAccessStream.GetOutputStreamAt(0))
        using (var writer = new UwpDataWriter(outputStream))
        {
          writer.WriteBytes(bitmapBytes);
          await writer.StoreAsync();
          await outputStream.FlushAsync();
        }

        // IRandomAccessStreamをSoftwareBitmapに変換
        // （ここはUWP APIのデコーダーを使う）
        var decoder = await UwpBitmapDecoder.CreateAsync(randomAccessStream);
        var softwareBitmap 
          = await decoder.GetSoftwareBitmapAsync(UwpBitmapPixelFormat.Bgra8,
                                                 UwpBitmapAlphaMode.Premultiplied);
        return softwareBitmap;
      }
    }
  }
}
