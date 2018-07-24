using C1.Xaml.BarCode;
using C1.Xaml.Bitmap;
using C1.Xaml.Document.Export;
using C1.Xaml.Excel;
using C1.Xaml.FlexReport;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace IsbnUwp
{
  /// <summary>
  /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      // FlexGrid にデータを表示
      this.flexgrid1.ItemsSource = Books.GetData();
    }

    private async void ExcelButton_Click(object sender, RoutedEventArgs e)
    {
      // 現在、FlexGrid に表示されている順のデータ
      var currentData = this.flexgrid1.Rows.Select(r => r.DataItem).Cast<Book>();

      // Excel データの作成
      // https://docs.grapecity.com/help/c1/uwp/uwp_excel/#Step_2_of_4-_Adding_Content_to_a_C1XLBook.html

      // 新しい Excel ワークブックを作成
      var xlBook = new C1XLBook();

      // デフォルトで作成されたシートを取得
      XLSheet sheet = xlBook.Sheets[0];

      // シートの中身を書き込みます
      int rowIndex = 0;
      // ヘッダー行
      sheet[rowIndex, 0].Value = "書名";
      sheet[rowIndex, 1].Value = "ISBN";
      sheet[rowIndex, 2].Value = "バーコード";
      sheet.Columns[2].Width
        = C1XLBook.PixelsToTwips(this.HiddenBarCode.ActualWidth);
      sheet[rowIndex, 3].Value = "価格";
      // データ行
      foreach (var book in currentData)
      {
        rowIndex++;

        // バーコードの画像を作る
        this.HiddenBarCode.Text = book.IsbnWithoutCheckDigit;
        C1Bitmap bitmap = new C1Bitmap();
        using (var ms = new InMemoryRandomAccessStream().AsStream())
        {
          await this.HiddenBarCode.SaveAsync(ms, ImageFormat.Png);
          bitmap.Load(ms);
        }

        // 行の高さをバーコードの画像に合わせる
        sheet.Rows[rowIndex].Height
          = C1XLBook.PixelsToTwips(this.HiddenBarCode.ActualHeight);

        // 1行分のデータとバーコード画像をセット
        sheet[rowIndex, 0].Value = book.Title;
        sheet[rowIndex, 1].Value = book.Isbn;
        sheet[rowIndex, 2].Value = bitmap;
        sheet[rowIndex, 3].Value = book.Price;
      }

      // Excel ファイルへの書き出し
      // https://docs.grapecity.com/help/c1/uwp/uwp_excel/#Step_3_of_4-_Saving_the_XLSX_File.html
      var picker = new FileSavePicker()
      {
        SuggestedStartLocation = PickerLocationId.DocumentsLibrary
      };
      picker.FileTypeChoices.Add("Open XML Excel ファイル", new string[] { ".xlsx", });
      picker.FileTypeChoices.Add("BIFF Excel ファイル", new string[] { ".xls", });
      picker.SuggestedFileName = "BarCodeControlSample";
      var file = await picker.PickSaveFileAsync();
      if (file != null)
      {
        var fileFormat = Path.GetExtension(file.Path).Equals(".xls") ? FileFormat.OpenXmlTemplate : FileFormat.OpenXml;
        await xlBook.SaveAsync(file, fileFormat);
      }
    }

    private async void PdfButton_Click(object sender, RoutedEventArgs e)
    {
      // 現在、FlexGrid に表示されている順のデータ
      var currentData = this.flexgrid1.Rows.Select(r => r.DataItem).Cast<Book>();

      // FlexReportの定義を読み込む
      var rpt = new C1FlexReport();
      using (var stream = File.OpenRead("Assets/BooksReport.flxr"))
        rpt.Load(stream, "BooksReport");
      // データを連結
      rpt.DataSource.Recordset = currentData.ToList(); // IEnumerable<T>は不可
      // レポートを生成
      await rpt.RenderAsync();

      // 印刷する場合
      //await rpt.ShowPrintUIAsync();

      // PDF ファイルに直接保存する場合
      var picker = new FileSavePicker()
      {
        SuggestedStartLocation = PickerLocationId.DocumentsLibrary
      };
      picker.FileTypeChoices.Add("PDF ファイル", new string[] { ".pdf", });
      picker.SuggestedFileName = "BarCodeControlSample";
      var file = await picker.PickSaveFileAsync();
      if (file != null)
      {
        // 出力先となる PdfFilter オブジェクトを作成
        var filter = new PdfFilter();
        filter.StorageFile = file;
        // Windows Forms 等では、filter.FileName = file.Path; とする

        // ファイルへ出力
        await rpt.RenderToFilterAsync(filter);
      }
    }
  }
}
