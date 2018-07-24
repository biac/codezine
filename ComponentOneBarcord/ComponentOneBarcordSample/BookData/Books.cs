using C1.Win.FlexReport;
using System;
using System.Collections.Generic;

namespace BookData
{
  public class Book
  {
    public string Title { get; set; }
    public string Isbn { get; set; }
    public string Price { get; set; }

    public string IsbnWithoutCheckDigit
      => (Isbn?.Length > 12) ? Isbn.Substring(0, 12) : string.Empty;

    public Book(string title, string isbn, string price)
    {
      this.Title = title;
      this.Isbn = isbn;
      this.Price = price;
    }
  }



  // FlexReport デザイナーが読み込めるコレクション
  // IC1FlexReportRecordset と IC1FlexReportExternalRecordset を実装する必要がある
  // ※ FlexReport for UWP は、IList<T> で OK。このコードは、デザイナーを使うためだけに必要なもの。

  // 詳細は、「GrapeCity.devlog」ブログの「動的に生成されるサンプルデータを利用した帳票デザイン」
  // の中の「カスタムデータソース」の項を参照
  // https://devlog.grapecity.co.jp/entry/2017/05/02/c1-flexreport-design

  public class Books : IC1FlexReportRecordset, IC1FlexReportExternalRecordset
  {
    private static List<Book> _books = new List<Book>() {
          new Book(title:"Effective C# 6.0/7.0",
                   isbn:"9784798153865", price:"￥3,456"),
          new Book(title:"More Effective C# 6.0/7.0",
                   isbn:"9784798153988", price:"￥3,672"),
          new Book(title:"テスト駆動Python",
                   isbn:"9784798157603", price:"￥3,024"),
          new Book(title:"LaTeX2ε辞典 増補改訂版",
                   isbn:"9784798157078", price:"￥3,218"),
          new Book(title:"正規表現辞典 改訂新版",
                   isbn:"9784798156422", price:"￥2,916"),
          new Book(title:"Xamarinネイティブによるモバイルアプリ開発",
                   isbn:"9784798149813", price:"￥3,758"),
          new Book(title:"最強囲碁AI アルファ碁 解体新書 増補改訂版",
                   isbn:"9784798157771", price:"￥2,894"),
        };

    public static IList<Book> GetData()
    {
      return _books;
    }



    // 以下、IC1FlexReportRecordset インターフェイスの実装

    public int Count => _books.Count;

    private int _bkmk;

    public bool BOF() => (_bkmk <= 0);

    public bool EOF() => (_bkmk >= _books.Count);

    public int GetBookmark() => _bkmk;

    public string[] GetFieldNames()
      => new string[] { "Title", "Isbn", "Price", "IsbnWithoutCheckDigit", };

    public Type[] GetFieldTypes()
      => new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), };

    public object GetFieldValue(int fieldIndex)
    {
      var book = _books[_bkmk];
      switch (fieldIndex)
      {
        case 0:
          return book.Title;
        case 1:
          return book.Isbn;
        case 2:
          return book.Price;
        case 3:
          return book.IsbnWithoutCheckDigit;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void MoveFirst() => _bkmk = 0;

    public void MoveLast() => _bkmk = _books.Count - 1;

    public void MoveNext() => _bkmk++;

    public void MovePrevious() => _bkmk--;

    public void SetBookmark(int bkmk) => _bkmk = bkmk;



    // 以下、IC1FlexReportExternalRecordset インターフェイスの実装
    public string Caption => "BooksReport";

    public string Params
    {
      get => _books.Count.ToString();
      set { /* (void) */}
    }

    public void EditParams()
    {
      /* (void) */
    }

    public IC1FlexReportRecordset GetRecordset() => this;
  }
}
