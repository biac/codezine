using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SQLiteSample
{
  public class ArticleDatabase
  {
    // 参考:
    // Entity Framework Core with Xamarin.Forms
    // https://xamarinhelp.com/entity-framework-core-xamarin-forms/


    public static ObservableCollection<Article> ArticlesList { get; private set; }


    public static void InitForAndroid()
    {
      ArticleContext.InitConnectionStringForAndroid();
    }

    public static void InitForIOS()
    {
      SQLitePCL.Batteries.Init();
      ArticleContext.InitConnectionStringForIOS();
    }

    public static async Task<ObservableCollection<Article>> LoadAsync()
    {
      using (var context = new ArticleContext())
      {
#if DEBUG
        //context.Database.EnsureDeleted(); // DB 削除
#endif

        await context.Database.EnsureCreatedAsync(CancellationToken.None);
        await EnsureInitialDataAsync();

        ArticlesList = new ObservableCollection<Article>(await context.Articles.ToListAsync());
        return ArticlesList;

        async Task EnsureInitialDataAsync()
        {
          if (await context.Articles.FirstOrDefaultAsync() != null)
            return;

          // set initial data
          context.Articles.Add(new Article(1)
          {
            Title = "UWPアプリを書けばiOS／Android／Webでも動く!?　～Uno Platform：クロスプラットフォーム開発環境",
            Url = "https://codezine.jp/article/detail/11795"
          });
          context.Articles.Add(new Article(2)
          {
            Title = "Uno Platform - Home",
            Url = "https://platform.uno/"
          });
          context.Articles.Add(new Article(3)
          {
            Title = "Uno Platform Team Blog",
            Url = "https://platform.uno/blog/"
          });
          var count = await context.SaveChangesAsync(CancellationToken.None);
#if DEBUG
          Console.WriteLine("{0} records saved to database", count);
#endif
        }
      } //end using
    }

    // 仮のデータを作成して、ArticlesListに追加する。
    // 仮のデータは、ArticleIdが負の値で、TitleとURLは空。DBには追加しない。
    public static Article CreateTempData()
    {
      int tempIndex = -1;
      if (ArticlesList.Count > 0)
      {
        int minId = ArticlesList.Min(m => m.ArticleId);
        if (minId < 0)
          tempIndex = minId - 1;
      }

      var tempData = new Article(tempIndex);
      ArticlesList.Add(tempData);

      return tempData;
    }

    // DBへの登録・更新
    // 仮のデータのupdateは、DBへの新規登録となり、ArticleIdが採番される。
    public static async Task<Article> UpdateAsync(Article originalArticle, Article newArticle)
    {
      if (!ArticlesList.Contains(originalArticle))
        throw new ArgumentException("指定されたArticleオブジェクトはArticlesListに含まれていません。");

      if (originalArticle.ArticleId != newArticle.ArticleId)
        throw new ArgumentException("2つの引数でArticleIdが違います。");

      if (originalArticle.Title == newArticle.Title && originalArticle.Url == newArticle.Url)
        return originalArticle;

      Article modifiedArticle;
      if (originalArticle.ArticleId > 0)
        modifiedArticle = await UpdateDataAsync(originalArticle, newArticle);
      else
        modifiedArticle = await InsertDataAsync(newArticle);

      // ObservableCollectionのデータを入れ替える（これでNotifyChangedが画面側に飛ぶ）
      int updateItemPosition = ArticlesList.IndexOf(originalArticle);
      ArticlesList.RemoveAt(updateItemPosition);
      ArticlesList.Insert(updateItemPosition, modifiedArticle);

      return modifiedArticle;

      async Task<Article> UpdateDataAsync(Article original, Article newData)
      {
        // 既存データの更新
        using (var context = new ArticleContext())
        {
          // データベースから更新対象のデータを取ってくる
          var targetItem
            = await context.Articles.SingleAsync(m => m.ArticleId == original.ArticleId);

          // 更新対象のデータを書き換え
          targetItem.Title = newData.Title;
          targetItem.Url = newData.Url;

          // データベースに反映
          await context.SaveChangesAsync(CancellationToken.None);

          return targetItem;
        }
      }
      async Task<Article> InsertDataAsync(Article article)
      {
        // 新規データを追加（自動採番）
        using (var context = new ArticleContext())
        {
          var newEntry = context.Articles.Add(new Article
          {
            Title = article.Title,
            Url = article.Url,
          });
          await context.SaveChangesAsync(CancellationToken.None);

          return newEntry.Entity;
        }
      }
    }

    public static async Task DeleteAsync(Article article)
    {
      if (!ArticlesList.Contains(article))
        throw new ArgumentException("指定されたArticleオブジェクトはArticlesListに含まれていません。");

      if (article.ArticleId > 0)
      {
        using (var context = new ArticleContext())
        {
          var targetItem
            = await context.Articles.SingleAsync(m => m.ArticleId == article.ArticleId);

          context.Articles.Remove(targetItem);
          await context.SaveChangesAsync(CancellationToken.None);
        }
      }

      ArticlesList.Remove(article);
    }


#if DEBUG
    // ADO.NETも利用可能（Microsoft.EntityFrameworkCore.Sqliteと一緒にMicrosoft.Data.Sqliteも入っている）
    public static async Task AdoDotNetSampleAsync()
    {
      using (var conn = new Microsoft.Data.Sqlite.SqliteConnection(ArticleContext._connectionString))
      {
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM Articles";
        using (var reader = cmd.ExecuteReader())
          while (await reader.ReadAsync())
            Console.WriteLine($"{reader.GetInt32(0)} - {reader.GetString(1)}, {reader.GetString(2)}");
      }
    }
#endif
  }
}
