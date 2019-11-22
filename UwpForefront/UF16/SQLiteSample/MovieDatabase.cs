using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
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

    public static async Task Run()
    {
      using (var db = new MovieContext())
      {
        db.Database.EnsureCreated();

        Console.WriteLine("Database created");

        db.Movies.Add(new Movie { Title = "TEST1", Url = "http://nowhere.com/?2" });
        var count = await db.SaveChangesAsync(CancellationToken.None);

        Console.WriteLine("{0} records saved to database", count);

        Console.WriteLine();
        Console.WriteLine("All blogs in database:");
        foreach (var movie in db.Movies)
        {
          //Console.WriteLine(" - {0}", moview.Url);
          Console.WriteLine($" [{movie.MovieId}] {movie.Title} {movie.Url}");
        }
      }

    }
  }




  public class Movie
  {
    public int MovieId { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
  }

  public class MovieContext : DbContext
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
