using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;


// 添付のデータ list_person_all_extended_utf8.csv は、
// 青空文庫の「公開中　作家リスト：全て」ページ https://www.aozora.gr.jp/index_pages/person_all.html
// にある「公開中　作家別作品一覧拡充版：全て(CSV形式、UTF-8、zip圧縮）」
// (2018/10/22時点)

namespace NewUI1703.Data
{
  public class AozoraBunko
  {
    // 日本語名の変数を XAML にバインドすると 1703 ではリリースビルドに失敗する
    // ので、分かりにくくなるけれどもローマ字表記に変更した。
    public string SAKUHIN_ID { get; set; }
    public string SAKUHIN_MEI { get; set; }
    public string SAKUHIN_MEI_YOMI { get; set; }
    public string MOJITSUKAI_SHUBETSU { get; set; }

    public string SEI { get; set; }
    public string SEI_YOMI { get; set; }
    public string NA { get; set; }
    public string NA_YOMI { get; set; }
    public string YAKUWARI_Flag { get; set; }
    public string SEIMEI => $"{SEI}, {NA}";
    public string SEIMEI_YOMI => $"{SEI_YOMI} {NA_YOMI}";


    private AozoraBunko() { /* avoid instance */}

    private static List<AozoraBunko> _list;
    public static async Task<List<AozoraBunko>> GetListAsync()
    {
      if (_list != null)
        return _list;

      const int SAKUHIN_ID_Index = 0;
      const int SAKUHIN_MEI_Index = 1;
      const int SAKUHIN_MEI_YOMI_Index = 2;
      const int MOJITSUKAI_SHUBETSU_Index = 9;
      const int YAKUWARI_Flag_Index = 23;
      const int SEI_Index = 15;
      const int SEI_YOMI_Index = 17;
      const int NA_Index = 16;
      const int NA_YOMI_Index = 18;

      _list = new List<AozoraBunko>();
      bool skipped = false;
      foreach (string line in await GetCsvLinesAsync())
      {
        if (!skipped) // 1行目を読み飛ばす
        {
          skipped = true;
          continue;
        }

        string[] items = line.Split(',');
        _list.Add(new AozoraBunko
        {
          SAKUHIN_ID = items[SAKUHIN_ID_Index].Trim('\"'),
          SAKUHIN_MEI = items[SAKUHIN_MEI_Index].Trim('\"'),
          SAKUHIN_MEI_YOMI = items[SAKUHIN_MEI_YOMI_Index].Trim('\"'),
          MOJITSUKAI_SHUBETSU = items[MOJITSUKAI_SHUBETSU_Index].Trim('\"'),
          YAKUWARI_Flag = items[YAKUWARI_Flag_Index].Trim('\"'),
          SEI = items[SEI_Index].Trim('\"'),
          SEI_YOMI = items[SEI_YOMI_Index].Trim('\"'),
          NA = items[NA_Index].Trim('\"'),
          NA_YOMI = items[NA_YOMI_Index].Trim('\"'),
        });
      }
      return _list;
    }

    private static async Task<IEnumerable<string>> GetCsvLinesAsync()
    {
      const string CsvFileUri = "ms-appx:///Data/list_person_all_extended_utf8.csv";
      var csvFile
        = await StorageFile.GetFileFromApplicationUriAsync(new Uri(CsvFileUri));
      return await FileIO.ReadLinesAsync(csvFile);
    }
  }
}
