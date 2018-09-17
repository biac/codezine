using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace FileManagerSample
{
  /// <summary>
  /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
  /// </summary>
  public sealed partial class MainPage : Page
  {
    // フォルダー／ファイルの情報を表示するためのデータ
    private StorageItemInfo ItemInfo { get; set; }


    public MainPage()
    {
      this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      await InitializeTreeViewAsync();

      //// GetFolderFromPathAsync() の動作確認
      //IStorageItem winFolder
      //  = await StorageFolder.GetFolderFromPathAsync(@"c:\windows");
      //await (new Windows.UI.Popups.MessageDialog(winFolder.Path)).ShowAsync();
      //// GetFileFromPathAsync() の動作確認
      //IStorageItem notepadFile
      //  = await StorageFile.GetFileFromPathAsync(@"c:\windows\NotePad.exe");
      //await (new Windows.UI.Popups.MessageDialog(notepadFile.Path)).ShowAsync();
    }

    private async Task InitializeTreeViewAsync()
    {
      // 全てのドライブレターを取得する
      string[] drives = System.IO.Directory.GetLogicalDrives();

      try
      {
        foreach (var drive in drives)
        {
          // 各ドライブのルートフォルダーを取得する
          StorageFolder folder 
            = await StorageFolder.GetFolderFromPathAsync(drive);

          // TreeViewのルートノードに追加する
          this.TreeView1.RootNodes.Add(new TreeViewNode()
          {
            IsExpanded = false,
            HasUnrealizedChildren = true,
            Content = folder,
          });
        }
      }
      catch (UnauthorizedAccessException)
      {
        var msg = @"ファイル システムにアクセスできません。
設定がオンになっているかご確認ください。

これから設定アプリを開きます。このアプリは終了します。";
        await(new Windows.UI.Popups.MessageDialog(msg)).ShowAsync();
        await Windows.System.Launcher.LaunchUriAsync(
                new Uri("ms-settings:privacy-broadfilesystemaccess"));
        Windows.ApplicationModel.Core.CoreApplication.Exit();
      }
    }

    private async void TreeView1_Expanding(TreeView sender, TreeViewExpandingEventArgs args)
    {
      bool searched = !args.Node.HasUnrealizedChildren;
      if (searched) // 検索済みなら何もしない
        return;

      sender.IsEnabled = false;

      try
      {
        var folder = args.Node.Content as StorageFolder;

        // フォルダー配下のIStorageItemオブジェクトを検索する
        var items = await folder.GetItemsAsync();

        // 見つかったIStorageItemオブジェクトをノードに追加する
        foreach (var storageItem in items)
          args.Node.Children.Add(new TreeViewNode()
          {
            IsExpanded = false,
            Content = storageItem,
            HasUnrealizedChildren = storageItem is StorageFolder,
          });
      }
      finally
      {
        // 検索を実行したので、必ずHasUnrealizedChildrenはfalseにする
        args.Node.HasUnrealizedChildren = false;

        sender.IsEnabled = true;
      }
    }

    private async void TreeView1_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
    {
      if (args.InvokedItem is TreeViewNode node)
      {
        this.ItemInfo = await StorageItemInfo.Create(node.Content as IStorageItem);
        this.Bindings.Update();

        if (node.Content is StorageFolder)
          node.IsExpanded = !node.IsExpanded;

        Windows.UI.ViewManagement.ApplicationView
          .GetForCurrentView().Title = (node.Content as IStorageItem).Path;
      }
    }
  }
}
