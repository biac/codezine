using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsbnUwp
{
  public static partial class License
  {
    // このライセンスキー文字列は、
    // 「ComponentOne for UWPライセンス」の説明に従ってライセンスキーを取得して、置き換える
    // （このままでは動作しません）
    // https://www.grapecity.co.jp/developer/license/componentone/uwp-license
    public static string Key { get; } =
        "ABYBFgIWB29JAHMAYgBuAFUAdwBwAGodFTVyyndYMg3iRRRVHTyjhw4x22esreNY" +
        "abCYWalo5xrq3rQTS9OtUNVFuzc8SH5Wi/uLkd4yL6j9laUzUMNKgMnbhy+prMbJ" +
        // ……中略……
        "gSi4YwCnxjgdhaSMd//qwJgTl17ZRaI6DiBKTfc2CqU7JKLsu4h5CkiENHQ6gOo/" +
        "pPuc4cuPINx5hmI=";
  }
}
