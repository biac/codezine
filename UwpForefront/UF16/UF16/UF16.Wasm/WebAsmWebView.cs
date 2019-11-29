using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace UF16
{
  [ContentProperty(Name = nameof(Source))]
  public class WebAsmWebView : Control
  {
    public WebAsmWebView()
      : base(htmlTag: "iframe")
    {
      base.SetAttribute("style", "");
    }
    // ※ style 属性には既定で「pointer-events: none;」が設定される。
    //    そのため、そのままではマウスでクリックやスクロールができない。
    //    そこで、style 属性を空にしてやる。
    //    ・既定のまま実行したときの、ブラウザ上での style 属性の例
    //        style="pointer-events: none; position: absolute; top: 80px; left: 0px; width: 393px; height: 761px;"
    //    ・ここで style 属性を空にしたときの、ブラウザ上での style 属性の例
    //        style="position: absolute; top: 80px; left: 0px; width: 396px; height: 761px;"

    private Uri _source;
    public Uri Source
    {
      get => _source;
      set {
        base.SetAttribute("src", value.ToString());
        _source = value;
      }
    }
  }
}
