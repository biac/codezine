namespace SQLiteSample
{
  public class Article
  {
    public int ArticleId { get; private set; }  // 主キー（変更不可）
    public string Title { get; set; }
    public string Url { get; set; }

    public Article() { }
    public Article(int articleId) => ArticleId = articleId;
  }
}
