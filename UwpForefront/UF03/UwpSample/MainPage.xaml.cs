using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace UwpSample
{
  /// <summary>
  /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
  /// </summary>
  public sealed partial class MainPage : Page
  {
    // 画面にバインドするデータ
    private DataRowCollection _rows;

    // x:Bind から使う関数
    private static ImageSource CastToImageSource(object o) => o as ImageSource;



    public MainPage()
    {
      this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      // SQL Server からデータを取得
      DataTable dt = UF03StdLib.Northwind.GetCategories(SqlClientFactory.Instance, true);

      // バイト配列を ImageSource に変換してテーブルに追加
      dt.Columns.Add("ImageSource", typeof(ImageSource));
      foreach (DataRow r in dt.Rows)
        r["ImageSource"] = await ByteArrayToImageSourceAsync(r["Picture"]);

      _rows = dt.Rows;
      Bindings.Update();
    }

    // SqlClientFactory が実装されているアセンブリ
    #region アセンブリ System.Data.SqlClient, Version=4.3.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
    // C:\Users\{USER NAME}\.nuget\packages\microsoft.netcore.universalwindowsplatform\6.0.7\ref\uap10.0.15138\System.Data.SqlClient.dll
    #endregion



    private static async Task<ImageSource> ByteArrayToImageSourceAsync(object byteArray)
    {
      if (byteArray is byte[] b)
      {
        // Northwind の画像データは、先頭に78バイトの「ゴミ」がくっついている
        const int SkipLength = 78;
        IRandomAccessStream image
          = (new MemoryStream(b, SkipLength, b.Length - SkipLength))
            .AsRandomAccessStream();

        // decode image
        var decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.BmpDecoderId, image);
        image.Seek(0);

        // create bitmap
        var output = new WriteableBitmap((int)decoder.PixelHeight, (int)decoder.PixelWidth);
        await output.SetSourceAsync(image);
        return output;
      }
      return null;
    }
  }
}
