using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uwpDataTransfer = Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
//using Windows.UI.Xaml.Media.Imaging;
// ↑WPF からは UWP の BitmapImage クラスは使えない!
// ↓System.Windows.Media.Imaging の BitmapImage を使う。
using System.Windows.Media.Imaging;
using System.IO;

namespace WpfSample.Data
{
  public abstract class AbstractClipboardData
  {
    // クリップボードのデータ
    public BitmapImage Bitmap { get; private set; }
    public string TextHead { get; private set; }
    const int TextHeadLength = 1024;

    public IReadOnlyList<string> AvailableFormats { get; private set; }
    public bool IsFromRoamingClipboard { get; private set; }



    protected async Task SetDataAsync(uwpDataTransfer.DataPackageView content)
    {
      this.AvailableFormats = content.AvailableFormats?.ToList() ?? (new List<string>());
      this.IsFromRoamingClipboard = content.Properties.IsFromRoamingClipboard;

      // テキストデータ
      this.TextHead = string.Empty;
      if (content.Contains(uwpDataTransfer.StandardDataFormats.WebLink)) //"UniformResourceLocatorW"
        this.TextHead = (await content.GetWebLinkAsync()).ToString();
      if (content.Contains(uwpDataTransfer.StandardDataFormats.Text)) //"Text"
        this.TextHead = await content.GetTextAsync();

      if (this.TextHead.Length > TextHeadLength)
        this.TextHead = $"{this.TextHead.Substring(0, TextHeadLength)}(以下略)";

      // ビットマップデータ
      this.Bitmap = new BitmapImage();
      if (content.Contains(uwpDataTransfer.StandardDataFormats.Bitmap)) //"Bitmap"
      {
        // ビットマップデータの取り出し (ここは UWP と同じにはできない)
        RandomAccessStreamReference stRef = await content.GetBitmapAsync();
        using (IRandomAccessStreamWithContentType uwpStream = await stRef.OpenReadAsync())
        using (Stream stream = uwpStream.AsStreamForRead())
        {
          Bitmap.BeginInit();
          Bitmap.StreamSource = stream;
          Bitmap.CacheOption = BitmapCacheOption.OnLoad;
          Bitmap.EndInit();
          Bitmap.Freeze();
        }
      }
    }
  }
}
