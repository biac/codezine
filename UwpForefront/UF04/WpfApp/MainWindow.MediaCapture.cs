using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WpfApp
{
  public partial class MainWindow
  {
    #region MediaCapture

    private UwpMediaCaptureWrapper _mediaCapture;

    private DispatcherTimer _dispatcherTimer;

    private async Task InitializeMediaCaptureAsync()
    {
      _mediaCapture = await UwpMediaCaptureWrapper.GetInstanceAsync(CameraComboBox.SelectedValue as string);
    }

    private async void ReInitializeMediaCaptureAsync()
    {
      if (_dispatcherTimer == null)
        return;

      this.IsEnabled = false;
      StopTimer();

      while (_isRunning)
        await Task.Delay(100);

      _mediaCapture.Dispose();
      await InitializeMediaCaptureAsync();

      StartTimer();
      this.IsEnabled = true;
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
      if (_dispatcherTimer != null)
        _dispatcherTimer.Stop();
    }

    private bool _isRunning = false;
    // OnTimerTick() は基本的にタイマー割り込みでしか呼び出されないから、ルーズな排他制御で十分。
    private async void OnTimerTickAsync(object sender, EventArgs ea)
    {
      if (_isRunning)
        return;

      try
      {
        _isRunning = true;
        this.Image1.Source = await _mediaCapture.CapturePhotoAsync();
      }
      catch
      {
        // MediaCapture を再作成してみる
        ReInitializeMediaCaptureAsync();
      }
      finally
      {
        _isRunning = false;
      }
    }

    #endregion
  }
}
