using System.Collections.Generic;

namespace IsbnUwp
{
  // 書籍
  public class Book
  {
    // 書籍のタイトル
    public string Title { get; set; }
    // ISBNコード
    public string Isbn { get; set; }
    // 価格
    public string Price { get; set; }

    // ISBNコードから末尾のチェックデジットを抜いたもの
    public string IsbnWithoutCheckDigit
      => (Isbn?.Length > 12) ? Isbn.Substring(0, 12) : Isbn;

    public Book(string title, string isbn, string price)
    {
      this.Title = title;
      this.Isbn = isbn;
      this.Price = price;
    }
  }

  // 書籍のデータストア
  public class Books
  {
    private static List<Book> _books;

    public static IList<Book> GetData()
    {
      if (_books == null)
      {
        _books = new List<Book>() {
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
      }
      return _books;
    }
  }
}
