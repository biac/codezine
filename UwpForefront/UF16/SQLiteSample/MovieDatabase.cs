using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SQLiteSample
{
  public class MovieDatabase
  {

    // Xamarin Android データアクセス
    // https://docs.microsoft.com/ja-jp/xamarin/android/data-cloud/data-access/
    //internal static SqliteConnection Connection;

    // Entity Framework Core with Xamarin.Forms
    // https://xamarinhelp.com/entity-framework-core-xamarin-forms/


    public static ObservableCollection<Movie> MoviesList { get; private set; }


    public static void InitForAndroid()
    {
      MovieContext.InitConnectionStringForAndroid();
    }

    public static void InitForIOS()
    {
      SQLitePCL.Batteries.Init();
      MovieContext.InitConnectionStringForIOS();
    }

    public static async Task<ObservableCollection<Movie>> LoadAsync()
    {
      using (var db = new MovieContext())
      {
#if DEBUG
        //db.Database.EnsureDeleted(); // DB 削除
#endif

        db.Database.EnsureCreated();
        await EnsureInitialDataAsync();

        MoviesList = new ObservableCollection<Movie>(await db.Movies.ToListAsync());
        return MoviesList;

        async Task EnsureInitialDataAsync()
        {
          if (await db.Movies.FirstOrDefaultAsync() != null)
            return;

          // set initial data
          db.Movies.Add(new Movie
          {
            MovieId = 1,
            Title = "UWPアプリを書けばiOS／Android／Webでも動く!?　～Uno Platform：クロスプラットフォーム開発環境",
            Url = "https://codezine.jp/article/detail/11795"
          });
          db.Movies.Add(new Movie
          {
            MovieId = 2,
            Title = "Uno Platform - Home",
            Url = "https://platform.uno/"
          });
          db.Movies.Add(new Movie
          {
            MovieId = 3,
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

    // 仮のデータを作成して、MoviesListに追加する。
    // 仮のデータは、MovieIdが負の値で、TitleとURLは空。DBには追加しない。
    public static Movie CreateTempData()
    {
      int tempIndex = -1;
      int minId = MoviesList.Min(m => m.MovieId);
      if (minId < 0)
        tempIndex = minId - 1;

      var tempData = new Movie { MovieId = tempIndex };
      MoviesList.Add(tempData);

      return tempData;
    }

    // DBへの登録・更新
    // 仮のデータのupdateは、DBへの新規登録となり、MovieIdが採番される。
    public static async Task<Movie> UpdateAsync(Movie originalMovie, Movie newMovie)
    {
      if (!MoviesList.Contains(originalMovie))
        throw new ArgumentException("No data to update on memory");

      if (originalMovie.Title == newMovie.Title && originalMovie.Url == newMovie.Url)
        return originalMovie;

      var updateItem = MoviesList.FirstOrDefault(m => m.MovieId == originalMovie.MovieId);
      int updateItemPosition = MoviesList.IndexOf(updateItem);

      if (originalMovie.MovieId > 0)
      {
        // 既存データの更新
        using (var db = new MovieContext())
        using (var tran = await db.Database.BeginTransactionAsync())
        {
          var targetItem = db.Movies.FirstOrDefault(m => m.MovieId == originalMovie.MovieId);
          if (targetItem == null)
            throw new ArgumentException("No data to update on DB");

          if (originalMovie.Title != targetItem.Title || originalMovie.Url != targetItem.Url)
            throw new ApplicationException("The data has already been changed on DB");

          targetItem.Title = newMovie.Title;
          targetItem.Url = newMovie.Url;
          var modifiedEntry = db.Movies.Update(targetItem);
          int count = await db.SaveChangesAsync(CancellationToken.None);
          if (count < 1)
            throw new ApplicationException("Fail to update on DB");

          MoviesList.RemoveAt(updateItemPosition);
          MoviesList.Insert(updateItemPosition, modifiedEntry.Entity);

          tran.Commit();
        }
      }
      else
      {
        // 新規データ
        using (var db = new MovieContext())
        {
          var newEntry = db.Movies.Add(new Movie
          {
            Title = newMovie.Title,
            Url = newMovie.Url,
          });
          var count = await db.SaveChangesAsync(CancellationToken.None);
          if (count < 1)
            throw new ApplicationException("Fail to insert on DB");

          MoviesList.RemoveAt(updateItemPosition);
          MoviesList.Insert(updateItemPosition, newEntry.Entity);
        }
      }

      return MoviesList.ElementAt(updateItemPosition);
    }

    public static async Task DeleteAsync(Movie movie)
    {
      if (!MoviesList.Contains(movie))
        throw new ArgumentException("No data to delete on memory");

      if (movie.MovieId > 0)
        using (var db = new MovieContext())
        using (var tran = await db.Database.BeginTransactionAsync())
        {
          var targetItem = db.Movies.FirstOrDefault(m => m.MovieId == movie.MovieId);
          if (targetItem == null)
            throw new ArgumentException("No data to delete on DB");

          db.Movies.Remove(targetItem);
          int count = await db.SaveChangesAsync(CancellationToken.None);
          if (count < 1)
            throw new ApplicationException("Fail to delete on DB");

          MoviesList.Remove(movie);
          tran.Commit();
        }
      else
        MoviesList.Remove(movie); // 仮のデータはメモリ上から削除するだけ
    }
   }
}
