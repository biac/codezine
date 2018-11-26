using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace ClipboardViewer.Data
{
  public class ClipboardHistoryData : AbstractClipboardData
  {
    public static ObservableCollection<ClipboardHistoryData> Items { get; }
      = new ObservableCollection<ClipboardHistoryData>();

    // クリップボード履歴のデータ
    public ClipboardHistoryItem HistoryItem { get; private set; }
    public string Id => HistoryItem.Id;
    public DateTimeOffset Timestamp => HistoryItem.Timestamp;

    // 表示用のデータ
    public string TimestampTime => Timestamp.ToString("HH:mm:ss");
    public string TooltipText
      => $@"AvailableFormats:
{string.Join("\n", AvailableFormats)}

Text: {TextSize:#,##0}Bytes
Bitmap: {BitmapSize:#,##0}Bytes ({BitmapWidth}x{BitmapHeight})
IsFromRoamingClipboard: {IsFromRoamingClipboard}
ID: {Id}
Timestamp: {TimestampTime}";


    // 履歴を取得する基本的な流れ（説明用）
    public static async Task GetClipboardHistoryAsync()
    {
      if (!Clipboard.IsHistoryEnabled())
        return; // 設定で履歴が無効にされている

      var result = await Clipboard.GetHistoryItemsAsync();
      // 履歴の取得に失敗したときは、result.Statusに
      // Success以外（AccessDeniedまたはClipboardHistoryDisabled）が返される
      if (result.Status != ClipboardHistoryItemsResultStatus.Success)
        return;

      // 履歴のリストを取り出す
      IReadOnlyList<ClipboardHistoryItem> historyList = result.Items;

      // それぞれの履歴アイテムを処理する
      foreach (ClipboardHistoryItem item in historyList)
      {
        // 履歴アイテムのIDとタイムスタンプ
        string id = item.Id;
        DateTimeOffset timestamp = item.Timestamp;

        // データ（クリップボードのデータと同じ型）
        DataPackageView content = item.Content;

        // テキストデータを取り出す例
        if (content.Contains(StandardDataFormats.Text))
        {
          string textData = await content.GetTextAsync();
          // DataPackageViewに入っているデータは、
          // このようにして非同期メソッドを使って取得する
        }

        // データとして入っているフォーマットの一覧
        List<string> formats = content.AvailableFormats.ToList();
        // content.AvailableFormatsはSystem.__ComObjectのリストなので、
        // ToList拡張メソッドを使って「こっちの世界に固定する」

        // 全てのデータを取りだす例
        foreach (string format in formats)
        {
          object data = await content.GetDataAsync(format);

          //Type dataType = data.GetType();
          //if (dataType.FullName == "System.__ComObject")
          //{
          //  // ただし、GetTypeしてもSystem.__ComObject型が返ってきてしまい、
          //  // その実体が何であるか不明なデータもある
          //  var types = dataType.GetInterfaces(); //←ITypeInfoを実装していないのでは何だか分からない
          //}
        }

        // ローミングされてクリップボードに入れられたデータかどうか
        bool isFromRoamingClipboard = content.Properties.IsFromRoamingClipboard;
      }
    }


    public static async Task<ClipboardHistoryItemsResultStatus> TryUpdateAsync()
    {
      if(!Clipboard.IsHistoryEnabled())
      {
        // 履歴がOFFになっている
        Items.Clear();
        return ClipboardHistoryItemsResultStatus.ClipboardHistoryDisabled;
      }

      var result = await Clipboard.GetHistoryItemsAsync();
      // GetHistoryItemsAsync() は、履歴の取得に失敗すると
      // Success 以外 (=AccessDenied または ClipboardHistoryDisabled) を返してくる
      if (result.Status != ClipboardHistoryItemsResultStatus.Success)
      {
        // 履歴リストの取得に失敗した
        return result.Status;
      }

      // 履歴リストが取得できたので、コレクションを更新する
      Items.Clear();
      foreach (ClipboardHistoryItem item in result.Items)
        Items.Add(await CreateNewDataAsync(item));
      // ※ このやり方では、UWPの場合はスクロールがリセットされてしまう。
      //    スクロール位置を維持するためには、Clear() せずに、
      //    新旧のデータを比較して1個ずつ追加・削除する。

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
