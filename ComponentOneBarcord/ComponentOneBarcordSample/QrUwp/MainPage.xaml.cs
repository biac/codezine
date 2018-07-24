using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

      this.SizeChanged += (s, e) => {
        var size = Math.Min(e.NewSize.Width, this.BarCodeAreaHeight.ActualHeight);
        if (size < 0)
          size = 0;
        this.BarCode1.Width = size;
        this.BarCode1.Height = size;
        this.BarCode1.BarHeight = size;
      };

      this.InputTextBox.Text = "UWPアプリでバーコードやQRコードを簡単出力！";
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
      // ファイル保存ダイアログを出す
      var savePicker = new FileSavePicker();
      savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
      savePicker.FileTypeChoices.Add("PNG ファイル", new List<string>() { ".png" });
      var file = await savePicker.PickSaveFileAsync();
      if (file != null)
      {
        // 画像をファイルに保存する
        using (var st = await file.OpenStreamForWriteAsync())
          await this.BarCode1.SaveAsync(st, C1.Xaml.BarCode.ImageFormat.Png);
      }
    }
  }
}
