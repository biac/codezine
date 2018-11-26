using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace WpfSample.Data
{
  public class ClipboardHistoryData : AbstractClipboardData
  {
    public static ObservableCollection<ClipboardHistoryData> Items { get; }
      = new ObservableCollection<ClipboardHistoryData>();

    // クリップボード履歴のデータ
    public ClipboardHistoryItem HistoryItem { get; private set; }
    public string Id => HistoryItem.Id;
    public DateTimeOffset Timestamp => HistoryItem.Timestamp;

    // 表示用
    public string TimestampTime => Timestamp.ToString("HH:mm:ss");
    public string TooltipText
      => $@"AvailableFormats:
{string.Join("\n", AvailableFormats)}

IsFromRoamingClipboard: {IsFromRoamingClipboard}
ID: {Id}
Timestamp: {TimestampTime}";



    public static async Task<ClipboardHistoryItemsResultStatus> TryUpdateAsync()
    {
      if(!Clipboard.IsHistoryEnabled())
      {
        Items.Clear();
        return ClipboardHistoryItemsResultStatus.ClipboardHistoryDisabled;
      }

      var result = await Clipboard.GetHistoryItemsAsync();
      // GetHistoryItemsAsync() は、履歴の取得に失敗すると
      // Success 以外 (=AccessDenied または ClipboardHistoryDisabled) を返してくる
      if (result.Status != ClipboardHistoryItemsResultStatus.Success)
      {
        return result.Status;
      }

      Items.Clear();
      foreach (ClipboardHistoryItem item in result.Items)
        Items.Add(await CreateNewDataAsync(item));

      return ClipboardHistoryItemsResultStatus.Success;
    }


    private ClipboardHistoryData()
    {
      // (avoid instance)
    }

    private static async Task<ClipboardHistoryData> CreateNewDataAsync(ClipboardHistoryItem item)
    {
      var newItem = new ClipboardHistoryData()
      {
        HistoryItem = item,
      };
      await newItem.SetDataAsync(item.Content);
      return newItem;
    }
  }
}
