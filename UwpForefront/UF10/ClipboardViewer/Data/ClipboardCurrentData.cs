using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace ClipboardViewer.Data
{
  public class ClipboardCurrentData : AbstractClipboardData, INotifyPropertyChanged
  {
    public static ClipboardCurrentData Instance { get; } = new ClipboardCurrentData();

    public event PropertyChangedEventHandler PropertyChanged;

    //// クリップボードのデータ
    //public DataPackageView OriginalContent { get; private set; }
    //public IReadOnlyList<string> AvailableFormats => OriginalContent.AvailableFormats;
    //public bool IsFromRoamingClipboard => OriginalContent.Properties.IsFromRoamingClipboard;

    public string TooltipText
    {
      get {
        //if (OriginalContent.AvailableFormats == null
        //    || OriginalContent.AvailableFormats?.Count == 0)
        //  return null;

        return $@"AvailableFormats:
{string.Join("\n", AvailableFormats)}

IsFromRoamingClipboard: {IsFromRoamingClipboard}";
//{string.Join("\n", ControlInfoDictionary.Select(kv => $"{kv.Key}: {kv.Value}"))}
//{string.Join("\n", Properties.Select(kv => $"{kv.Key}: {kv.Value}"))}";
      }
    }


    public static async Task<bool> TryUpdateAsync()
    {
#if DEBUG
      try
      {
#endif
        DataPackageView dpv;
        try
        {
          dpv = Clipboard.GetContent();
          // Clipboard.GetContent() は、取得に失敗すると例外を出す。
        }
        catch (UnauthorizedAccessException)
        {
          return false;
        }

        if (dpv == null)
        {
          await Instance.ClearAsync();
          Instance.Notify();
          return false;
        }

        //Instance.OriginalContent = dpv;
        await Instance.SetDataAsync(dpv);
        Instance.Notify();
        return true;
#if DEBUG
      }
      catch (Exception ex)
      {
        await (new Windows.UI.Popups.MessageDialog(ex.ToString())).ShowAsync();
        return false;
      }
#endif
    }

    private ClipboardCurrentData()
    {
      // (avoid instance)
    }

    private async Task ClearAsync()
    {
      var blankData = (new DataPackage()).GetView();
      //OriginalContent = blankData;
      await base.SetDataAsync(blankData);
    }

    private readonly string[] NotifyProperties
      = { nameof(Text), nameof(Bitmap),
          //nameof(OriginalContent),
          nameof(AvailableFormats),
          nameof(IsFromRoamingClipboard), nameof(TooltipText), };

    private void Notify()
    {
      foreach (string p in NotifyProperties)
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
  }
}
