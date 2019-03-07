using System.Data;
using System.IO;
using Xamarin.Forms;

namespace XamarinFormsSample
{
  public partial class MainPage : ContentPage
  {
    public MainPage()
    {
      InitializeComponent();
    }

    protected override void OnAppearing()
    {
      base.OnAppearing();

      //try
      //{

      // SQL Server からデータを取得 (SQL Server のユーザー認証を使う)
      var sqlClient = DependencyService.Get<ISqlClientFactoryDS>().Instance;
      DataTable dt = UF03StdLib.Northwind.GetCategories(sqlClient);

      // バイト配列を ImageSource に変換してテーブルに追加
      dt.Columns.Add("ImageSource", typeof(ImageSource));
      foreach (DataRow r in dt.Rows)
        r["ImageSource"] = ByteArrayToImageSource(r["Picture"]);

      ListView1.ItemsSource = dt.Rows;

      //  Label1.Text = $"dt.Rows.Count={dt.Rows.Count}";
      //}
      //catch (Exception ex)
      //{
      //  Label1.Text = ex.ToString();
      //}
    }


    private static ImageSource ByteArrayToImageSource(object byteArray)
    {
      if (byteArray is byte[] b)
      {
        // Northwind の画像データは、先頭に78バイトの「ゴミ」がくっついている
        const int SkipLength = 78;
        return ImageSource.FromStream(() => new MemoryStream(b, SkipLength, b.Length - SkipLength));
      }
      return null;
    }
  }
}
