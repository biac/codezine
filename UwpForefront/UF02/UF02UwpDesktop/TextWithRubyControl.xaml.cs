using System.Windows;
using System.Windows.Controls;

namespace UF02UwpDesktop
{
  /// <summary>
  /// TextWithRubyControl.xaml の相互作用ロジック
  /// </summary>
  public partial class TextWithRubyControl : UserControl
  {
    // Bodyプロパティ（文字列本体） 
    public string Body
    {
      get { return GetValue(BodyProperty) as string; }
      set { SetValue(BodyProperty, value); }
    }
    public static readonly DependencyProperty BodyProperty
      = DependencyProperty.Register(
          "Body", typeof(string), typeof(TextWithRubyControl), null
        );

    // Rubyプロパティ（フリガナ） 
    public string Ruby
    {
      get { return GetValue(RubyProperty) as string; }
      set { SetValue(RubyProperty, value); }
    }
    public static readonly DependencyProperty RubyProperty
      = DependencyProperty.Register(
          "Ruby", typeof(string), typeof(TextWithRubyControl), null
        );



    public TextWithRubyControl()
    {
      InitializeComponent();

      DataContext = this;
    }
  }
}
