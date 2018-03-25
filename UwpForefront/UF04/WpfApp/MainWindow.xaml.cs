using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

// UWP の OCR
using UwpOcrEngine = Windows.Media.Ocr.OcrEngine;
using UwpLanguage = Windows.Globalization.Language;

// UWP のデバイス
using UwpDeviceInformation = Windows.Devices.Enumeration.DeviceInformation;
using UwpDeviceClass = Windows.Devices.Enumeration.DeviceClass;

namespace WpfApp
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {

    public MainWindow()
    {
      InitializeComponent();

      this.Loaded += MainWindow_Loaded;
      this.Closed += MainWindow_Closed;

      Application.Current.DispatcherUnhandledException += (s, e) =>
      {
        var ex = e.Exception;

        switch (ex.HResult)
        {
          case -1072875854: // C00D 36B2
            // 現在の状態では、要求は無効です。\n deviceActivateCount
          case -1072873822: // C00D 3EA2
            // ビデオ録画デバイスは存在しません。 (HRESULT からの例外:0xC00D3EA2)
          case -2147024893: // 8007 0003
            // 指定されたパスが見つかりません。

            e.Handled = true; 
            _isRunning = false;
            ReInitializeMediaCaptureAsync();
            break;

          default:
#if DEBUG
            MessageBox.Show($"DispatcherUnhandledException[{ex.HResult}]: {ex.Message}", "ERROR", MessageBoxButton.OK, MessageBoxImage.Stop);
#endif
            break;
        }

      };
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs ea)
    {
      setLanguageList();
      bool hasCamera = await setCameraListAsync();
      if (hasCamera)
      {
        await InitializeMediaCaptureAsync();
        setBrightnessControl();
        initializeTimer();
      }
      return;

      #region local functions in MainWindow_Loaded
      void setLanguageList()
      {
        IReadOnlyList<UwpLanguage> langList = UwpOcrEngine.AvailableRecognizerLanguages;
        this.LangComboBox.ItemsSource = langList;
        this.LangComboBox.DisplayMemberPath = nameof(UwpLanguage.DisplayName);
        this.LangComboBox.SelectedValuePath = nameof(UwpLanguage.LanguageTag);

        var ocrEngine
        = UwpOcrEngine.TryCreateFromUserProfileLanguages();
        this.LangComboBox.SelectedValue = ocrEngine.RecognizerLanguage.LanguageTag;
      }

      async Task<bool> setCameraListAsync()
      {
        var devices = await UwpDeviceInformation.FindAllAsync(UwpDeviceClass.VideoCapture);
        if (devices.Count == 0)
        {
          hideCameraUI();
          return false;
        }
        setupCameraComboBox(devices);
        return true;

        void hideCameraUI()
        {
          this.CameraLabel.Visibility = Visibility.Collapsed;
          this.CameraComboBox.Visibility = Visibility.Collapsed;
          this.MonitorCameraButton.Visibility = Visibility.Collapsed;
          this.CameraControlGrid.Visibility = Visibility.Collapsed;
          this.OcrCameraButtan.Visibility = Visibility.Collapsed;
        }

        void setupCameraComboBox(IReadOnlyList<UwpDeviceInformation> deviceList)
        {
          this.CameraComboBox.ItemsSource = deviceList;
          this.CameraComboBox.DisplayMemberPath = nameof(UwpDeviceInformation.Name);
          this.CameraComboBox.SelectedValuePath = nameof(UwpDeviceInformation.Id);
          this.CameraComboBox.SelectedIndex = 0;
        }
      }

      void setBrightnessControl()
      {
        // [明るさ] スライダーを設定
        var brightCtl = _mediaCapture.BrightnessControl;
        this.BrightSlider.Minimum = brightCtl.Capabilities.Min;
        this.BrightSlider.Maximum = brightCtl.Capabilities.Max;
        if (brightCtl.TryGetValue(out double bright))
          this.BrightSlider.Value = bright;

        // [コントラスト] スライダーを設定
        var contrastCtl = _mediaCapture.ContrastControl;
        this.ContrastSlider.Minimum = contrastCtl.Capabilities.Min;
        this.ContrastSlider.Maximum = contrastCtl.Capabilities.Max;
        if (contrastCtl.TryGetValue(out double contrast))
          this.ContrastSlider.Value = contrast;
      }

      void initializeTimer()
      {
        // CapturePhotoToStreamAsync() が、どうも毎秒1回程度しかキャプチャさせてくれないようだ。
        // タイマー動作にする必要はなかったかもしれない。

        _dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal)
        {
          Interval = TimeSpan.FromMilliseconds(333),
        };
        _dispatcherTimer.Tick += new EventHandler(OnTimerTickAsync);
        _dispatcherTimer.Start();
      }
      #endregion
    } // END MainWindow_Loaded

    private void MainWindow_Closed(object sender, EventArgs e)
    {
      _dispatcherTimer?.Stop();
      _dispatcherTimer = null;

      _mediaCapture?.Dispose();
    }


    private void DisableOcrButtons()
    {
      this.LangComboBox.IsEnabled = false;
      this.OcrCameraButtan.IsEnabled = false;
      this.OcrClipboardButton.IsEnabled = false;
      this.OcrFileButton.IsEnabled = false;
    }
    private void EnableOcrButtons()
    {
      this.LangComboBox.IsEnabled = true;
      this.OcrCameraButtan.IsEnabled = true;
      this.OcrClipboardButton.IsEnabled = true;
      this.OcrFileButton.IsEnabled = true;
    }

    private async Task TurnOffMonitorCameraButtonAsync()
    {
      this.MonitorCameraButton.IsChecked = false;
      do
      {
        await Task.Delay(100);
      } while (_isRunning);
    }

    #region Event Handlers

    private void CameraComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      // モニター中だったら、カメラを切り替えるために再初期化する
      if(this.MonitorCameraButton.IsChecked == true)
        ReInitializeMediaCaptureAsync();
    }

    private void MonitorCameraButton_Checked(object sender, RoutedEventArgs e)
    {
      ReInitializeMediaCaptureAsync();

      if(this.CameraControlGrid != null)
        this.CameraControlGrid.Visibility = Visibility.Visible;
    }

    private void MonitorCameraButton_Unchecked(object sender, RoutedEventArgs e)
    {
      StopTimer();

      this.ErrorMessageGrid.Visibility = Visibility.Collapsed;
      this.CameraControlGrid.Visibility = Visibility.Hidden;
    }

    private async void BrightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      e.Handled = true;
      await Task.Run(async () =>
      {
        using (await _mediaCaptureLock.LockAsync())
        {
          _mediaCapture.BrightnessControl.TrySetValue(e.NewValue);
        }
      });
    }

    private async void ContrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      e.Handled = true;
      await Task.Run(async () =>
      {
        using (await _mediaCaptureLock.LockAsync())
        {
          _mediaCapture.ContrastControl.TrySetValue(e.NewValue);
        }
      });
    }


    private async void LangComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
      => await ReRecognizeAsync();

    private async void OcrCameraButtan_Click(object sender, RoutedEventArgs e)
    {
      DisableOcrButtons();

      if (!_dispatcherTimer.IsEnabled && !_isRunning)
        // カメラのモニター画像が表示されていないので、1回キャプチャする
        OnTimerTickAsync(this, new EventArgs());
      else
        await TurnOffMonitorCameraButtonAsync();

      await RecognizeImageAsync();

      EnableOcrButtons();
    }

    private async void OcrFileButton_Click(object sender, RoutedEventArgs e)
    {
      DisableOcrButtons();

      // ファイルから画像を読み込む
      BitmapFrame bitmapFrame = GetBitmapFromFile();
      if (bitmapFrame == null)
      {
        EnableOcrButtons();
        return;
      }

      await TurnOffMonitorCameraButtonAsync();
      this.Image1.Source = bitmapFrame;

      await RecognizeImageAsync();

      EnableOcrButtons();
    }

    private async void OcrClipboardButton_Click(object sender, RoutedEventArgs e)
    {
      DisableOcrButtons();

      // クリップボードから画像を取り込む
      BitmapFrame bitmapFrame = GetBitmapFromClipboard();
      if (bitmapFrame == null)
      {
        EnableOcrButtons();
        return;
      }

      await TurnOffMonitorCameraButtonAsync();
      this.Image1.Source = bitmapFrame;

      await RecognizeImageAsync();

      EnableOcrButtons();
    }

    #endregion
  }
}
