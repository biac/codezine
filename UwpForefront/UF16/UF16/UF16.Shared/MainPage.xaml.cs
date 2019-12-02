using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UF16
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      var articles = await SQLiteSample.ArticleDatabase.LoadAsync();
      ListView1.ItemsSource = articles;
      // ↑作った ObservableCollection をここで ItemsSource にセットしてやらないと、
      //   WebASMでは上手く動作しない

      if (articles.Count > 0)
      {
        await Task.Yield();
        ListView1.SelectedIndex = 0;
      }

#if DEBUG
      await SQLiteSample.ArticleDatabase.AdoDotNetSampleAsync();
#endif
    }

    private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var selectedArticle = e.AddedItems.FirstOrDefault() as SQLiteSample.Article;

      // update EditPanel
      EditPanel.Visibility = (selectedArticle == null) ? Visibility.Collapsed : Visibility.Visible;
      IdText.Text = (selectedArticle?.ArticleId > 0) ? selectedArticle.ArticleId.ToString() : string.Empty;
      TitleText.Text = selectedArticle?.Title ?? string.Empty;
      UrlText.Text = selectedArticle?.Url ?? string.Empty;

      // update WebView
      WebView1.Source = new Uri("about:blank");
      await Task.Delay(100);
      if (selectedArticle?.Url?.ToLower().StartsWith("http") == true)
        WebView1.Source = new Uri(selectedArticle.Url);
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
      var tempData = SQLiteSample.ArticleDatabase.CreateTempData();

      await Task.Yield();
      ListView1.SelectedItem = tempData;
      ListView1.ScrollIntoView(tempData);
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
      var deleteArticle = ListView1.SelectedItem as SQLiteSample.Article;
      if (deleteArticle == null)
        return;

      if (!await ConfirmAsync())
        return;

      await SQLiteSample.ArticleDatabase.DeleteAsync(deleteArticle);
      return;

      async Task<bool> ConfirmAsync()
      {
        if (deleteArticle.ArticleId < 1)
          return true;

        var result = await (new ContentDialog()
        {
          Title = "Delete Item",
          Content = "Are you sure you want to delete this item from DB?",
          PrimaryButtonText = "Delete",
          SecondaryButtonText = "Cancel",
        }).ShowAsync();
        return (result == ContentDialogResult.Primary);
      }
    }

    private async void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
      var originalArticle = ListView1.SelectedItem as SQLiteSample.Article;
      if (originalArticle == null)
        return;

      var newArticle = new SQLiteSample.Article(originalArticle.ArticleId)
      {
        Title = TitleText.Text?.Trim(),
        Url = UrlText.Text?.Trim(),
      };
      if (!await ValidateAndShowMessageWhenInvalidAsync())
        return;

      var updatedArticle
        = await SQLiteSample.ArticleDatabase.UpdateAsync(originalArticle, newArticle);

      await Task.Yield();
      ListView1.SelectedItem = updatedArticle;

      return;

      async Task<bool> ValidateAndShowMessageWhenInvalidAsync()
      {
        if (string.IsNullOrEmpty(newArticle.Title) || string.IsNullOrEmpty(newArticle.Url))
        {
          await (new ContentDialog()
          {
            Title = "Can't update",
            Content = "Both Title and URL must be entered.",
            PrimaryButtonText = "OK",
          }).ShowAsync();
          return false;
        }

        if (!Uri.IsWellFormedUriString(newArticle.Url, UriKind.Absolute))
        {
          await (new ContentDialog()
          {
            Title = "Can't update",
            Content = "Invalid URL. Enter the correct URL.",
            PrimaryButtonText = "OK",
          }).ShowAsync();
          return false;
        }

        return true;
      }
    }
  }
}
