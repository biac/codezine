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

        context.Database.EnsureCreated();
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
        throw new ArgumentException("No data to update on memory");

      if (originalArticle.ArticleId != newArticle.ArticleId)
        throw new ArgumentException("Both parameters must have same ArticleId");

      if (originalArticle.Title == newArticle.Title && originalArticle.Url == newArticle.Url)
        return originalArticle;

      Article modifiedArticle;
      if (originalArticle.ArticleId > 0)
        modifiedArticle = await UpdateAsync(originalArticle, newArticle);
      else
        modifiedArticle = await InsertAsync(newArticle);

      int updateItemPosition = ArticlesList.IndexOf(originalArticle);
      ArticlesList.RemoveAt(updateItemPosition);
      ArticlesList.Insert(updateItemPosition, modifiedArticle);

      return modifiedArticle;

      async Task<Article> UpdateAsync(Article original, Article newData)
      {
        // 既存データの更新
        using (var context = new ArticleContext())
        using (var tran = await context.Database.BeginTransactionAsync())
        {
          // データベースから更新対象のデータを取ってくる
          var targetItem 
            = await context.Articles.SingleOrDefaultAsync(m => m.ArticleId == original.ArticleId);

          // 前回取得時からデータが変更されていないかをチェック
          if (targetItem == null)
            throw new ArgumentException("No data to update on DB");
          if (original.Title != targetItem.Title || original.Url != targetItem.Url)
            throw new ApplicationException("The data has already been changed on DB");

          // 更新対象のデータを書き換え
          targetItem.Title = newData.Title;
          targetItem.Url = newData.Url;

          // データベースに反映
          await context.SaveChangesAsync(CancellationToken.None);

          tran.Commit();
          return targetItem;
        }
      }
      async Task<Article> InsertAsync(Article article)
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
        throw new ArgumentException("No data to delete on memory");

      if (article.ArticleId > 0)
      {
        using (var context = new ArticleContext())
        using (var tran = await context.Database.BeginTransactionAsync())
        {
          var targetItem 
            = await context.Articles.SingleOrDefaultAsync(m => m.ArticleId == article.ArticleId);
          if (targetItem == null)
            throw new ArgumentException("No data to delete on DB");

          context.Articles.Remove(targetItem);
          await context.SaveChangesAsync(CancellationToken.None);

          tran.Commit();
        }
      }

      ArticlesList.Remove(article);
    }
   }
}
