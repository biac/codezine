using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;

namespace WpfApp
{
  public partial class MainWindow
  {
    #region MediaCapture

    private UwpMediaCaptureWrapper _mediaCapture;

    private DispatcherTimer _dispatcherTimer;

    private async Task InitializeMediaCaptureAsync()
    {
      this.CameraControlGrid.IsEnabled = false;

      _mediaCapture = await UwpMediaCaptureWrapper.GetInstanceAsync(CameraComboBox.SelectedValue as string);

      _mediaCapture.Failed += (s, e) =>
      {
        Dispatcher.Invoke(new Action(() =>
        {
          this.ErrorMessageText.Text = $"{e.Message}\n(code=0x{e.Code:X})";
          this.RetryLabel.Visibility = Visibility.Hidden;
          this.ErrorMessageGrid.Visibility = Visibility.Visible;

          if (e.Code == 0xC00D3704)
          {
            // 0xC00D3704: ハードウェア リソースがないため、ハードウェア MFT はストリーミングを開始できませんでした。
            this.ErrorMessageText.Text += "\n\nカメラを使っているアプリが他にあるなら、そのアプリを閉じてから、[カメラのモニター] ボタンを入れなおしてみてください。。";
          }
        }));
      };

      this.CameraControlGrid.IsEnabled = true;
    }

    private async void ReInitializeMediaCaptureAsync()
    {
      if (_dispatcherTimer == null)
        return;
      if (this.MonitorCameraButton.IsChecked != true)
        return;

      this.CameraControlGrid.IsEnabled = false;

      StopTimer();

      while (_isRunning)
        await Task.Delay(100);

      await Task.Delay(500);
      this.RetryLabel.Visibility = Visibility.Visible;

      _mediaCapture.Dispose();
      await InitializeMediaCaptureAsync();

      _mediaCapture.BrightnessControl.TrySetValue(this.BrightSlider.Value);
      _mediaCapture.ContrastControl.TrySetValue(this.ContrastSlider.Value);

      StartTimer();
      //this.IsEnabled = true;
    }

    private void StartTimer()
    {
      if (_dispatcherTimer != null)
      {
        _lastRecognizedBitmap = null;
        _dispatcherTimer.Start();
      }
    }

    private void StopTimer()
    {
      _dispatcherTimer?.Stop();
    }

    private bool _isRunning = false;
    // OnTimerTick() は基本的にタイマー割り込みでしか呼び出されないから、ルーズな排他制御で十分。
    // …と思っていたけど、TrySetValue を追加したので、そこと CapturePhotoAsync はアトミックにしておかないとまずそう。
    static AsyncLock _mediaCaptureLock = new AsyncLock();

    private async void OnTimerTickAsync(object sender, EventArgs ea)
    {
      if (_isRunning)
        return;

      try
      {
        _isRunning = true;

        using (await _mediaCaptureLock.LockAsync())
        {
          _mediaCapture.BrightnessControl.TrySetValue(this.BrightSlider.Value);
          _mediaCapture.ContrastControl.TrySetValue(this.ContrastSlider.Value);

          this.Image1.Source = await _mediaCapture.CapturePhotoAsync();
        }

        this.ErrorMessageGrid.Visibility = Visibility.Collapsed;
      }
      catch(Exception ex)
      {
        switch (ex.HResult)
        {
          case -2147023901:
          // = 8007 03E3: スレッドの終了またはアプリケーションの要求によって、I/O 処理は中止されました。 (HRESULT からの例外:0x800703E3)
          // (実行中、頻繁に出ることがある)
          case -1072873822:
          // = C00D 3EA2: ビデオ録画デバイスは存在しません。
          // (休止からのリジューム時に出ることがある)

            // MediaCapture を再作成してみる
            ReInitializeMediaCaptureAsync();
            break;

          case -1072875772:
          // = C00D 3704: ハードウェア リソースがないため、ハードウェア MFT はストリーミングを開始できませんでした。
          case -1072845852:
          // = 3FF2 541C: この操作は無効です。\r\nPhotoState

            // カメラが他のアプリに使われているときのエラー
            _dispatcherTimer?.Stop();
            break;

          default:
#if DEBUG
            MessageBox.Show($"カメラキャプチャ時に未知の例外[{ex.HResult}]: {ex.Message}", "ERROR", MessageBoxButton.OK, MessageBoxImage.Stop);
            ReInitializeMediaCaptureAsync();
#endif
            break;
        }
      }
      finally
      {
        _isRunning = false;
      }
    }

    #endregion
  }
}
