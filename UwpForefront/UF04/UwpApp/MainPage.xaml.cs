using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace UwpApp
{
  /// <summary>
  /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
  /// </summary>
  public sealed partial class MainPage : Page
  {



    public MainPage()
    {
      this.InitializeComponent();

      // 画面遷移がないので、サスペンド対応はここでやってしまう
      App.Current.Suspending += async (s, e) => {
        var deferral = e.SuspendingOperation.GetDeferral();
        await CleanupCameraAsync();
        deferral.Complete();
      };
      App.Current.Resuming += async(s, e) =>
      {
        if (this.MonitorCameraButton.IsChecked == true)
          await InitializeMediaCaptureAsync();
      };

      this.Image1.SizeChanged += (s, e) =>
      {
        this.OverlayViewbox.Width = e.NewSize.Width;
        this.OverlayViewbox.Height = e.NewSize.Height;
      };
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      setLanguageList();
      bool hasCamera = await setCameraListAsync();
      if (hasCamera)
      {
        await InitializeMediaCaptureAsync();
      }
      return;

      #region local functions in OnNavigatedTo

      void setLanguageList()
      {
        // サポート言語の一覧を得る
        IReadOnlyList<Language> langList = OcrEngine.AvailableRecognizerLanguages;

        // コンボボックスにサポート言語の一覧を表示する
        this.LangComboBox.ItemsSource = langList;
        // 選択肢として表示するのはDisplayNameプロパティ（「日本語」など）
        this.LangComboBox.DisplayMemberPath = nameof(Windows.Globalization.Language.DisplayName);
        // SelectedValueとしてLanguageTagプロパティを使う（「ja」など）
        this.LangComboBox.SelectedValuePath = nameof(Windows.Globalization.Language.LanguageTag);

        // ユーザープロファイルに基いてOCRエンジンを作ってみる
        var ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
        // その認識言語をコンボボックスで選択状態にする
        this.LangComboBox.SelectedValue = ocrEngine.RecognizerLanguage.LanguageTag;
      }

      async Task<bool> setCameraListAsync()
      {
        var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
        if (devices.Count == 0)
        {
          HideCameraUI();
          return false;
        }
        setupCameraComboBox(devices);

        return true;

        void setupCameraComboBox(IReadOnlyList<DeviceInformation> deviceList)
        {
          this.CameraComboBox.ItemsSource = deviceList;
          this.CameraComboBox.DisplayMemberPath = nameof(DeviceInformation.Name);
          this.CameraComboBox.SelectedValuePath = nameof(DeviceInformation.Id);
          this.CameraComboBox.SelectedIndex = 0;
        }
      }
      #endregion
    } // OnNavigatedTo





    private void HideCameraUI()
    {
      this.CamereUIPanel.Visibility = Visibility.Collapsed;
      this.OcrCameraButton.Visibility = Visibility.Collapsed;
      this.MonitorCameraButton.IsChecked = false;
    }

    private void ShowErrorMessage(uint errorCode, string messge, bool retry = false)
    {
      this.ErrorMessageText.Text = $"{messge}\n(code=0x{errorCode:X})";
      this.RetryLabel.Opacity = retry ? 1.0 : 0.0;
      this.ErrorMessageGrid.Visibility = Visibility.Visible;
    }

    private void DisableOcrButtons()
    {
      this.LangComboBox.IsEnabled = false;
      this.OcrCameraButton.IsEnabled = false;
      this.OcrClipboardButton.IsEnabled = false;
      this.OcrFileButton.IsEnabled = false;
    }
    private void EnableOcrButtons()
    {
      this.LangComboBox.IsEnabled = true;
      this.OcrCameraButton.IsEnabled = true;
      this.OcrClipboardButton.IsEnabled = true;
      this.OcrFileButton.IsEnabled = true;
    }





    private async void CameraComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (_mediaCapture == null)
        return;

      // モニター中だったら、カメラを切り替えるために再初期化する
      this.CameraControlGrid.Opacity = 0.0;
      await CleanupCameraAsync();
      if(await InitializeMediaCaptureAsync())
        this.CameraControlGrid.Opacity = 1.0;
    }

    private async void MonitorCameraButton_Checked(object sender, RoutedEventArgs e)
    {
      if (this.CameraControlGrid == null) // まだUIの初期化中
        return;

      this.MonitorCameraButton.IsEnabled = false;
      if (await InitializeMediaCaptureAsync())
      {
        //this.Image1.Source = null;
        //_lastRecognizedBitmap?.Dispose();
        //_lastRecognizedBitmap = null;
        this.CameraControlGrid.Opacity = 1.0;
      }
      this.MonitorCameraButton.IsEnabled = true;
      EnableOcrButtons();
    }

    private async void MonitorCameraButton_Unchecked(object sender, RoutedEventArgs e)
    {
      this.MonitorCameraButton.IsEnabled = false;
      this.ErrorMessageGrid.Visibility = Visibility.Collapsed;
      this.CameraControlGrid.Opacity = 0.0;

      await CleanupCameraAsync();
      this.MonitorCameraButton.IsEnabled = true;
    }

    private void BrightSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        _mediaCapture?.VideoDeviceController
          .Brightness.TrySetValue(e.NewValue);
    }

    private void ContrastSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        _mediaCapture?.VideoDeviceController
          .Contrast.TrySetValue(e.NewValue);
    }

    private async void LangComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      //if (_mediaCapture != null)
      //  return; // モニター中

      await ReRecognizeAsync();
    }



    private async Task SetImage(SoftwareBitmap bitmap)
    {
      this.Image1.Source = await SoftwareBitmapHelper.CreateBitmapSourceAsync(bitmap);

      this.OverlayCanvas.Width = bitmap.PixelWidth;
      this.OverlayCanvas.Height = bitmap.PixelHeight;
    }

    //private async Task<SoftwareBitmapSource> CreateBitmapSourceAsync(SoftwareBitmap bitmap)
    //{
    //  //if (bitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8
    //  //      || bitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
    //  //{
    //  //  bitmap = SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
    //  //}
    //  var source = new SoftwareBitmapSource();
    //  await source.SetBitmapAsync(bitmap);
    //  return source;
    //}


    //private SoftwareBitmap _lastRecognizedBitmap;

    //// SoftwareBitmap を OCR に掛ける
    //private async Task RecognizeBitmapAsync(SoftwareBitmap bitmap)
    //{
    //  this.RecognizedTextTextBox.Text = string.Empty;

    //  // 認識言語を変えて再認識させたいときのために保持
    //  if(_lastRecognizedBitmap != null && _lastRecognizedBitmap != bitmap)
    //    _lastRecognizedBitmap.Dispose();
    //  _lastRecognizedBitmap = bitmap;

    //  var ocrEngine = OcrEngine.TryCreateFromLanguage(this.LangComboBox.SelectedItem as Language);
    //  OcrResult ocrResult = await ocrEngine.RecognizeAsync(bitmap);

    //  DisplayOcrResult(ocrResult);
    //}

    //private void DisplayOcrResult(OcrResult result)
    //{

    //  foreach (var ocrLine in result.Lines)
    //    this.RecognizedTextTextBox.Text += (ocrLine.Text + "\n");

    //  this.OverlayCanvas.Children.Clear();
    //  foreach (var ocrLine in result.Lines)
    //    foreach(var word in ocrLine.Words)
    //    {
    //      Rect r = word.BoundingRect;
    //      Rectangle rect = new Rectangle()
    //      {
    //        Width = r.Width,
    //        Height = r.Height,
    //        Fill = new SolidColorBrush(Color.FromArgb(0x40, 0,0xff,0)),
    //        Stroke = new SolidColorBrush(Color.FromArgb(0x60, 0, 0xff, 0)),
    //        StrokeThickness = 1.0,
    //      };
    //      this.OverlayCanvas.Children.Add(rect);
    //      Canvas.SetLeft(rect, r.Left);
    //      Canvas.SetTop(rect, r.Top);
    //    }
    //  this.OverlayCanvas.RenderTransform = new RotateTransform()
    //  {
    //    Angle = result.TextAngle?? 0.0,
    //    CenterX = this.OverlayCanvas.Width / 2.0,
    //    CenterY = this.OverlayCanvas.Height / 2.0
    //  };

    //}

    //private async Task ReRecognizeAsync()
    //{
    //  if(_lastRecognizedBitmap != null)
    //    await RecognizeBitmapAsync(_lastRecognizedBitmap);
    //}



    private async void OcrCameraButtun_Click(object sender, RoutedEventArgs e)
    {
      DisableOcrButtons();
      this.ErrorMessageGrid.Visibility = Visibility.Collapsed;

      bool isTemp = (_mediaCapture == null);
      if (isTemp)
      {
        await InitializeMediaCaptureAsync();
        //if (_mediaCapture == null)
        //{
        //  EnableOcrButtons();
        //  return;
        //}
      }

      var bitmap = await CaputureAsync();
      if (bitmap != null)
      {
        bitmap = SoftwareBitmapHelper.CorrectFormat(bitmap);
        await SetImage(bitmap);
        await RecognizeBitmapAsync(bitmap);

        if (isTemp)
          await CleanupCameraAsync();
        this.MonitorCameraButton.IsChecked = false;
      }
      EnableOcrButtons();
    }

    //private async Task<SoftwareBitmap> ConvertToSoftwareBitmap(IRandomAccessStream stream)
    //{
    //  BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
    //  SoftwareBitmap bitmap = await decoder.GetSoftwareBitmapAsync();
    //  //if (bitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8
    //  //      || bitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
    //  //{
    //  //  bitmap = SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
    //  //}
    //  //return bitmap;
    //  return SoftwareBitmapHelper.CorrectFormat(bitmap);
    //}

    //private SoftwareBitmap CorrectFormat(SoftwareBitmap bitmap)
    //{
    //  // Imageコントロールに表示できるのは、Bgra8フォーマットで
    //  // BitmapAlphaModeはStraightでないもの。
    //  if (bitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8
    //        || bitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
    //  {
    //    var newBitmap = SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
    //    bitmap.Dispose();
    //    return newBitmap;
    //  }
    //  return bitmap;
    //}


    private async void OcrFileButton_Click(object sender, RoutedEventArgs e)
    {
      DisableOcrButtons();
      this.ErrorMessageGrid.Visibility = Visibility.Collapsed;

      var picker = new Windows.Storage.Pickers.FileOpenPicker();
      picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
      //picker.SuggestedStartLocation =
      //    Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
      picker.FileTypeFilter.Add(".jpg");
      picker.FileTypeFilter.Add(".jpeg");
      picker.FileTypeFilter.Add(".png");
      picker.FileTypeFilter.Add(".gif");
      picker.FileTypeFilter.Add(".bmp");
      StorageFile file = await picker.PickSingleFileAsync();
      if (file == null)
      {
        EnableOcrButtons();
        return;
      }

      if(this.MonitorCameraButton.IsChecked == true)
        this.MonitorCameraButton.IsChecked = false;

      using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
      {
        //// Create the decoder from the stream
        //BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

        //// Get the SoftwareBitmap representation of the file
        //var bitmap = await decoder.GetSoftwareBitmapAsync();
        //if (bitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8
        //      || bitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
        //{
        //  bitmap = SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
        //}
        var bitmap = await SoftwareBitmapHelper.ConvertToSoftwareBitmap(stream);

        await SetImage(bitmap);
        await RecognizeBitmapAsync(bitmap);
      }
      EnableOcrButtons();
    }

    private async void OcrClipboardButton_Click(object sender, RoutedEventArgs e)
    {
      DisableOcrButtons();
      this.ErrorMessageGrid.Visibility = Visibility.Collapsed;

      DataPackageView dataPackageView = Clipboard.GetContent();
      if (dataPackageView.Contains(StandardDataFormats.Bitmap))
      {
        RandomAccessStreamReference sr = await dataPackageView.GetBitmapAsync();
        // ※ RandomAccessStreamReference は Dispose 不要

        if (sr != null)
        {
          if (this.MonitorCameraButton.IsChecked == true)
            this.MonitorCameraButton.IsChecked = false;

          using (var stream = await sr.OpenReadAsync())
          {
            //// Create the decoder from the stream
            //BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

            //// Get the SoftwareBitmap representation of the file
            //var bitmap = await decoder.GetSoftwareBitmapAsync();
            //if (bitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8
            //    || bitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
            //{
            //  bitmap = SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            //}
            var bitmap = await SoftwareBitmapHelper.ConvertToSoftwareBitmap(stream);

            await SetImage(bitmap);
            await RecognizeBitmapAsync(bitmap);
          }
        }
      }
      else
      {
        await (new MessageDialog("クリップボードに画像がありません", "画像なし"))
                  .ShowAsync();
      }
      EnableOcrButtons();
    }
  }
}
