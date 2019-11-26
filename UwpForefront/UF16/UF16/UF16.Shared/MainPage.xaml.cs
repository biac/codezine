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

      var movies = await SQLiteSample.MovieDatabase.LoadAsync();
      ListView1.ItemsSource = movies;
      // ↑作った ObservableCollection をここで ItemsSource にセットしてやらないと、
      //   WebASMでは上手く動作しない

      if (movies.Count > 0)
      {
        await Task.Yield();
        ListView1.SelectedIndex = 0;
      }
    }

    private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var selectedMovie = e.AddedItems.FirstOrDefault() as SQLiteSample.Movie;

      // update EditPanel
      EditPanel.Visibility = (selectedMovie == null) ? Visibility.Collapsed : Visibility.Visible;
      IdText.Text = (selectedMovie?.MovieId > 0) ? selectedMovie.MovieId.ToString() : string.Empty;
      TitleText.Text = selectedMovie?.Title ?? string.Empty;
      UrlText.Text = selectedMovie?.Url ?? string.Empty;

      // update WebView
      WebView1.Source = new Uri("about:blank");
      await Task.Delay(100);
      if (selectedMovie?.Url?.ToLower().StartsWith("http") == true)
        WebView1.Source = new Uri(selectedMovie.Url);
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
      var tempData = SQLiteSample.MovieDatabase.CreateTempData();

      await Task.Yield();
      ListView1.SelectedItem = tempData;
      ListView1.ScrollIntoView(tempData);
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
      var deleteMovie = ListView1.SelectedItem as SQLiteSample.Movie;
      if (deleteMovie == null)
        return;

      if (!await ConfirmAsync())
        return;

      await SQLiteSample.MovieDatabase.DeleteAsync(deleteMovie);
      return;

      async Task<bool> ConfirmAsync()
      {
        if (deleteMovie.MovieId < 1)
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
      var originalMovie = ListView1.SelectedItem as SQLiteSample.Movie;
      if (originalMovie == null)
        return;

      var newMovie = new SQLiteSample.Movie
      {
        MovieId = originalMovie.MovieId,
        Title = TitleText.Text?.Trim(),
        Url = UrlText.Text?.Trim(),
      };
      if (!await ValidateAndShowMessageWhenInvalidAsync())
        return;

      var updatedMovie = await SQLiteSample.MovieDatabase.UpdateAsync(originalMovie, newMovie);

      await Task.Yield();
      ListView1.SelectedItem = updatedMovie;

      return;

      async Task<bool> ValidateAndShowMessageWhenInvalidAsync()
      {
        if (string.IsNullOrEmpty(newMovie.Title) || string.IsNullOrEmpty(newMovie.Url))
        {
          await (new ContentDialog()
          {
            Title = "Can't update",
            Content = "Both Title and URL must be entered.",
            PrimaryButtonText = "OK",
          }).ShowAsync();
          return false;
        }

        if (!Uri.IsWellFormedUriString(newMovie.Url, UriKind.Absolute))
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
