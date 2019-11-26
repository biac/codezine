using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UF16
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    //private ObservableCollection<SQLiteSample.Movie> _movies;
    //= new ObservableCollection<SQLiteSample.Movie>();


    public MainPage()
    {
      this.InitializeComponent();
    }


    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      //await SQLiteSample.MovieDatabase.Run();
      //  _movies.Add(
      //new SQLiteSample.Movie { MovieId = 1, Title = "AAA", Url = "http://bluewatersoft.jp" });
      //  _movies.Add(
      //      new SQLiteSample.Movie { MovieId = 2, Title = "bbb", Url = "http://bluewatersoft.jp" });


      var movies = await SQLiteSample.MovieDatabase.LoadAsync();
      //_movies = new ObservableCollection<SQLiteSample.Movie>(movies);
      ListView1.ItemsSource = movies;
      // ↑作った ObservableCollection をここで ItemsSource にセットしてやらないと、
      //   WebASMでは上手く動作しなかった

      if (movies.Count > 0)
      {
        await Task.Yield();
        ListView1.SelectedIndex = 0;
      }

      //WebView1.Source = new Uri("http://www.bluewatersoft.jp");

    }


    private void DataGrid_Loaded(object sender, RoutedEventArgs e)
    {
      //DataGrid1.ItemsSource = _movies;


      //DataGrid1.UpdateLayout();
    }
    private void ListView_Loaded(object sender, RoutedEventArgs e)
    {
    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
    }

    private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var movie = e.AddedItems.FirstOrDefault() as SQLiteSample.Movie;
      IdText.Text = (movie?.MovieId > 0) ? movie.MovieId.ToString() : string.Empty;
      TitleText.Text = movie?.Title ?? string.Empty;
      UrlText.Text = movie?.Url ?? string.Empty;
      EditPanel.Visibility = (movie == null) ? Visibility.Collapsed : Visibility.Visible;

      WebView1.Source = new Uri("about:blank");
      await Task.Delay(100);
      if (movie?.Url?.StartsWith("http") == true)
        WebView1.Source = new Uri(movie.Url);
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
      //DataGrid1.ItemsSource = _movies;

      int tempIndex = -1;
      //if (ListView1.Items?.FirstOrDefault() != null)
      //{
      //  int minId = ListView1.Items.Min(o => (o as SQLiteSample.Movie).MovieId);
      //  if (minId < 0)
      //    tempIndex = minId - 1;
      //}
      int minId = SQLiteSample.MovieDatabase.MoviesList.Min(m => m.MovieId);
      if (minId < 0)
        tempIndex = minId - 1;

      var newData = new SQLiteSample.Movie { MovieId = tempIndex };
      SQLiteSample.MovieDatabase.MoviesList.Add(newData);

      await Task.Yield();
      //DataGrid1.ScrollIntoView(newData, IdColum);
      //DataGrid1.SelectedItem = newData;
      ListView1.SelectedItem = newData;
      ListView1.ScrollIntoView(newData);
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
      if (ListView1.SelectedItem == null)
        return;

      var deleteMovie = ListView1.SelectedItem as SQLiteSample.Movie;
      if (deleteMovie.MovieId > 0)
      {
        var result = await (new ContentDialog()
        {
          Title = "Delete Item",
          Content = "Are you sure you want to delete this item from DB?",
          PrimaryButtonText = "Delete",
          SecondaryButtonText = "Cancel",
        }).ShowAsync();
        if (result != ContentDialogResult.Primary)
          return;

      }
      await SQLiteSample.MovieDatabase.DeleteAsync(deleteMovie);
      //_movies.Remove(deleteMovie);


    }

    private async void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
      if (ListView1.SelectedItem == null)
        return;

      var newMovie = new SQLiteSample.Movie {
        MovieId = (ListView1.SelectedItem as SQLiteSample.Movie).MovieId, 
        Title=TitleText.Text?.Trim(),
        Url=UrlText.Text?.Trim(),
      };
      if(string.IsNullOrEmpty(newMovie.Title) || string.IsNullOrEmpty(newMovie.Url))
      {
        await (new ContentDialog()
        {
          Title = "Can't update",
          Content = "Both Title and URL must be entered.",
          PrimaryButtonText = "OK",
        }).ShowAsync();
        return;
      }
      if (!Uri.IsWellFormedUriString(newMovie.Url, UriKind.Absolute))
      {
        await (new ContentDialog()
        {
          Title = "Can't update",
          Content = "Invalid URL. Enter the correct URL.",
          PrimaryButtonText = "OK",
        }).ShowAsync();
        return;
      }

      var originalMovie = ListView1.SelectedItem as SQLiteSample.Movie;
      var updatedMovie = await SQLiteSample.MovieDatabase.UpdateAsync(originalMovie, newMovie);

      await Task.Yield();
      ListView1.SelectedItem = updatedMovie;
    }
  }
}
