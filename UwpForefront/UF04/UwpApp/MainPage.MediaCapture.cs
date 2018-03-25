using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.Devices;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace UwpApp
{
  partial class MainPage
  {
    MediaCapture _mediaCapture;
    //bool isPreviewing;
    //DisplayRequest displayRequest = new DisplayRequest();
    //int _initializeRetryCount = 0;
    //const int RETRY_COUNT_MAX = 10;

    async Task<bool> InitializeMediaCaptureAsync()
    {
      if (this.CameraComboBox.SelectedValue == null)
        return false;

      this.ErrorMessageGrid.Visibility = Visibility.Collapsed;

      if (_mediaCapture != null)
        await CleanupCameraAsync(); //通った!

      _mediaCapture = new MediaCapture();
      _mediaCapture.Failed += MediaCapture_Failed;

      var setting = new MediaCaptureInitializationSettings()
      {
        VideoDeviceId = this.CameraComboBox.SelectedValue as string,
        StreamingCaptureMode = StreamingCaptureMode.Video,
      };
      try
      {
        await _mediaCapture.InitializeAsync(setting);
        this.CameraPreview.Source = _mediaCapture;

        await _mediaCapture.StartPreviewAsync();

        setBrightnessControl();
        await setVideoStabilizationEffectAsync();
      }
      catch (UnauthorizedAccessException)
      {
        HideCameraUI();
        //_mediaCapture = null;
        await CleanupCameraAsync();
        return false;
      }
      catch (Exception ex)
      {
        //if (this.MonitorCameraButton.IsChecked != true)
        //{
        //  _mediaCapture = null;
        //  return false; // 通ったか?
        //}

        switch (ex.HResult)
        {
          //case -1072875854: // C00D 36B2 // 通ったか?
          //                  // 現在の状態では、要求は無効です。\n deviceActivateCount
          case -1072873822: // C00D 3EA2 // 通ったか?
            // ビデオ録画デバイスは存在しません。
            // (休止からのリジューム時に出ることがある)

            //case -2147467261: // 8000 4003 // 通った!
            //  // Object reference not set to an instance of an object.

            //  if (_initializeRetryCount++ < RETRY_COUNT_MAX)
            //  {
            //    ShowErrorMessage((uint)ex.HResult, ex.Message, true); // 通った!
            //    await Task.Delay(500);
            //    await CleanupCameraAsync();
            //    await Task.Delay(500);
            //    return await InitializeMediaCaptureAsync();
            //  }
            //  else
            //  {
            //    ShowErrorMessage((uint)ex.HResult, ex.Message, false); // 通ったか?
            //    _mediaCapture = null;
            //    return false;
            //  }

            // Failedハンドラでメッセージ表示済み。何もせず抜ける
            break;

          // リトライ不能ケース
          case -1072875772: // C00D 3704 // 通った!
            // ハードウェア リソースがないため、ハードウェア MFT はストリーミングを開始できませんでした。

            //ShowErrorMessage((uint)ex.HResult, ex.Message);
            //this.ErrorMessageText.Text +=
            //  "\n\nカメラを使っているアプリが他にあるなら、そのアプリを閉じてから、[プレビュー ON / OFF] ボタンを入れなおしてみてください。";
            //await CleanupCameraAsync();

            // Failedハンドラでメッセージ表示済み。何もせず抜ける
            break;

          default:
            ShowErrorMessage((uint)ex.HResult, ex.Message, false); // 通った!
            //_mediaCapture = null;
            await CleanupCameraAsync();
#if DEBUG
            await (new Windows.UI.Popups.MessageDialog($"{ex.Message} [{ex.HResult:X}]", "catch (Exception ex)"))
                      .ShowAsync();
#endif
            return false;
        }
      }


      //_initializeRetryCount = 0;
      return true; // 通った!


      void setBrightnessControl()
      {
        _mediaCapture.VideoDeviceController.Brightness.TrySetAuto(true);
        _mediaCapture.VideoDeviceController.Contrast.TrySetAuto(true);
        _mediaCapture.VideoDeviceController.Focus.TrySetAuto(true);
        _mediaCapture.VideoDeviceController.WhiteBalance.TrySetAuto(true);

        // [明るさ] スライダーを設定
        var brightCtl = _mediaCapture.VideoDeviceController.Brightness;
        this.BrightSlider.Minimum = brightCtl.Capabilities.Min;
        this.BrightSlider.Maximum = brightCtl.Capabilities.Max;
        if (brightCtl.TryGetValue(out double bright))
          this.BrightSlider.Value = bright;

        // [コントラスト] スライダーを設定
        var contrastCtl = _mediaCapture.VideoDeviceController.Contrast;
        this.ContrastSlider.Minimum = contrastCtl.Capabilities.Min;
        this.ContrastSlider.Maximum = contrastCtl.Capabilities.Max;
        if (contrastCtl.TryGetValue(out double contrast))
          this.ContrastSlider.Value = contrast;
      }

      async Task setVideoStabilizationEffectAsync()
      {
        // https://docs.microsoft.com/ja-jp/windows/uwp/audio-video-camera/effects-for-video-capture
        var stabilizerDefinition = new VideoStabilizationEffectDefinition();
        var effect =
    (VideoStabilizationEffect)await _mediaCapture.AddVideoEffectAsync(stabilizerDefinition, MediaStreamType.Photo);
        effect.Enabled = true;
      }

    } // InitializeMediaCaptureAsync


    private async void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs e)
    {
      await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
      {
        switch (e.Code)
        {
          // リトライ可能ケース
          case 0xC00D36B2: // 通ったか?
                           // 現在の状態では、要求は無効です。\n deviceActivateCount
          case 0xC00D3EA2: // 通った!
            // ビデオ録画デバイスは存在しません。
            // (休止からのリジューム時に出ることがある)

            ShowErrorMessage(e.Code, e.Message, true);
            await Task.Delay(500);
            await CleanupCameraAsync();
            await Task.Delay(500);
            await InitializeMediaCaptureAsync();
            break;

          // リトライ不能ケース
          case 0xC00D3704: // 通った!
            // ハードウェア リソースがないため、ハードウェア MFT はストリーミングを開始できませんでした。
            ShowErrorMessage(e.Code, e.Message);
            this.ErrorMessageText.Text +=
              "\n\nカメラを使っているアプリが他にあるなら、そのアプリを閉じてから、[プレビュー ON / OFF] ボタンを入れなおしてみてください。";
            await CleanupCameraAsync();
            break;

          // 対処不要
          case 0xC00D3EA3:
            // ビデオ録画デバイスは、別の Immersive アプリケーションによって割り込まれています。
            break;

          default:
            ShowErrorMessage(e.Code, e.Message);
#if DEBUG
            await (new Windows.UI.Popups.MessageDialog($"{e.Message} [{e.Code:X}]", "_mediaCapture.Failed"))
                      .ShowAsync();
#endif
            break;
        }
      });
    }


    private async Task CleanupCameraAsync()
    {
      if (_mediaCapture == null)
        return;

      try
      {
        if (_mediaCapture.CameraStreamState == CameraStreamState.Streaming)
          await _mediaCapture.StopPreviewAsync();
      }
      catch { }

      await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        this.CameraPreview.Source = null;

        _mediaCapture?.Dispose();
        _mediaCapture = null;
      });
    }


    private async Task<SoftwareBitmap> CaputureAsync()
    {
      if (_mediaCapture == null)
        return null;

      try
      {
        var lowLagCapture = await _mediaCapture.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8));

        var capturedPhoto = await lowLagCapture.CaptureAsync();
        var softwareBitmap = capturedPhoto.Frame.SoftwareBitmap;

        await lowLagCapture.FinishAsync();

        return softwareBitmap;
      }
      catch
      {
        return null;
      }
    }
  }
}
