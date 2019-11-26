using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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


    //internal static string DbPath { get; private set; }
    public static void InitForAndroid()
    {
      //SQLitePCL.Batteries.Init();

      string dbPath = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.Personal),
          "uf16.db");
      MovieContext.ConnectionString = $"filename={dbPath}";
      //if (!File.Exists(dbPath)) File.Create(dbPath);

      //Connection = new SqliteConnection(dbPath);
    }


    public static void InitForIOS()
    {
      SQLitePCL.Batteries.Init();

      string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", "uf16.db");
      MovieContext.ConnectionString = $"filename={dbPath}";
    }


    //public static string qq()
    //{
    //  //return "skip";

    //  using (var connection = new SqliteConnection("Data Source=local.db"))
    //  {
    //    connection.Open();

    //    var command = connection.CreateCommand();
    //    command.CommandText = "SELECT Url FROM Blogs";

    //    using (var reader = command.ExecuteReader())
    //    {
    //      while (reader.Read())
    //      {
    //        var url = reader.GetString(0);
    //      }
    //    }
    //    return "OK";
    //  }
    //}

    //public static async Task Run()
    //{
    //  using (var db = new MovieContext())
    //  {
    //    db.Database.EnsureCreated();

    //    Console.WriteLine("Database created");

    //    db.Movies.Add(new Movie { Title = "TEST1", Url = "http://nowhere.com/?2" });
    //    var count = await db.SaveChangesAsync(CancellationToken.None);

    //    Console.WriteLine("{0} records saved to database", count);

    //    Console.WriteLine();
    //    Console.WriteLine("All blogs in database:");
    //    foreach (var movie in db.Movies)
    //    {
    //      //Console.WriteLine(" - {0}", moview.Url);
    //      Console.WriteLine($" [{movie.MovieId}] {movie.Title} {movie.Url}");
    //    }
    //  }
    //}

    public static async Task<IReadOnlyList<Movie>> LoadAsync()
    {
      using (var db = new MovieContext())
      {
#if DEBUG
        //// DB 削除
        //db.Database.EnsureDeleted();
#endif

        db.Database.EnsureCreated();



        if (await db.Movies.FirstOrDefaultAsync() == null)
        {
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

        MoviesList = new ObservableCollection<Movie>(await db.Movies.ToListAsync());
        return MoviesList;
      }
    }

    public static async Task DeleteAsync(Movie movie)
    {
      if (!MoviesList.Contains(movie))
        throw new ArgumentException("No data to delete on memory");

      if(movie.MovieId > 0)
        using (var db = new MovieContext())
        using (var tran = await db.Database.BeginTransactionAsync())
        {
          var targetItem = db.Movies.FirstOrDefault(m => m.MovieId == movie.MovieId);
          if (targetItem == null)
            throw new ArgumentException("No data to delete on DB");

          db.Movies.Remove(targetItem);
          int count = await db.SaveChangesAsync(CancellationToken.None);
          if(count < 1)
            throw new ApplicationException("Fail to delete on DB");

          MoviesList.Remove(movie);
          tran.Commit();
        }
    }

    public static async Task<Movie> UpdateAsync(Movie originalMovie, Movie newMovie)
    {
      if (!MoviesList.Contains(originalMovie))
        throw new ArgumentException("No data to update on memory");

      if (originalMovie.Title == newMovie.Title && originalMovie.Url == newMovie.Url)
        return originalMovie;

      var updatedItem = MoviesList.FirstOrDefault(m => m.MovieId == originalMovie.MovieId);
      int updatedItemPosition = MoviesList.IndexOf(updatedItem);

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

          MoviesList.RemoveAt(updatedItemPosition);
          MoviesList.Insert(updatedItemPosition, modifiedEntry.Entity);

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

          MoviesList.RemoveAt(updatedItemPosition);
          MoviesList.Insert(updatedItemPosition, newEntry.Entity);
        }
      }

      return MoviesList.ElementAt(updatedItemPosition);
    }
  }




  public class Movie
  {
    public int MovieId { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
  }

  internal class MovieContext : DbContext
  {
    public static string ConnectionString { get; set; } = "data source=uf16.db";

    public DbSet<Movie> Movies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      //if(Class1.DbPath != null)
      //  optionsBuilder.UseSqlite($"filename={Class1.DbPath}");
      //else
      //  optionsBuilder.UseSqlite("data source=local.db");
      optionsBuilder.UseSqlite(ConnectionString);
    }
  }
}
