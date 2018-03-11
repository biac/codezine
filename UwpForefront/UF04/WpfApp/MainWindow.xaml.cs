using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



// UWP の OCR
using UwpOcrEngine = Windows.Media.Ocr.OcrEngine;
//using UwpOcrResult = Windows.Media.Ocr.OcrResult;
using UwpLanguage = Windows.Globalization.Language;

// UWP の SoftwareBitmap
using UwpSoftwareBitmap = Windows.Graphics.Imaging.SoftwareBitmap;
using UwpBitmapDecoder = Windows.Graphics.Imaging.BitmapDecoder;
using UwpBitmapPixelFormat = Windows.Graphics.Imaging.BitmapPixelFormat;
using UwpBitmapAlphaMode = Windows.Graphics.Imaging.BitmapAlphaMode;
using UwpInMemoryRandomAccessStream = Windows.Storage.Streams.InMemoryRandomAccessStream;
using UwpDataWriter = Windows.Storage.Streams.DataWriter;

// UWP の MediaCapture
using UwpMediaCapture = Windows.Media.Capture.MediaCapture;
using UwpMediaCaptureInitializationSettings
  = Windows.Media.Capture.MediaCaptureInitializationSettings;

// UWP のデバイス
using UwpDeviceInformation = Windows.Devices.Enumeration.DeviceInformation;
using UwpDeviceClass = Windows.Devices.Enumeration.DeviceClass;
//
using System.Windows.Threading;

using System.IO;
using System.Threading;
using System.Windows.Interop;
using Microsoft.Win32;

