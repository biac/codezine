using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace FileManagerSample
{
  public class StorageItemInfo
  {
    public string Name { get; private set; } // フォルダー名／ファイル名
    public string Path { get; private set; } // フォルダー／ファイルまでのパス
    public DateTimeOffset DateCreated { get; private set; } // 作成日時
    public FileAttributes Attributes { get; private set; } // 属性
    public string DisplayType { get; private set; } // 種類
    public ulong Size { get; private set; } // サイズ
    public Windows.UI.Xaml.Media.ImageSource Thumbnail { get; private set; } // サムネイル

    // プロパティを表示用にフォーマットした文字列
    public string DateCreatedString => DateCreated.ToString("yyyy/MM/dd HH:mm:ss");
    public string SizeString => Size.ToString("#,##0");


    private StorageItemInfo() { /* (avoid instance) */}

    public static async Task<StorageItemInfo> Create(IStorageItem item)
    {
      var itemProperties = item as IStorageItemProperties;

      // サムネイルをビットマップとして取得する（ローカル関数）
      async Task<Windows.UI.Xaml.Media.Imaging.BitmapImage>
        GetThumbnailBitmapAsync()
      {
        var bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
        StorageItemThumbnail thumbnail
          = await itemProperties?.GetThumbnailAsync(ThumbnailMode.SingleItem, 256);
        if (thumbnail != null)
          bitmapImage.SetSource(thumbnail);
        return bitmapImage;
      }

      // StorageItemInfoオブジェクトを作る
      var itemInfo = new StorageItemInfo()
      {
        Name = item.Name,
        Path = System.IO.Path.GetDirectoryName(item.Path),
        DateCreated = item.DateCreated,
        Attributes = item.Attributes,
        DisplayType = itemProperties?.DisplayType,
        Thumbnail = await GetThumbnailBitmapAsync(),
      };

      // ファイルの場合、ファイルサイズもStorageItemInfoに追加する
      if (item is StorageFile file)
      {
        var basicProperties = await file.GetBasicPropertiesAsync();
        itemInfo.Size = basicProperties.Size;
      }

      return itemInfo;
    }
  }
}
