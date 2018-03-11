using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;



// UWP の OCR
using UwpOcrEngine = Windows.Media.Ocr.OcrEngine;
//using UwpOcrResult = Windows.Media.Ocr.OcrResult;
using UwpLanguage = Windows.Globalization.Language;

// UWP の SoftwareBitmap
using UwpSoftwareBitmap = Windows.Graphics.Imaging.SoftwareBitmap;

// UWP のデバイス
using UwpDeviceInformation = Windows.Devices.Enumeration.DeviceInformation;
using UwpDeviceClass = Windows.Devices.Enumeration.DeviceClass;
//

namespace WpfApp
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    private UwpMediaCaptureWrapper _mediaCapture;

    private DispatcherTimer _dispatcherTimer;



    public MainWindow()
    {
      InitializeComponent();

      this.Loaded += MainWindow_Loaded;

      this.Closed += (s, e) =>
      {
        //var task = Task.Run(async() =>
        //  {
        //  StopTimer();
        //  while (_isRunning)
        //    await Task.Delay(100);
        //  //if (_mediaCapture != null)
        //  //  _mediaCapture.Dispose();
        //  if (_qq != null)
        //    _qq.Dispose();
        //});
        //task.Wait();
        if (_dispatcherTimer != null)
        {
          _dispatcherTimer.Stop();
          _dispatcherTimer = null;
        }
        if (_mediaCapture != null)
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
          //this.CaptureButton.Visibility = Visibility.Collapsed;

          this.CameraComboBox.Visibility = Visibility.Collapsed;
          this.CameraLabel.Visibility = Visibility.Collapsed;
          this.MonitorCameraButton.Visibility = Visibility.Collapsed;
          this.OcrCameraButtan.Visibility = Visibility.Collapsed;
          //MessageBox.Show("カメラがありません", "No Camera",
          //                MessageBoxButton.OK, MessageBoxImage.Information);
          return false;
        }
        var qq = devices.ToList();
        qq.AddRange(qq);
        this.CameraComboBox.ItemsSource = qq;// devices;
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
        _dispatcherTimer.Tick += new EventHandler(OnTimerTickAsync);
        _dispatcherTimer.Start();
      }
    }

    private async Task InitializeMediaCaptureAsync()
    {
      _mediaCapture = await UwpMediaCaptureWrapper.GetInstanceAsync(CameraComboBox.SelectedValue as string);


      //_mediaCapture = new UwpMediaCapture();

      //_mediaCapture.Failed += (s, e) =>
      //{
      //  MessageBox.Show("キャプチャ失敗\n" + e.Message, "Failed", MessageBoxButton.OK, MessageBoxImage.Stop);
      //};

      //var setting = new UwpMediaCaptureInitializationSettings()
      //{
      //  VideoDeviceId = CameraComboBox.SelectedValue as string,
      //  StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Video,
      //};
      //await _mediaCapture.InitializeAsync(setting);
      //_mediaCapture.VideoDeviceController.Brightness.TrySetAuto(true);
      //_mediaCapture.VideoDeviceController.Contrast.TrySetAuto(true);
      //_mediaCapture.VideoDeviceController.Focus.TrySetAuto(true);
      //_mediaCapture.VideoDeviceController.WhiteBalance.TrySetAuto(true);
    }

    private async void ReinitializeMediaCaptureAsync()
    {
      if (_dispatcherTimer == null)
        return;

      this.IsEnabled = false;
      StopTimer();

      while (_isRunning)
        await Task.Delay(100);

      //_mediaCapture.Dispose();
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

        ////var encProperties = Windows.Media.MediaProperties.ImageEncodingProperties.CreatePng();
        //var encProperties = Windows.Media.MediaProperties.ImageEncodingProperties.CreateBmp();
        ////encProperties.Width = (uint)ImageGrid.ActualWidth;
        ////encProperties.Height = (uint)ImageGrid.ActualHeight;

        //using (var randomAccessStream = new UwpInMemoryRandomAccessStream())
        //{
        //  await _mediaCapture.CapturePhotoToStreamAsync(encProperties, randomAccessStream);
        //  randomAccessStream.Seek(0);

        //  //ビットマップにして表示
        //  var bitmap = new BitmapImage();
        //  using (Stream stream = randomAccessStream.AsStream())
        //  {
        //    bitmap.BeginInit();
        //    bitmap.CacheOption = BitmapCacheOption.OnLoad;
        //    bitmap.StreamSource = stream;
        //    bitmap.EndInit();
        //  }
        //  this.Image1.Source = BitmapFrame.Create(bitmap);
        //}
        this.Image1.Source = await _mediaCapture.CapturePhotoAsync();
      }
      catch
      {
        // MediaCapture を再作成してみる
        ReinitializeMediaCaptureAsync();
      }
      finally
      {
        _isRunning = false;
      }
      //}
    }



    private UwpSoftwareBitmap _lastRecognizedBitmap;
    private async Task RecognizeImageAsync()
    {
      UwpSoftwareBitmap bitmap = await GetSoftwareBitmapFromImageAsync();
      if (bitmap == null)
        return;
      _lastRecognizedBitmap = bitmap;

      await RecognizeBitmapAsync(bitmap);
    }

    private async Task ReRecognizeAsync()
    {
      UwpSoftwareBitmap bitmap = _lastRecognizedBitmap;
      if (bitmap != null)
        await RecognizeBitmapAsync(bitmap);
    }

    private async Task RecognizeBitmapAsync(UwpSoftwareBitmap bitmap)
    {
      this.RecognizedTextTextBox.Text = string.Empty;

      var ocrEngine = UwpOcrEngine.TryCreateFromLanguage(this.LangComboBox.SelectedItem as UwpLanguage);
      var ocrResult = await ocrEngine.RecognizeAsync(bitmap);

      //this.RecognizedTextTextBox.Text = ocrResult.Text;
      foreach (var ocrLine in ocrResult.Lines)
        this.RecognizedTextTextBox.Text += (ocrLine.Text + "\n");
    }

    private async Task<UwpSoftwareBitmap> GetSoftwareBitmapFromImageAsync()
    {
      // 表示している BitmapImage
      var sourceBitmap = this.Image1.Source as BitmapFrame;
      if (sourceBitmap == null)
        return null;

      return await UwpSoftwareBitmapHelper.ConvertFrom(sourceBitmap);

      //// BitmapImage を BMP 形式のバイト配列に変換
      //byte[] bitmap;
      //var encoder = new BmpBitmapEncoder();
      //encoder.Frames.Add(sourceBitmap);
      //using (var memoryStream = new MemoryStream())
      //{
      //  encoder.Save(memoryStream);
      //  bitmap = memoryStream.ToArray();
      //}

      //// バイト配列を UWP の IRandomAccessStream に変換
      //var randomAccessStream = new UwpInMemoryRandomAccessStream();
      //var outputStream = randomAccessStream.GetOutputStreamAt(0);
      //var dw = new UwpDataWriter(outputStream);
      ////var task = new Task(() => dw.WriteBytes(bitmap));
      ////task.Start();
      ////await task;
      ////await Task.Run(() => dw.WriteBytes(bitmap));
      //dw.WriteBytes(bitmap);
      //await dw.StoreAsync();
      //await outputStream.FlushAsync();

      //// IRandomAccessStream を SoftwareBitmap に変換
      //var decoder = await UwpBitmapDecoder.CreateAsync(randomAccessStream);
      //var softwareBitmap = await decoder.GetSoftwareBitmapAsync(UwpBitmapPixelFormat.Bgra8, UwpBitmapAlphaMode.Premultiplied);
      //return softwareBitmap;
    }




    #region Event Handlers
    private void CameraComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if(this.MonitorCameraButton.IsChecked == true)
        ReinitializeMediaCaptureAsync();
    }

    private async void LangComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    => await ReRecognizeAsync();


    #endregion


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


    private BitmapFrame GetBitmapFromFile()
    {
      var dialog = new OpenFileDialog();
      dialog.Title = "画像ファイルを開く";
      dialog.Filter = "画像ファイル(*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp";
      if (dialog.ShowDialog() != true)
      {
        return null;
      }

      var source = new BitmapImage(new Uri(dialog.FileName));
      var encoder = new BmpBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(source));
      return encoder.Frames[0];
    }

    private void MonitorCameraButton_Checked(object sender, RoutedEventArgs e)
      => ReinitializeMediaCaptureAsync();//StartTimer();

    private void MonitorCameraButton_Unchecked(object sender, RoutedEventArgs e)
      => StopTimer();


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

    private async void OcrCameraButtan_Click(object sender, RoutedEventArgs e)
    {
      DisableOcrButtons();

      //if (!_dispatcherTimer.IsEnabled)
      if (this.MonitorCameraButton.IsChecked == false)
      {
        // カメラのモニター画像が表示されていないので、1回キャプチャする
        OnTimerTickAsync(this, new EventArgs());
        do
        {
          await Task.Delay(100);
        } while (_isRunning);
      }
      else
      {
        this.MonitorCameraButton.IsChecked = false;
      }

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

      this.MonitorCameraButton.IsChecked = false;
      while (_isRunning)
        await Task.Delay(100);

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

      this.MonitorCameraButton.IsChecked = false;
      while (_isRunning)
        await Task.Delay(100);

      this.Image1.Source = bitmapFrame;
      await RecognizeImageAsync();

      EnableOcrButtons();
    }
  }
}
