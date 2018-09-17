using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FileManagerSample
{
  public class StorageItemTemplateSelector : DataTemplateSelector
  {
    // 選択対象のデータテンプレート
    // その実体はXAML側からセットされる
    public DataTemplate FolderTemplate { get; set; }
    public DataTemplate FileTemplate { get; set; }

    // 渡されたTreeViewNodeのContentがフォルダーだったらFolderTemplateを返す。
    // それ以外のときはFileTemplateを返す。
    protected override DataTemplate SelectTemplateCore(object item)
      => ((item as TreeViewNode)?.Content is StorageFolder)
          ? FolderTemplate : FileTemplate;
  }
}
