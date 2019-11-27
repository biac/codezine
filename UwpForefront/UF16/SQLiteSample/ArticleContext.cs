using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace SQLiteSample
{
  internal class ArticleContext : DbContext
  {
    public DbSet<Article> Articles { get; set; }

    // 接続文字列（UWPとWebASM）
    const string DbName = "uf16.db";
    private static string _connectionString = $"data source={DbName}";

    // 接続文字列（Android）
    public static void InitConnectionStringForAndroid()
    {
      string dbPath = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.Personal),
          DbName);
      _connectionString = $"filename={dbPath}";
    }

    // 接続文字列（iOS）
    public static void InitConnectionStringForIOS()
    {
      string dbPath = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
          "..", "Library", DbName);
      _connectionString = $"filename={dbPath}";
    }

    // DbContextの設定（SQLiteを使用）
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite(_connectionString);
    }
  }
}
