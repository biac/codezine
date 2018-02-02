using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace UF02UwpDesktop
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    private Version _osVersion;


    public MainWindow()
    {
      _osVersion = GetOsVersion();
      if (_osVersion.Major < 10)
        MessageBox.Show("このアプリは、Windows 10 でなければ正常に動作しません。",
          "動作不可", MessageBoxButton.OK, MessageBoxImage.Error);
      else if (_osVersion.Build < 14393)
        MessageBox.Show("このアプリは、Windows 10 build 14393 未満では一部の機能が動作しません。",
          "動作不完全", MessageBoxButton.OK, MessageBoxImage.Exclamation);

      InitializeComponent();

      //// My Picture フォルダーのファイル一覧を取得してみる
      //Task.Run(async () =>
      //{
      //  var files = await Windows.Storage.KnownFolders.PicturesLibrary.GetFilesAsync();
      //  var myPictures = string.Join("\n", files.Select(f => f.Name));
      //  MessageBox.Show(myPictures);
      //});

      this.Loaded += MainWindow_Loaded;
    }

    private Version GetOsVersion()
    {
      using (var mc = new System.Management.ManagementClass("Win32_OperatingSystem"))
      using (var moc = mc.GetInstances())
        foreach (System.Management.ManagementObject mo in moc)
        {
          var v = mo["Version"] as string;
          if (!string.IsNullOrWhiteSpace(v))
            return new Version(v);
        }

      return new Version("0.0.0.0");
    }

#if DEBUG
    async
#endif
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      ClearRubyText();

#if DEBUG
      InputText.Text = "日本語の漢字・カナ交じりの文字列です。山河と山川、須弥山登山計画";
#endif

#if DEBUG
      if (_osVersion.Major < 10)
        return;

      // 以下、説明用のコード (このアプリの動作には関係がない)

      // JapanesePhoneticAnalyzer は Windows 8.1 から使える
      // ただし、半角英数字を全角に変換してくれちゃうのが玉に瑕
      // http://www.atmarkit.co.jp/ait/articles/1511/25/news028.html
      IReadOnlyList<Windows.Globalization.JapanesePhoneme> list
        = Windows.Globalization.JapanesePhoneticAnalyzer.GetWords("日本語の文字列abc");
      foreach (var phoneme in list)
      {
        // 分割した文字列（形態素）
        string displayText = phoneme.DisplayText;

        // 分割した文字列の読み仮名
        string yomiText = phoneme.YomiText;

        // この形態素は句の先頭か？
        bool isPhraseStart = phoneme.IsPhraseStart;

        // 形態素ごとに何か処理をする
      }


      // 使える API
      var geolocator = new Windows.Devices.Geolocation.Geolocator();
      var geoposition = await geolocator.GetGeopositionAsync();

      // 使えない (DualApiPartition属性が付いていない)
      //bool isContract5Present
      //  = Windows.Foundation.Metadata.ApiInformation
      //      .IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5);

      // 使えない (API が package identity を必要とするため)
      try
      {
        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
      }
      catch (InvalidOperationException ex)
      {
        var exMsg = ex.Message;
        // exMsg: "プロセスにパッケージ ID がありません。 (HRESULT からの例外:0x80073D54)"
      }


      // 使える
      var gamepad = Windows.Gaming.Input.Gamepad.Gamepads.FirstOrDefault();

      // 使えない (UwpDesktop が 15063 に未対応なため)
      //var flightStick = Windows.Gaming.Input.FlightStick.FlightSticks.FirstOrDefault();
#endif
    }



    private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (_osVersion.Major < 10)
        return;

      string inputText = (sender as TextBox).Text;

      // 文字列全体の読み仮名を取得する（Win10全バージョン）
      string yomi = await GetReadingAndDisplay(inputText);

      if (_osVersion.Build >= 14393)
      {
        // 形態素分解して読み仮名を個別に取得する（14393以降）
        await GetRubyAndDisplay(inputText);
      }

      // 読み仮名から漢字に変換（Win10全バージョン）
      await ReconvertAndDisplay(yomi);
    }

    private async Task<string> GetReadingAndDisplay(string inputText)
    {
      // 読み仮名を取得するためのクラス
      var trcg = new Windows.Data.Text.TextReverseConversionGenerator("ja");

      // 文字列全体の読み仮名を取得する（Win10全バージョン）
      var yomi = await trcg.ConvertBackAsync(inputText);

      YomiText.Text = yomi;

      return yomi;
    }

    // 14393以前では、このメソッドを最初に呼び出そうとした時にTypeLoadException例外が出る
    // (14393以前ではTextPhoneme型がない)
    private async Task GetRubyAndDisplay(string inputText)
    {
      // 読み仮名を取得するためのクラス
      var trcg = new Windows.Data.Text.TextReverseConversionGenerator("ja");

      // 形態素分解して読み仮名を個別に取得する（14393以降）
      IReadOnlyList<Windows.Data.Text.TextPhoneme> textPhonemeList
        = await trcg.GetPhonemesAsync(inputText);

      // ルビ付きの文字列として表示
      ClearRubyText();
      foreach (var phoneme in textPhonemeList)
      {
        string text = phoneme.DisplayText;
        string ruby = phoneme.ReadingText;
        AppendTextAndRuby(text, ruby);
      }
    }

    private async Task ReconvertAndDisplay(string yomi)
    {
      // 読み仮名から漢字に変換（Win10全バージョン）
      var tcg = new Windows.Data.Text.TextConversionGenerator("ja");
      IReadOnlyList<string> candidatesList = await tcg.GetCandidatesAsync(yomi);

      ListBox1.ItemsSource = candidatesList;
    }




    private void ClearRubyText()
    {
      P1.Inlines.Clear();
    }

    private void AppendTextAndRuby(string text, string ruby)
    {
      if (ruby == text)
        ruby = string.Empty;

      P1.Inlines.Add(
        new InlineUIContainer
        {
          Child = new TextWithRubyControl
          {
            Body = text,
            Ruby = ruby,
          },
        }
      );
    }
  }
}
