using UIKit;

namespace UF16.iOS
{
  public class Application
  {
    // This is the main entry point of the application.
    static void Main(string[] args)
    {
      ///
      SQLiteSample.MovieDatabase.InitForIOS();



      // if you want to use a different Application Delegate class from "AppDelegate"
      // you can specify it here.
      UIApplication.Main(args, null, typeof(App));
    }
  }
}