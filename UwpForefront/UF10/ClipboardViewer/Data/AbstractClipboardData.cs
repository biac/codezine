using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace ClipboardViewer.Data
{
  public abstract class AbstractClipboardData
  {
    // クリップボードのデータ
    protected DataPackageView OriginalContent { get; private set; }

    // 表示用のデータ
    public BitmapImage Bitmap { get; private set; }
    public ulong BitmapSize { get; private set; }
    public int BitmapWidth => this.Bitmap?.PixelWidth ?? 0;
    public int BitmapHeight => this.Bitmap?.PixelHeight ?? 0;

    public int TextSize { get; private set; }
    public string TextHead { get; private set; }
    const int TextHeadLength = 1024;
    // テキストデータは、履歴にも 10MBytes 以上のサイズで入ってくる。
    // それを TextBlock にバインドするとメモリ不足になる可能性があるので、
    // 保持するテキストのサイズは制限する。
    // ※ 必要になったら OriginalContent から再取得すればよい。
    //    (そのとき、OriginalContent が無くなっているかもしれないことには注意)

    // 入っているデータのフォーマット
    public IReadOnlyList<string> AvailableFormats { get; private set; }

    // ローミングされてきたデータかどうか
    public bool IsFromRoamingClipboard { get; private set; }
    // クリップボードでは、ローミングされてきたら true になる
    // 履歴では常に false
     

    //public readonly Dictionary<string, string> ControlInfoDictionary
    //  = new Dictionary<string, string>();
    //public readonly Dictionary<string, string> Properties
    //  = new Dictionary<string, string>();



    protected async Task SetDataAsync(DataPackageView content)
    {
#if DEBUG
      if (content.Properties.ApplicationName is string name
           && name.Length > 0)
        await (new Windows.UI.Popups.MessageDialog($"ApplicationName:{name}")).ShowAsync();
#endif

      this.OriginalContent = content;
      this.AvailableFormats = content.AvailableFormats?.ToList() ?? (new List<string>());
      this.IsFromRoamingClipboard = content.Properties.IsFromRoamingClipboard;

      // テキストデータ（該当するデータが複数あるときは、最後のもの）
      this.TextHead = string.Empty; this.TextSize = 0;

      if (content.Contains(StandardDataFormats.Html)) //"HTML Format"
        this.TextHead = await content.GetHtmlFormatAsync();
      if (content.Contains(StandardDataFormats.WebLink)) //"UniformResourceLocatorW"
        this.TextHead = (await content.GetWebLinkAsync()).ToString();
      if (content.Contains(StandardDataFormats.Text)) //"Text"
        this.TextHead = await content.GetTextAsync();
      this.TextSize = this.TextHead.Length * sizeof(char);

      if (this.TextHead.Length > TextHeadLength)
        this.TextHead = $"{this.TextHead.Substring(0, TextHeadLength)}(以下略)";

      // ビットマップデータ
      this.Bitmap = new BitmapImage(); this.BitmapSize = 0;
      if (content.Contains(StandardDataFormats.Bitmap)) //"Bitmap"
      {
        RandomAccessStreamReference stRef = await content.GetBitmapAsync();
        using (IRandomAccessStreamWithContentType stream = await stRef.OpenReadAsync())
        {
          this.Bitmap.SetSource(stream);
          this.BitmapSize = stream.Size;
        }
      }

      //// AvailableFormats で、名前に "Clipboard" が入っているもの
      //ControlInfoDictionary.Clear();
      //var infoNames = content.AvailableFormats.Where(fmt => fmt.Contains("Clipboard"));
      //foreach(var name in infoNames)
      //{
      //  var value = (await content.GetDataAsync(name));
      //  ControlInfoDictionary.Add(name, $"{value.ToString()} ({value.GetType().Name})");
      //}

      //Properties.Clear();
      //foreach (var k in OriginalContent.Properties.Keys)
      //{
      //  Properties.Add(k, OriginalContent.Properties.GetValueOrDefault(k)?.ToString());
      //}
    }
  }
}
