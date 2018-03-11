using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

// UWP の MediaCapture
using UwpMediaCapture = Windows.Media.Capture.MediaCapture;
using UwpMediaCaptureInitializationSettings
  = Windows.Media.Capture.MediaCaptureInitializationSettings;
using UwpInMemoryRandomAccessStream = Windows.Storage.Streams.InMemoryRandomAccessStream;

namespace WpfApp
{
  public class UwpMediaCaptureWrapper : IDisposable
  {
    private UwpMediaCapture _mediaCapture;

    private UwpMediaCaptureWrapper() { /* avoid instance */}
    public static async Task<UwpMediaCaptureWrapper> GetInstanceAsync(string videoDeviceId)
    {
      var instance = new UwpMediaCaptureWrapper();
      await instance.InitializeMediaCaptureAsync(videoDeviceId);
      return instance;
    }

    private async Task InitializeMediaCaptureAsync(string videoDeviceId)
    {
      _mediaCapture = new UwpMediaCapture();

      _mediaCapture.Failed += (s, e) =>
      {
        MessageBox.Show("キャプチャ失敗\n" + e.Message, "Failed", MessageBoxButton.OK, MessageBoxImage.Stop);
      };

      var setting = new UwpMediaCaptureInitializationSettings()
      {
        VideoDeviceId = videoDeviceId,
        StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Video,
      };
      await _mediaCapture.InitializeAsync(setting);
      _mediaCapture.VideoDeviceController.Brightness.TrySetAuto(true);
      _mediaCapture.VideoDeviceController.Contrast.TrySetAuto(true);
      _mediaCapture.VideoDeviceController.Focus.TrySetAuto(true);
      _mediaCapture.VideoDeviceController.WhiteBalance.TrySetAuto(true);
    }

    public async Task<BitmapFrame> CapturePhotoAsync()
    {
      //var encProperties = Windows.Media.MediaProperties.ImageEncodingProperties.CreatePng();
      var encProperties = Windows.Media.MediaProperties.ImageEncodingProperties.CreateBmp();
      //encProperties.Width = (uint)ImageGrid.ActualWidth;
      //encProperties.Height = (uint)ImageGrid.ActualHeight;

      using (var randomAccessStream = new UwpInMemoryRandomAccessStream())
      {
        await _mediaCapture.CapturePhotoToStreamAsync(encProperties, randomAccessStream);
        randomAccessStream.Seek(0);

        //ビットマップにして返す
        var bitmap = new BitmapImage();
        using (Stream stream = randomAccessStream.AsStream())
        {
          bitmap.BeginInit();
          bitmap.CacheOption = BitmapCacheOption.OnLoad;
          bitmap.StreamSource = stream;
          bitmap.EndInit();
        }
        return BitmapFrame.Create(bitmap);
      }

    }



    #region IDisposable Support
    private bool disposedValue = false; // 重複する呼び出しを検出するには

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          _mediaCapture.Dispose();
        }

        // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
        // TODO: 大きなフィールドを null に設定します。

        disposedValue = true;
      }
    }

    // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
    // ~UwpMediaCaptureHelper() {
    //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
    //   Dispose(false);
    // }

    // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
    public void Dispose()
    {
      // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
      Dispose(true);
      // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
      // GC.SuppressFinalize(this);
    }
    #endregion


  }
}
