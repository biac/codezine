using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace ClipboardViewer
{
  /// <summary>
  /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();

      // 説明用メソッドの呼び出し
      //Data.ClipboardHistoryData.GetClipboardHistoryAsync();

      this.AdaptiveGridViewControl.ItemsSource = Data.ClipboardHistoryData.Items;
      this.ClipboardCurrentGrid.DataContext = Data.ClipboardCurrentData.Instance;

      SetupEventHandlers();
    }
    
    void SetupEventHandlers()
    {
      // 現在のクリップボードの内容に変化があったとき
      // ※ ウインドウにフォーカスがないと取得に失敗する⇒フラグを立てておく
      bool notGetClipboardCurrentYet = true;
      Clipboard.ContentChanged += async (s, e) =>
      {
        var result = await Data.ClipboardCurrentData.TryUpdateAsync();
        notGetClipboardCurrentYet = !result;
      };

      // クリップボードの履歴に変化があったとき
      // ※ ウインドウにフォーカスがないと取得に失敗する⇒フラグを立てておく
      bool notGetClipboardHistoryYet = Clipboard.IsHistoryEnabled();
      Clipboard.HistoryChanged += async (s, e) =>
      {
        //var result = await Data.ClipboardHistoryData.TryUpdateAsync();
        // notGetClipboardHistoryYet = (result != ClipboardHistoryItemsResultStatus.Success);
        if (await Data.ClipboardHistoryData.TryUpdateAsync()
            != ClipboardHistoryItemsResultStatus.Success)
          notGetClipboardHistoryYet = true;
      };

      // ウインドウがフォーカスを受け取ったとき
      // ※ 取得失敗のフラグが立っていたら、取得してみる⇒成功したらフラグを倒す
      CoreWindow.GetForCurrentThread().Activated += async (s, e) =>
      {
        if (notGetClipboardCurrentYet)
        {
          if (await Data.ClipboardCurrentData.TryUpdateAsync())
            notGetClipboardCurrentYet = false;
        }
        if (notGetClipboardHistoryYet)
        {
          if (await Data.ClipboardHistoryData.TryUpdateAsync()
                == ClipboardHistoryItemsResultStatus.Success)
            notGetClipboardHistoryYet = false;
        }
      };

      // 最小化状態 (アイコン状態) から元のサイズに戻されたとき
      Application.Current.Resuming += async (s, e) =>
      {
        // 最小化時 (中断状態) の間の Clipboard.ContentChanged イベントは捨てられる
        // ので、復元時には必ず取得してみる。
        // ※ Clipboard.HistoryChanged イベントは復元後に発生する
        var result = await Data.ClipboardCurrentData.TryUpdateAsync();
        notGetClipboardCurrentYet = !result;
      };


      // クリップボード履歴の利用可否が切り替えられたとき
      this.IsHistoryEnabledText.Text = Clipboard.IsHistoryEnabled().ToString();
      Clipboard.HistoryEnabledChanged += (s, e) =>
      {
        this.IsHistoryEnabledText.Text = Clipboard.IsHistoryEnabled().ToString();
        TryUpdateBothDataAsync();
      };

      // クリップボード履歴のローミング可否が切り替えられたとき
      this.IsRoamingEnabledText.Text = Clipboard.IsRoamingEnabled().ToString();
      Clipboard.RoamingEnabledChanged += (s, e) =>
      {
        this.IsRoamingEnabledText.Text = Clipboard.IsRoamingEnabled().ToString();
        TryUpdateBothDataAsync();
      };

      async void TryUpdateBothDataAsync()
      {
        notGetClipboardCurrentYet = !(await Data.ClipboardCurrentData.TryUpdateAsync());
        notGetClipboardHistoryYet = (await Data.ClipboardHistoryData.TryUpdateAsync()
                != ClipboardHistoryItemsResultStatus.Success);
      }
    }

    // グリッドで履歴を選択した
    private void AdaptiveGridViewControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var selectedData = e.AddedItems.FirstOrDefault() as Data.ClipboardHistoryData;
      if (selectedData != null)
      {
        // 選択されたデータを、クリップボードのカレントにする
        Clipboard.SetHistoryItemAsContent(selectedData.HistoryItem);
      }
      else
      {
        // 選択が解除されたときは、履歴の先頭をカレントにする
        if (Data.ClipboardHistoryData.Items.Count > 0)
          Clipboard.SetHistoryItemAsContent(Data.ClipboardHistoryData.Items[0].HistoryItem);
      }
    }

    // 選択された履歴データを削除
    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
      //var selectedData = (sender as AppBarButton)?.Tag as Data.ClipboardHistoryData;

      var historyData = (sender as AppBarButton).DataContext as Data.ClipboardHistoryData;
      Clipboard.DeleteItemFromHistory(historyData.HistoryItem);
    }

    // 設定アプリのクリップボードのページを開く
    private async void OpenSettingsButton_Click(object sender, RoutedEventArgs e)
    {
      await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:clipboard"));
    }

    // クリップボードのクリア
    private void ClearClipboardCurrentButton_Click(object sender, RoutedEventArgs e)
    {
      Clipboard.Clear();
    }

    // 履歴のクリア
    private void DeleteClipboardHistoryButton_Click(object sender, RoutedEventArgs e)
    {
      Clipboard.ClearHistory();
    }
  }
}
