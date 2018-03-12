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
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs ea)
    {
      SetLanguageList();
      bool hasCamera = await SetCameraListAsync();
      if (hasCamera)
      {
        await InitializeMediaCaptureAsync();
        InitializeTimer();
      }
      return;

      #region local functions in MainWindow_Loaded
      void SetLanguageList()
      {
        IReadOnlyList<UwpLanguage> langList = UwpOcrEngine.AvailableRecognizerLanguages;
        this.LangComboBox.ItemsSource = langList;
        this.LangComboBox.DisplayMemberPath = nameof(UwpLanguage.DisplayName);
        this.LangComboBox.SelectedValuePath = nameof(UwpLanguage.LanguageTag);

        var ocrEngine
        = UwpOcrEngine.TryCreateFromUserProfileLanguages();
        this.LangComboBox.SelectedValue = ocrEngine.RecognizerLanguage.LanguageTag;
      }

      async Task<bool> SetCameraListAsync()
      {
        var devices = await UwpDeviceInformation.FindAllAsync(UwpDeviceClass.VideoCapture);
        if (devices.Count == 0)
        {
          HideCameraUI();
          return false;
        }
        IEnumerable<UwpDeviceInformation> xx = devices;

        //TODO: テスト用
        var qq = devices.ToList();
        qq.AddRange(qq);

        SetupCameraComboBox(qq);
        return true;

        void HideCameraUI()
        {
          this.CameraLabel.Visibility = Visibility.Collapsed;
          this.CameraComboBox.Visibility = Visibility.Collapsed;
          this.MonitorCameraButton.Visibility = Visibility.Collapsed;
          this.OcrCameraButtan.Visibility = Visibility.Collapsed;
        }

        void SetupCameraComboBox(IReadOnlyList<UwpDeviceInformation> deviceList)
        {
          this.CameraComboBox.ItemsSource = deviceList;
          this.CameraComboBox.DisplayMemberPath = nameof(UwpDeviceInformation.Name);
          this.CameraComboBox.SelectedValuePath = nameof(UwpDeviceInformation.Id);
          this.CameraComboBox.SelectedIndex = 0;
        }
      }

      void InitializeTimer()
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
      if (_dispatcherTimer != null)
      {
        _dispatcherTimer.Stop();
        _dispatcherTimer = null;
      }
      if (_mediaCapture != null)
        _mediaCapture.Dispose();
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
      => ReInitializeMediaCaptureAsync();

    private void MonitorCameraButton_Unchecked(object sender, RoutedEventArgs e)
      => StopTimer();

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