namespace WpfApp
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    //private UwpOcrEngine _ocrEngine
    //  = UwpOcrEngine.TryCreateFromUserProfileLanguages();

    private UwpMediaCapture _mediaCapture;

    private DispatcherTimer _dispatcherTimer;



    public MainWindow()
    {
      InitializeComponent();

      this.Loaded += MainWindow_Loaded;

      this.Closed += async (s, e) =>
      {
        StopTimer();
        while (_isRunning)
          await Task.Delay(100);
        _mediaCapture.Dispose();
      };

      //this.SizeChanged += (s, e) =>
      //{
      //  var w = Image1.Width;
      //};

      Application.Current.DispatcherUnhandledException += (s, e) =>
      {
        var ex = e.Exception;

        //if( sがMediaCaptureだったら… )
        e.Handled = true; //…としても、継続はムリっぽい orz
        _isRunning = false;
        ReinitializeMediaCaptureAsync();
      };
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
          this.CameraComboBox.Visibility = Visibility.Collapsed;
          this.CaptureButton.Visibility = Visibility.Collapsed;
          //MessageBox.Show("カメラがありません", "No Camera",
          //                MessageBoxButton.OK, MessageBoxImage.Information);
          return false;
        }
        var qq = devices.ToList();
        qq.AddRange(qq);
        this.CameraComboBox.ItemsSource = qq;//devices;
        this.CameraComboBox.DisplayMemberPath = nameof(UwpDeviceInformation.Name);
        this.CameraComboBox.SelectedValuePath = nameof(UwpDeviceInformation.Id);
        this.CameraComboBox.SelectedIndex = 0;
        return true;
      }

      void InitializeTimer()
      {
        // CapturePhotoToStreamAsync() が、どうも毎秒1回程度しかキャプチャさせてくれないようだ。
        // タイマー動作にする必要はなかったかもしれない。

        _dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal)
        {
          Interval = TimeSpan.FromMilliseconds(333),
        };
        _dispatcherTimer.Tick += new EventHandler(OnTimerTick);
        _dispatcherTimer.Start();
      }
    }

    private async Task InitializeMediaCaptureAsync()
    {
      _mediaCapture = new UwpMediaCapture();

      _mediaCapture.Failed += (s, e) =>
      {
        MessageBox.Show("キャプチャ失敗\n" + e.Message, "Failed", MessageBoxButton.OK, MessageBoxImage.Stop);
      };

      var setting = new UwpMediaCaptureInitializationSettings()
      {
        VideoDeviceId = CameraComboBox.SelectedValue as string,
        StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Video,
      };
      await _mediaCapture.InitializeAsync(setting);
      _mediaCapture.VideoDeviceController.Brightness.TrySetAuto(true);
      _mediaCapture.VideoDeviceController.Contrast.TrySetAuto(true);
      _mediaCapture.VideoDeviceController.Focus.TrySetAuto(true);
      _mediaCapture.VideoDeviceController.WhiteBalance.TrySetAuto(true);
    }

    private async void ReinitializeMediaCaptureAsync()
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
      if(_dispatcherTimer != null)
        _dispatcherTimer.Start();
    }

    private void StopTimer()
    {
      if (_dispatcherTimer != null)
        _dispatcherTimer.Stop();
    }




    private bool _isRunning = false;
    // OnTimerTick() はタイマー割り込みでしか呼び出されないから、ルーズな排他制御で十分。
    private async void OnTimerTick(object sender, EventArgs ea)
    {
      if (_isRunning)
        return;

      try
      {
        _isRunning = true;

        //var encProperties = Windows.Media.MediaProperties.ImageEncodingProperties.CreatePng();
        var encProperties = Windows.Media.MediaProperties.ImageEncodingProperties.CreateBmp();
        //encProperties.Width = (uint)ImageGrid.ActualWidth;
        //encProperties.Height = (uint)ImageGrid.ActualHeight;

        using (var randomAccessStream = new UwpInMemoryRandomAccessStream())
        {
          await _mediaCapture.CapturePhotoToStreamAsync(encProperties, randomAccessStream);
          randomAccessStream.Seek(0);

          //ビットマップにして表示
          var bmp = new BitmapImage();
          using (Stream stream = randomAccessStream.AsStream())
          {
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.StreamSource = stream;
            bmp.EndInit();
          }
          this.Image1.Source = BitmapFrame.Create(bmp); //bmp;
          // ☝ streamから直接BitmapFrameを作れる!?
        }
      }
      catch (Exception ex)
      {
        // 場合によっては、ここで…
        ReinitializeMediaCaptureAsync();
      }
      finally
      {
        _isRunning = false;
      }
      //}
    }


    private static AsyncLock _recognizeLock = new AsyncLock();
    // ここは複数のイベントハンドラから呼び出されるので、真面目な排他制御が必要
    private async Task RecognizeImageAsync()
    {
      using (await _recognizeLock.LockAsync())
      {
        this.RecognizedTextTextBox.Text = string.Empty;

        UwpSoftwareBitmap bitmap = await GetSoftwareBitmapFromImageAsync();

        var ocrEngine = UwpOcrEngine.TryCreateFromLanguage(this.LangComboBox.SelectedItem as UwpLanguage);
        var ocrResult = await ocrEngine.RecognizeAsync(bitmap);
        //this.RecognizedTextTextBox.Text = ocrResult.Text;
        foreach (var ocrLine in ocrResult.Lines)
          this.RecognizedTextTextBox.Text += (ocrLine.Text + "\n");
      }
    }

    private async Task<UwpSoftwareBitmap> GetSoftwareBitmapFromImageAsync()
    {
      // 表示している BitmapImage
      var sourceBitmap = this.Image1.Source as BitmapFrame;

      // BitmapImage を BMP 形式のバイト配列に変換
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
      //var task = new Task(() => dw.WriteBytes(bitmap));
      //task.Start();
      //await task;
      //await Task.Run(() => dw.WriteBytes(bitmap));
      dw.WriteBytes(bitmap);
      await dw.StoreAsync();
      await outputStream.FlushAsync();

      // IRandomAccessStream を SoftwareBitmap に変換
      var decoder = await UwpBitmapDecoder.CreateAsync(randomAccessStream);
      var softwareBitmap = await decoder.GetSoftwareBitmapAsync(UwpBitmapPixelFormat.Bgra8, UwpBitmapAlphaMode.Premultiplied);
      return softwareBitmap;
    }




    #region Event Handlers
    private void CameraComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ReinitializeMediaCaptureAsync();
    }

    private async void LangComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if(this.CaptureButton.IsChecked == true 
        || this.OpenFileButton.IsChecked == true
        || this.PasteButton.IsChecked == true)
        await RecognizeImageAsync();
    }

    private void CaptureButton_Checked(object sender, RoutedEventArgs e)
      => EnterCaptureMode();
    private void CaptureButton_Unchecked(object sender, RoutedEventArgs e)
      => ExitCaptureMode();

    private void OpenFileButton_Checked(object sender, RoutedEventArgs e)
      => EnterFileMode();
      private void OpenFileButton_Unchecked(object sender, RoutedEventArgs e)
      => ExitFileMode();

    private void PasteButton_Checked(object sender, RoutedEventArgs e)
      => EnterClipboardMode();
    private void PasteButton_Unchecked(object sender, RoutedEventArgs e)
      => ExitClipboardMode();
    #endregion

    #region Change UI Mode
    private async void EnterCaptureMode()
    {
      await disableControlsAsync();
      await RecognizeImageAsync();

      async Task disableControlsAsync()
      {
        CameraComboBox.IsEnabled = false;
        OpenFileButton.IsChecked = false;
        PasteButton.IsChecked = false;
        StopTimer();
        while (_isRunning)
          await Task.Delay(100);
      }
    }
    private void ExitCaptureMode()
    {
      enableControls();

      void enableControls()
      {
        CameraComboBox.IsEnabled = true;
        StartTimer();
      }
    }

    private async void EnterClipboardMode()
    {
      await disableControlsAsync();

      // クリップボードから画像を取り込む
      BitmapFrame bitmapFrame = GetBitmapFromClipboard();
      if (bitmapFrame == null)
      {
        this.PasteButton.IsChecked = false;
        return;
      }

      this.Image1.Source = bitmapFrame;
      await RecognizeImageAsync();

      async Task disableControlsAsync()
      {
        this.CameraComboBox.IsEnabled = false;
        this.CaptureButton.IsChecked = false;
        this.CaptureButton.IsEnabled = false;
        this.OpenFileButton.IsChecked = false;
        StopTimer();
        while (_isRunning)
          await Task.Delay(100);
      }
    }

    private void ExitClipboardMode()
    {
      enableControls();

      void enableControls()
      {
        this.CameraComboBox.IsEnabled = true;
        this.CaptureButton.IsEnabled = true;
        StartTimer();
      }
    }

    private BitmapFrame GetBitmapFromClipboard()
    {
      // クリップボードから画像を取り込む
      BitmapSource image = Clipboard.GetImage();
      if (image == null)
      {
        MessageBox.Show("クリップボードに画像がありません", "画像なし", MessageBoxButton.OK, MessageBoxImage.Information);
        return null;
      }

      BitmapFrame bitmapFrame;
      var encoder = new BmpBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(image));
      using (var stream = new MemoryStream())
      {
        encoder.Save(stream);
        stream.Seek(0, System.IO.SeekOrigin.Begin);
        var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        bitmapFrame = decoder.Frames[0];
      }
      if (bitmapFrame == null)
      {
        MessageBox.Show("クリップボードの画像は認識できないフォーマットです", "不明なフォーマット", MessageBoxButton.OK, MessageBoxImage.Information);
      }
      return bitmapFrame;
    }

    private async void EnterFileMode()
    {
      await disableControlsAsync();

      // ファイルから画像を読み込む
      BitmapFrame bitmapFrame = GetBitmapFromFile();
      if (bitmapFrame == null)
      {
        this.PasteButton.IsChecked = false;
        return;
      }

      this.Image1.Source = bitmapFrame;
      await RecognizeImageAsync();

      async Task disableControlsAsync()
      {
        this.CameraComboBox.IsEnabled = false;
        this.CaptureButton.IsChecked = false;
        this.CaptureButton.IsEnabled = false;
        this.PasteButton.IsChecked = false;
        StopTimer();
        while (_isRunning)
          await Task.Delay(100);
      }
    }

    private void ExitFileMode()
    {
      enableControls();

      void enableControls()
      {
        this.CameraComboBox.IsEnabled = true;
        this.CaptureButton.IsEnabled = true;
        StartTimer();
      }
    }

    private BitmapFrame GetBitmapFromFile()
    {
      var dialog = new OpenFileDialog();
      dialog.Title = "画像ファイルを開く";
      dialog.Filter = "画像ファイル(*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp";
      if (dialog.ShowDialog() != true)
      {
        this.OpenFileButton.IsChecked = false;
        return null;
      }

      var source = new BitmapImage(new Uri(dialog.FileName));
      var encoder = new BmpBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(source));
      return encoder.Frames[0];
    }
    #endregion
  }
}
