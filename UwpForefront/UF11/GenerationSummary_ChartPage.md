*推奨される Markdown ビューアー: [Markdown Editor VS Extension](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.MarkdownEditor)*

# 生成の概要
次の変更がプロジェクトに組み込まれました: 

生成したファイルは、一時的な生成フォルダーにあります:C:\Users\biac\AppData\Local\Temp\WTSTempGeneration\bdf0edb4-9fcc-4a39-990c-9bc2c48899c6\Sample01HamburgerCodeBehind_20181214_144628

## 新しいファイル:
これらのファイルがプロジェクトに追加されました。
* [Sample01.Hamburger.CodeBehind\Models\DataPoint.cs](about:/C:/project-copy/CodeZine/UwpForefront/UF11/Sample01.Hamburger.CodeBehind/Models/DataPoint.cs)
* [Sample01.Hamburger.CodeBehind\Views\ChartPage.xaml](about:/C:/project-copy/CodeZine/UwpForefront/UF11/Sample01.Hamburger.CodeBehind/Views/ChartPage.xaml)
* [Sample01.Hamburger.CodeBehind\Views\ChartPage.xaml.cs](about:/C:/project-copy/CodeZine/UwpForefront/UF11/Sample01.Hamburger.CodeBehind/Views/ChartPage.xaml.cs)

## 修正したファイル:
次の変更が適用されました: 

### ファイル 'Sample01.Hamburger.CodeBehind\Sample01.Hamburger.CodeBehind.csproj' の変更:
最終的な結果を見る:[Sample01.Hamburger.CodeBehind\Sample01.Hamburger.CodeBehind.csproj](about:/C:/project-copy/CodeZine/UwpForefront/UF11/Sample01.Hamburger.CodeBehind/Sample01.Hamburger.CodeBehind.csproj)

```XML
<!--***-->
<!--This xml block adds a reference to Telerik.UI.for.UniversalWindowsPlatform to your project.-->
<!--***-->

<!-- Nuget package references -->
<!--Block to be included-->
<PackageReference Include="Telerik.UI.for.UniversalWindowsPlatform">
  <Version>1.0.1.2</Version>
</PackageReference>
<!--End of block-->

```


### ファイル 'Sample01.Hamburger.CodeBehind\Views\ShellPage.xaml' の変更:
最終的な結果を見る:[Sample01.Hamburger.CodeBehind\Views\ShellPage.xaml](about:/C:/project-copy/CodeZine/UwpForefront/UF11/Sample01.Hamburger.CodeBehind/Views/ShellPage.xaml)

```XML
<!--***-->
<!--This code adds the new page to the NavigationView control in ShellPage -->
<!--***-->

<Page
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d">

    <winui:NavigationView
        Background="{ThemeResource SystemControlBackgroundAltHighBrush}">
        <winui:NavigationView.MenuItems>
            <!--Include the following block at the end of the containing block.-->
            <!--Block to be included-->
            <winui:NavigationViewItem x:Uid="Shell_Chart" Icon="Document" helpers:NavHelper.NavigateTo="views:ChartPage" />
            <!--End of block-->
        </winui:NavigationView.MenuItems>        
    </winui:NavigationView>
</Page>

```


### ファイル 'Sample01.Hamburger.CodeBehind\Services\SampleDataService.cs' の変更:
最終的な結果を見る:[Sample01.Hamburger.CodeBehind\Services\SampleDataService.cs](about:/C:/project-copy/CodeZine/UwpForefront/UF11/Sample01.Hamburger.CodeBehind/Services/SampleDataService.cs)

```CSHARP
//***
// This code block adds the method `GetChartSampleData()` to the SampleDataService of your project.
//***
//Block to be included
using System.Collections.ObjectModel;
using System.Linq;
//End of block

namespace Sample01.Hamburger.CodeBehind.Services
{
    public static class SampleDataService
    {
//Include the following block at the end of the containing block.
//Block to be included

        // TODO WTS: Remove this once your chart page is displaying real data
        public static ObservableCollection<DataPoint> GetChartSampleData()
        {
            var data = AllOrders().Select(o => new DataPoint() { Category = o.Company, Value = o.OrderTotal })
                                  .OrderBy(dp => dp.Category);

            return new ObservableCollection<DataPoint>(data);
        }
//End of block
    }
}

```
```CSHARP
//***
// This code block adds the method `GetChartSampleData()` to the SampleDataService of your project.
//***
//Block to be included
using System.Collections.ObjectModel;
using System.Linq;
//End of block

namespace Sample01.Hamburger.CodeBehind.Services
{
    public static class SampleDataService
    {
//Include the following block at the end of the containing block.
//Block to be included

        // TODO WTS: Remove this once your chart page is displaying real data
        public static ObservableCollection<DataPoint> GetChartSampleData()
        {
            var data = AllOrders().Select(o => new DataPoint() { Category = o.Company, Value = o.OrderTotal })
                                  .OrderBy(dp => dp.Category);

            return new ObservableCollection<DataPoint>(data);
        }
//End of block
    }
}

```
```CSHARP
//***
// This code block adds the method `GetChartSampleData()` to the SampleDataService of your project.
//***
//Block to be included
using System.Collections.ObjectModel;
using System.Linq;
//End of block

namespace Sample01.Hamburger.CodeBehind.Services
{
    public static class SampleDataService
    {
//Include the following block at the end of the containing block.
//Block to be included

        // TODO WTS: Remove this once your chart page is displaying real data
        public static ObservableCollection<DataPoint> GetChartSampleData()
        {
            var data = AllOrders().Select(o => new DataPoint() { Category = o.Company, Value = o.OrderTotal })
                                  .OrderBy(dp => dp.Category);

            return new ObservableCollection<DataPoint>(data);
        }
//End of block
    }
}

```


### ファイル 'Sample01.Hamburger.CodeBehind\Strings\en-us\Resources.resw' の変更:
最終的な結果を見る:[Sample01.Hamburger.CodeBehind\Strings\en-us\Resources.resw](about:/C:/project-copy/CodeZine/UwpForefront/UF11/Sample01.Hamburger.CodeBehind/Strings/en-us/Resources.resw)

```
<!--***-->
<!--This xml block adds string resources from the ChartPage in the NavigationPane to your project.-->
<!--***-->

<root>
  <!--Include the following block at the end of the containing block.-->
  <!--Block to be included-->
  <data name="Shell_Chart.Content" xml:space="preserve">
    <value>Chart</value>
    <comment>Navigation view item name for Chart</comment>
  </data>
  <!--End of block-->
</root>

```



