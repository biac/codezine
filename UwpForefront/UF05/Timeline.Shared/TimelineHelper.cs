using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserActivities;
using Windows.UI;
using Windows.UI.Shell;

namespace TimelineLIb
{
  [System.Runtime.Serialization.DataContract]
  public enum AdaptiveCardType
  {
    None,
    ByCode,
    ByJson,
  }

  public class TimelineHelper
  {
    private UserActivitySession _userActivitySession;

    private TimelineHelper() {/* (avoid instance) */}
    public static TimelineHelper Current { get; } = new TimelineHelper();

    public async Task AddToTimelineAsync(string url, AdaptiveCardType type = AdaptiveCardType.None)
    {
      string activityId = url;  //ここでは URL を Activity ID とする

      // 必須: UserActivity を生成する (登録済みなら取得する)
      var userActivityChannel = UserActivityChannel.GetDefault();
      UserActivity userActivity
            = await userActivityChannel.GetOrCreateUserActivityAsync(activityId);

      // タイムラインの表示を最新のものだけにするなら、既存のものを削除する
      //await UserActivityChannel.GetDefault().DeleteActivityAsync(activityId);

      // 必須: タイムラインから呼び出されるときのパラメーターをセット
      // (UserActivityState.Published=登録済みのアクティビティではセット済み。変更の必要なし)
      // ※ Package.appxmanifest での宣言が必要
      if (userActivity.State != UserActivityState.Published)
        userActivity.ActivationUri = new Uri($"uf05.bluewatersoft.jp-timelinetest://url?{url}");

      // 必須: AdaptiveCard 未使用時に表示される文字列をセット (タイムライン上での検索対象)
      userActivity.VisualElements.DisplayText = $"UF05 {DateTime.Now.ToString("HH:mm:ss")}";

      // オプション: AdaptiveCard 未使用時に表示される文字列(詳細)をセット (タイムライン上での検索対象)
      if (userActivity.State != UserActivityState.Published)
        userActivity.VisualElements.Description = url;

      // オプション: AdaptiveCard を添付、または、背景色を設定
      switch (type)
      {
        case AdaptiveCardType.ByJson:
          userActivity.VisualElements.Content = CreateAdaptiveCardFromJson(url);
          break;
        case AdaptiveCardType.ByCode:
          userActivity.VisualElements.Content = CreateAdaptiveCardByCode(url);
          break;
        default:
          userActivity.VisualElements.Content = null;
          userActivity.VisualElements.BackgroundColor = Color.FromArgb(0xFF, 0x40, 0x5D, 0x47);
          break;
      }

      // オプション: AdaptiveCard 左上のアイコンと文字列を変更する
      if (userActivity.State != UserActivityState.Published)
        userActivity.VisualElements.Attribution = new UserActivityAttribution()
        {
          IconUri = new Uri((new Uri(url)), "/favicon.ico"),
          AlternateText = "Timeline TEST",
        };

      // 必須: UserActivity を保存
      await userActivity.SaveAsync();

      // 必須: 以前の UserActivitySession があるなら破棄して、新しいセッションを保持
      _userActivitySession?.Dispose();
      _userActivitySession = userActivity.CreateSession();
    }




    // JSON で記述したカードのテンプレートから AdaptiveCard を作る
    private IAdaptiveCard CreateAdaptiveCardFromJson(string url)
    {
      // JSON データを読み込む
      string adaptiveCardJson;
      var assembly = this.GetType().GetTypeInfo().Assembly;
      using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.AdaptiveCard.json"))
      using (var reader = new StreamReader(stream))
        adaptiveCardJson = reader.ReadToEnd();

      // プレースホルダーを置き換え
      adaptiveCardJson = adaptiveCardJson.Replace("{{time}}", $"{DateTime.Now.ToString("HH:mm:ss")}");
      adaptiveCardJson = adaptiveCardJson.Replace("{{url}}", url);

      // AdaptiveCard を生成して返す
      return AdaptiveCardBuilder.CreateAdaptiveCardFromJson(adaptiveCardJson);
    }

    // コードで AdaptiveCard を組み立てる
    private static IAdaptiveCard CreateAdaptiveCardByCode(string url)
    {
      // NuGet package "AdaptiveCards" が必要

      var card = new AdaptiveCards.AdaptiveCard();

      // 背景画像
      card.BackgroundImage = new Uri("http://bluewatersoft.cocolog-nifty.com/blog/IMG_0252d.png");

      // 一段目
      card.Body.Add(
        new AdaptiveCards.AdaptiveTextBlock
        {
          Text = "Timeline",
          Size = AdaptiveCards.AdaptiveTextSize.Large,
          Weight = AdaptiveCards.AdaptiveTextWeight.Bolder,
          Color = AdaptiveCards.AdaptiveTextColor.Light,
        });

      // 二段目
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

      // 三段目 (何段でも追加可能。ただし、どこまで表示されるかは分からない)
      card.Body.Add(
        new AdaptiveCards.AdaptiveTextBlock
        {
          Text = url,
          Size = AdaptiveCards.AdaptiveTextSize.Medium,
          Color = AdaptiveCards.AdaptiveTextColor.Light,
          Wrap = true,
          MaxLines = 5,
          Spacing = AdaptiveCards.AdaptiveSpacing.Small,
        });

      return AdaptiveCardBuilder.CreateAdaptiveCardFromJson(card.ToJson());
    }


  }
}
