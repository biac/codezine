*推奨される Markdown ビューアー: [Markdown Editor VS Extension](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.MarkdownEditor)*

# 生成の概要
次の変更がプロジェクトに組み込まれました: 

生成したファイルは、一時的な生成フォルダーにあります:C:\Users\biac\AppData\Local\Temp\WTSTempGeneration\bdf0edb4-9fcc-4a39-990c-9bc2c48899c6\Sample01HamburgerCodeBehind_20181214_153140

## 新しいファイル:
これらのファイルがプロジェクトに追加されました。
* [Sample01.Hamburger.CodeBehind\Services\OnBackgroundEnteringEventArgs.cs](about:/C:/project-copy/CodeZine/UwpForefront/UF11/Sample01.Hamburger.CodeBehind/Services/OnBackgroundEnteringEventArgs.cs)
* [Sample01.Hamburger.CodeBehind\Services\SuspendAndResumeService.cs](about:/C:/project-copy/CodeZine/UwpForefront/UF11/Sample01.Hamburger.CodeBehind/Services/SuspendAndResumeService.cs)
* [Sample01.Hamburger.CodeBehind\Services\SuspensionState.cs](about:/C:/project-copy/CodeZine/UwpForefront/UF11/Sample01.Hamburger.CodeBehind/Services/SuspensionState.cs)

## 修正したファイル:
次の変更が適用されました: 

### ファイル 'Sample01.Hamburger.CodeBehind\Services\ActivationService.cs' の変更:
最終的な結果を見る:[Sample01.Hamburger.CodeBehind\Services\ActivationService.cs](about:/C:/project-copy/CodeZine/UwpForefront/UF11/Sample01.Hamburger.CodeBehind/Services/ActivationService.cs)

```CSHARP
//***
//This code block includes the SuspendAndResumeService Instance in the method
//`GetActivationHandlers()` in the ActivationService of your project.
//***

using System;
//Block to be included
using Sample01.Hamburger.CodeBehind.Helpers;
//End of block

namespace Sample01.Hamburger.CodeBehind.Services
{
    internal class ActivationService
    {
        private IEnumerable<ActivationHandler> GetActivationHandlers()
        {
            //Block to be included
            yield return Singleton<SuspendAndResumeService>.Instance;
            //End of block
        }
    }
}

```


### ファイル 'Sample01.Hamburger.CodeBehind\App.xaml.cs' の変更:
最終的な結果を見る:[Sample01.Hamburger.CodeBehind\App.xaml.cs](about:/C:/project-copy/CodeZine/UwpForefront/UF11/Sample01.Hamburger.CodeBehind/App.xaml.cs)

```CSHARP
//***
//This code block adds the subscription to `App_EnteredBackground` to the App class of your project.
//***

using System;

//Block to be included
using Windows.ApplicationModel;
//End of block

namespace Sample01.Hamburger.CodeBehind
{
    public sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();

//Block to be included
            EnteredBackground += App_EnteredBackground;
            Resuming += App_Resuming;
//End of block
        }
//Include the following block at the end of the containing block.
//Block to be included

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            await Helpers.Singleton<SuspendAndResumeService>.Instance.SaveStateAsync();
            deferral.Complete();
        }

        private void App_Resuming(object sender, object e)
        {
            Helpers.Singleton<SuspendAndResumeService>.Instance.ResumeApp();
        }
//End of block
    }
}

```



