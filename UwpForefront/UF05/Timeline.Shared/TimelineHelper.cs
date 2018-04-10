using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.UserActivities;
using Windows.UI.Shell;


namespace TimelineLIb
{
  public enum AdaptiveCardType
  {
    None,
    ByCode,
    ByJson,
  }


  public class TimelineHelper 
  {
    //private UserActivityChannel _userActivityChannel;
    //private UserActivity _userActivity;
    private UserActivitySession _userActivitySession;

    //private static TimelineHelper _theInstance;

    private TimelineHelper() {/*(avoid instance)*/}
    //public static TimelineHelper GetInstance()
    //{
    //  if (_theInstance != null)
    //    return _theInstance;

    //  _theInstance = new TimelineHelper
    //  {
    //    _userActivityChannel = UserActivityChannel.GetDefault(),
    //  };
    //  return _theInstance;
    //}
    public static TimelineHelper Current { get; } = new TimelineHelper();
    //{
    //  get
    //  {
    //    if (_theInstance == null)
    //    {
    //      _theInstance = new TimelineHelper
    //      {
    //        //_userActivityChannel = UserActivityChannel.GetDefault(),
    //      };
    //    }
    //    return _theInstance;
    //  }
    //}



    public async Task AddToTimelineAsync(string url, AdaptiveCardType type = AdaptiveCardType.None)
    {
      var userActivityChannel = UserActivityChannel.GetDefault();
      var userActivity
            = await userActivityChannel
                .GetOrCreateUserActivityAsync($"{url}");

      // Create the protocol, so when the clicks the Adaptive Card on the Timeline, it will directly launch to the correct image.
      userActivity.ActivationUri = new Uri($"uf05.bluewatersoft.jp-timelinetest://url?{url}");

      // Set the display text to the User Activity(e.g. Pike Place market.)
      userActivity.VisualElements.DisplayText = $"UF05 {DateTime.Now.ToString("HH:mm:ss")} {url}";

      if (type == AdaptiveCardType.ByJson)
      {
        // Fetch the adative card JSON
        //var adaptiveCard 
        //  = File.ReadAllText($@"{Package.Current.InstalledLocation.Path}\{nameof(TimelineLIb)}\AdaptiveCard.json");
        string adaptiveCardJson;
        //var assembly = typeof(TimelineHelper).GetTypeInfo().Assembly;
        var assembly = this.GetType().GetTypeInfo().Assembly;
        //using (var stream = assembly.GetManifestResourceStream("StandardClassLibrary1.AdaptiveCard.json"))
        using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.AdaptiveCard.json"))
        using (var reader = new StreamReader(stream))
          adaptiveCardJson = reader.ReadToEnd();


        // Replace the content.
        //adaptiveCard = adaptiveCard.Replace("{{backgroundImage}}", "http://bluewatersoft.jp/images/ASP-NET-Banners-01.png");
        //adaptiveCard = adaptiveCard.Replace("{{backgroundImage}}", "http://bluewatersoft.cocolog-nifty.com/blog/IMG_0252d.png");
        //adaptiveCard = adaptiveCard.Replace("{{backgroundImage}}", "https://codezine.jp/static/common/images/no-image.png");
        //adaptiveCard = adaptiveCard.Replace("{{backgroundImage}}", "http://bluewatersoft.jp/images/20160312_katagami_180.png");
        adaptiveCardJson = adaptiveCardJson.Replace("{{time}}", $"{DateTime.Now.ToString("HH:mm:ss")}");
        adaptiveCardJson = adaptiveCardJson.Replace("{{url}}", $"{url}");


        // Assign the Adaptive Card to the user activity. 
        userActivity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson(adaptiveCardJson);
      }
      else if (type == AdaptiveCardType.ByCode)
      {
        // NuGet package "AdaptiveCards" が必要

        var card = new AdaptiveCards.AdaptiveCard();

        card.BackgroundImage = new Uri( "http://bluewatersoft.cocolog-nifty.com/blog/IMG_0252d.png");
        card.Body.Add(
         new AdaptiveCards.AdaptiveTextBlock
         {
           Text = "Timeline",
           Size = AdaptiveCards.AdaptiveTextSize.Large,
           Weight = AdaptiveCards.AdaptiveTextWeight.Bolder,
           Color = AdaptiveCards.AdaptiveTextColor.Light,
         });
        card.Body.Add(
         new AdaptiveCards.AdaptiveTextBlock
         {
           Text = $"at {DateTime.Now.ToString("HH:mm:ss")}",
           Size = AdaptiveCards.AdaptiveTextSize.Small,
           Spacing = AdaptiveCards.AdaptiveSpacing.None,
           Separator = true,
           Color = AdaptiveCards.AdaptiveTextColor.Attention,
           HorizontalAlignment = AdaptiveCards.AdaptiveHorizontalAlignment.Right,
         });
        card.Body.Add(
        new AdaptiveCards.AdaptiveTextBlock
        {
          Text = $"{url}",
          Size = AdaptiveCards.AdaptiveTextSize.Medium,
          Color = AdaptiveCards.AdaptiveTextColor.Light,
          Wrap = true,
          MaxLines = 5,
          Spacing = AdaptiveCards.AdaptiveSpacing.Small,
        });

        string json = card.ToJson();
        // Assign the Adaptive Card to the user activity. 
        userActivity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson(json);
      }

      // Save the details user activity.
      await userActivity.SaveAsync();

      // Dispose of the session and create a new one ready for the next user activity.
      _userActivitySession?.Dispose();

      // ↓ 16299でビルドしてWPFだと、スレ違い例外が出る!!
      _userActivitySession = userActivity.CreateSession();
    }


  }
}
