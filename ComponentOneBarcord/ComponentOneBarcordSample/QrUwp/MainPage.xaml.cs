using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace QrUwp
{
  /// <summary>
  /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();

      //this.BarCode1.QRCodeOptions.Encoding = System.Text.Encoding.ASCII;

      this.SizeChanged += (s, e) => {
        var size = Math.Min(e.NewSize.Width, this.BarCodeAreaHeight.ActualHeight);
        this.BarCode1.Width = size;
        this.BarCode1.Height = size;
        this.BarCode1.BarHeight = size;
      };
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
      var savePicker = new Windows.Storage.Pickers.FileSavePicker();
      savePicker.SuggestedStartLocation
        = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
      savePicker.FileTypeChoices.Add("PNG ファイル", new List<string>() { ".png" });
      var file = await savePicker.PickSaveFileAsync();
      if (file != null)
      {
        using (var st = await file.OpenStreamForWriteAsync())
          await this.BarCode1.SaveAsync(st, C1.Xaml.BarCode.ImageFormat.Png);
      }
    }
  }
}
