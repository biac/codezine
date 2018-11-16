using System;
using System.Collections.Generic;
using System.Linq;
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
    public BitmapImage Bitmap { get; private set; }
    public string Text { get; private set; }

    //protected DataPackageView OriginalContent { get; private set; }
    public IReadOnlyList<string> AvailableFormats { get; private set; }//=> OriginalContent.AvailableFormats;
    public bool IsFromRoamingClipboard { get; private set; }//=> OriginalContent.Properties.IsFromRoamingClipboard;

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

      //this.OriginalContent = content;
      this.AvailableFormats = content.AvailableFormats?.ToList() ?? (new List<string>());
      this.IsFromRoamingClipboard = content.Properties.IsFromRoamingClipboard;

      // テキストデータ（該当するデータが複数あるときは、最後のもの）
      this.Text = string.Empty;
      //if (content.Contains(StandardDataFormats.WebLink)) //"UniformResourceLocatorW"
      //  this.Text = (await content.GetWebLinkAsync()).ToString();
      //if (content.Contains(StandardDataFormats.Html)) //"HTML Format"
      //  this.Text = await content.GetHtmlFormatAsync();
      if (content.Contains(StandardDataFormats.Text)) //"Text"
        this.Text = await content.GetTextAsync();

      // ビットマップデータ
      this.Bitmap = new BitmapImage();
      if (content.Contains(StandardDataFormats.Bitmap)) //"Bitmap"
      {
        RandomAccessStreamReference stRef = await content.GetBitmapAsync();
        using (IRandomAccessStreamWithContentType stream = await stRef.OpenReadAsync())
        {
          this.Bitmap.SetSource(stream);
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
