CodoZine 連載 [UWPアプリ開発の最前線](https://codezine.jp/article/corner/731)
# [第14回](https://codezine.jp/article/detail/11706): **Windows 電卓の中はどうなっているのだろう? (後編)** ～オープンソースのコードを改造してみよう (2019/09/xx)

このサンプルコードは、 OSS として公開されている [Windows 電卓アプリのコード](https://github.com/microsoft/calculator)に以下の 2 つの機能を追加したものです。 いずれの機能も、 Windows の地域設定が日本のときだけ有効になります。

* 面積コンバーターに「坪」を追加
* 日付計算に西暦⇔和暦の切り替えスイッチを追加

**※制限事項**  
カレンダーのドロップダウンを表示すると、 西暦⇔和暦の切り替えスイッチは無効になります。  
ドロップダウン表示後に切り替えると、 次にドロップダウンを表示しようとしたときに CalendarDatePicker がメモリーリークを起こすという不具合があるためです。 この不具合は、[CalendarDatePickerTest](./calculator/src/CalendarDatePickerTest/) プロジェクト (C#) で確認できます (Win10 1809, 1903 で確認済み)。

![左:「坪」の換算、右:日付の和暦表示](https://codezine.jp/static/images/article/11706/fig01.png)

## 面積コンバーターに「坪」を追加

記事で詳しく解説しています。 [UWPアプリ開発の最前線 第14回](https://codezine.jp/article/detail/11706)をご覧ください。

## 日付計算に西暦⇔和暦の切り替えスイッチを追加

こちらは記事で解説していませんので、 詳細はこのサンプルコードを読んでください。 上の「坪」と合わせて、 オリジナルソースからの変更箇所に「bw:」で始まるコメントを付けてあります。

以下、 変更内容を簡単に紹介しておきます。

### **Model**

(変更なし)

### **ViewModel**

* DateCalculatorViewModel.h  
  バインド可能なプロパティを追加
  * IsJapan: 日本専用機能の ON / OFF
  * UseJapaneseCalendar: 切替スイッチの ON / OFF
  * CalendarIdentifier: カレンダーの表示形式
* DateCalculatorViewModel.cpp
  * コンストラクターを修正: 追加したプロパティの初期化
  * OnPropertyChanged メソッドを修正: 切替スイッチの ON / OFF に応じて CalendarIdentifier を切り替え

### **View**

本来なら ToggleSwitch コントロールを追加し、 既存の CalendarDatePicker コントロールの CalendarIdentifier プロパティを ViewModel にバインドする程度で済むはずでした。 実際には、 CalendarDatePicker コントロールに不具合がいくつかあって、 その対策のためのコードがずいぶんと増えてしまいました。

* DateCalculator.xaml
  * ToggleSwitch コントロールを追加: 西暦⇔和暦の切り替えスイッチ (名前: UseJapaneseCalendarSwitch)
  * DateDiffGrid に Loaded イベントハンドラーを追加
* DateCalculator.xaml.cpp
  * AddSubtractDateGrid_Loaded メソッドを修正: ロード時の西暦⇔和暦表示切り替え、 および、 CalendarDatePicker の不具合対策を追加
  * DateDiffGrid_Loaded メソッドを追加: CalendarDatePicker の不具合対策
  * OnPropertyChangedEventHandler メソッドを追加: ViewModel の CalendarIdentifier が変化したときに、 次に示す SetCalendarIdentifiers メソッドを呼び出す (不具合対策)
  * SetCalendarIdentifiers メソッドを追加: CalendarDatePicker コントロールの CalendarIdentifier を変更 (不具合対策)
  * CalendarFlyoutClosed メソッドを修正: CalendarDatePicker の不具合対策を追加 (冒頭に記した制限事項)
  


