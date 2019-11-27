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
      using (var db = new ArticleContext())
      {
#if DEBUG
        //db.Database.EnsureDeleted(); // DB 削除
#endif

        db.Database.EnsureCreated();
        await EnsureInitialDataAsync();

        ArticlesList = new ObservableCollection<Article>(await db.Articles.ToListAsync());
        return ArticlesList;

        async Task EnsureInitialDataAsync()
        {
          if (await db.Articles.FirstOrDefaultAsync() != null)
            return;

          // set initial data
          db.Articles.Add(new Article
          {
            ArticleId = 1,
            Title = "UWPアプリを書けばiOS／Android／Webでも動く!?　～Uno Platform：クロスプラットフォーム開発環境",
            Url = "https://codezine.jp/article/detail/11795"
          });
          db.Articles.Add(new Article
          {
            ArticleId = 2,
            Title = "Uno Platform - Home",
            Url = "https://platform.uno/"
          });
          db.Articles.Add(new Article
          {
            ArticleId = 3,
            Title = "Uno Platform Team Blog",
            Url = "https://platform.uno/blog/"
          });
          var count = await db.SaveChangesAsync(CancellationToken.None);
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
      int minId = ArticlesList.Min(m => m.ArticleId);
      if (minId < 0)
        tempIndex = minId - 1;

      var tempData = new Article { ArticleId = tempIndex };
      ArticlesList.Add(tempData);

      return tempData;
    }

    // DBへの登録・更新
    // 仮のデータのupdateは、DBへの新規登録となり、ArticleIdが採番される。
    public static async Task<Article> UpdateAsync(Article originalArticle, Article newArticle)
    {
      if (!ArticlesList.Contains(originalArticle))
        throw new ArgumentException("No data to update on memory");

      if (originalArticle.ArticleId != newArticle.ArticleId)
        throw new ArgumentException("Both parameters must have same ArticleId");

      if (originalArticle.Title == newArticle.Title && originalArticle.Url == newArticle.Url)
        return originalArticle;

      Article modifiedArticle;
      if (originalArticle.ArticleId > 0)
        modifiedArticle = await UpdateAsync();
      else
        modifiedArticle = await InsertAsync(newArticle);

      int updateItemPosition = ArticlesList.IndexOf(originalArticle);
      ArticlesList.RemoveAt(updateItemPosition);
      ArticlesList.Insert(updateItemPosition, modifiedArticle);

      return modifiedArticle;

      async Task<Article> UpdateAsync()
      {
        // 既存データの更新
        using (var db = new ArticleContext())
        using (var tran = await db.Database.BeginTransactionAsync())
        {
          var targetItem = db.Articles.FirstOrDefault(m => m.ArticleId == originalArticle.ArticleId);
          if (targetItem == null)
            throw new ArgumentException("No data to update on DB");

          if (originalArticle.Title != targetItem.Title || originalArticle.Url != targetItem.Url)
            throw new ApplicationException("The data has already been changed on DB");

          targetItem.Title = newArticle.Title;
          targetItem.Url = newArticle.Url;
          var updatedEntry = db.Articles.Update(targetItem);
          await db.SaveChangesAsync(CancellationToken.None);

          tran.Commit();
          return updatedEntry.Entity;
        }
      }
      async Task<Article> InsertAsync(Article article)
      {
        // 新規データを追加（自動採番）
        using (var db = new ArticleContext())
        {
          var newEntry = db.Articles.Add(new Article
          {
            Title = article.Title,
            Url = article.Url,
          });
          await db.SaveChangesAsync(CancellationToken.None);

          return newEntry.Entity;
        }
      }
    }

    public static async Task DeleteAsync(Article article)
    {
      if (!ArticlesList.Contains(article))
        throw new ArgumentException("No data to delete on memory");

      if (article.ArticleId > 0)
      {
        using (var db = new ArticleContext())
        using (var tran = await db.Database.BeginTransactionAsync())
        {
          var targetItem = db.Articles.FirstOrDefault(m => m.ArticleId == article.ArticleId);
          if (targetItem == null)
            throw new ArgumentException("No data to delete on DB");

          db.Articles.Remove(targetItem);
          int count = await db.SaveChangesAsync(CancellationToken.None);
          if (count < 1)
            throw new ApplicationException("Fail to delete on DB");

          tran.Commit();
        }
      }

      ArticlesList.Remove(article);
    }
   }
}
