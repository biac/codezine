using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;


// 添付のデータ list_person_all_extended_utf8.csv は、
// 青空文庫の「公開中　作家リスト：全て」ページ https://www.aozora.gr.jp/index_pages/person_all.html
// にある「公開中　作家別作品一覧拡充版：全て(CSV形式、UTF-8、zip圧縮）」
// (2018/10/22時点)

namespace NewUI1809.Data
{
  public class AozoraBunko
  {
    public string 作品ID { get; set; }
    public string 作品名 { get; set; }
    public string 作品名読み { get; set; }
    public string 文字遣い種別 { get; set; }

    public string 姓 { get; set; }
    public string 姓読み { get; set; }
    public string 名 { get; set; }
    public string 名読み { get; set; }
    public string 役割フラグ { get; set; }
    public string 姓名 => $"{姓}, {名}";
    public string 姓名読み => $"{姓読み} {名読み}";


    private AozoraBunko() { /* avoid instance */}

    private static List<AozoraBunko> _list;
    public static async Task<List<AozoraBunko>> GetListAsync()
    {
      if (_list != null)
        return _list;

      const int 作品IdIndex = 0;
      const int 作品名Index = 1;
      const int 作品名読みIndex = 2;
      const int 文字遣い種別Index = 9;
      const int 役割フラグIndex = 23;
      const int 姓Index = 15;
      const int 姓読みIndex = 17;
      const int 名Index = 16;
      const int 名読みIndex = 18;

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
          作品ID = items[作品IdIndex].Trim('\"'),
          作品名 = items[作品名Index].Trim('\"'),
          作品名読み = items[作品名読みIndex].Trim('\"'),
          文字遣い種別 = items[文字遣い種別Index].Trim('\"'),
          役割フラグ = items[役割フラグIndex].Trim('\"'),
          姓 = items[姓Index].Trim('\"'),
          姓読み = items[姓読みIndex].Trim('\"'),
          名 = items[名Index].Trim('\"'),
          名読み = items[名読みIndex].Trim('\"'),
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
