using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace ClipboardViewer.Data
{
  public class ClipboardCurrentData : AbstractClipboardData, INotifyPropertyChanged
  {
    // 唯一のインスタンス
    public static ClipboardCurrentData Instance { get; } = new ClipboardCurrentData();

    public event PropertyChangedEventHandler PropertyChanged;

    // 表示用のデータ
    // ツールチップの文字列
    public string TooltipText
    {
      get {
        return $@"AvailableFormats:
{string.Join("\n", AvailableFormats)}

Text: {TextSize:#,##0}Bytes
Bitmap: {BitmapSize:#,##0}Bytes ({BitmapWidth}x{BitmapHeight})
IsFromRoamingClipboard: {IsFromRoamingClipboard}";
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
      await base.SetDataAsync(blankData);
    }

    // データ更新時、変更を通知するプロパティ名
    private readonly string[] NotifyProperties
      = { nameof(TextHead), nameof(Bitmap),
          nameof(AvailableFormats),
          nameof(IsFromRoamingClipboard), nameof(TooltipText), };

    // データバインディング先に変更を通知する
    private void Notify()
    {
      foreach (string p in NotifyProperties)
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
  }
}
