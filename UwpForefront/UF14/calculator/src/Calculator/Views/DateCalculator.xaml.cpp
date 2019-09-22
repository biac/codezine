// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

//
// DateCalculator.xaml.cpp
// Implementation of the DateCalculator class
//

#include "pch.h"
#include "DateCalculator.xaml.h"
#include "CalcViewModel/Common/TraceLogger.h"
#include "CalcViewModel/Common/CopyPasteManager.h"
#include "CalcViewModel/Common/LocalizationSettings.h"
#include "CalcViewModel/DateCalculatorViewModel.h"

using namespace CalculatorApp;
using namespace CalculatorApp::Common;
using namespace CalculatorApp::ViewModel;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;
using namespace Windows::System::UserProfile;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Automation;
using namespace Windows::UI::Xaml::Automation::Peers;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

DateCalculator::DateCalculator()
{
    InitializeComponent();

    const auto& localizationSettings = LocalizationSettings::GetInstance();

    // Set Calendar Identifier
    DateDiff_FromDate->CalendarIdentifier = localizationSettings.GetCalendarIdentifier();
    DateDiff_ToDate->CalendarIdentifier = localizationSettings.GetCalendarIdentifier();

    // Setting the FirstDayofWeek
    DateDiff_FromDate->FirstDayOfWeek = localizationSettings.GetFirstDayOfWeek();
    DateDiff_ToDate->FirstDayOfWeek = localizationSettings.GetFirstDayOfWeek();

    // Setting the Language explicitly is not required,
    // this is a workaround for the bug in the control due to which
    // the displayed date is incorrect for non Gregorian Calendar Systems
    // The displayed date doesn't honor the shortdate format, on setting the Language the format is refreshed
    DateDiff_FromDate->Language = localizationSettings.GetLocaleName();
    DateDiff_ToDate->Language = localizationSettings.GetLocaleName();

    // Set Min and Max Dates according to the Gregorian Calendar(1601 & 9999)
    auto calendar = ref new Calendar();
    auto today = calendar->GetDateTime();

    calendar->ChangeCalendarSystem(CalendarIdentifiers::Gregorian);
    calendar->Day = 1;
    calendar->Month = 1;
    calendar->Year = c_minYear;
    auto minYear = calendar->GetDateTime(); // 1st January, 1601
    DateDiff_FromDate->MinDate = minYear;
    DateDiff_ToDate->MinDate = minYear;

    calendar->Day = 31;
    calendar->Month = 12;
    calendar->Year = c_maxYear;
    auto maxYear = calendar->GetDateTime(); // 31st December, 9878
    DateDiff_FromDate->MaxDate = maxYear;
    DateDiff_ToDate->MaxDate = maxYear;

    // Set the PlaceHolderText for CalendarDatePicker
    DateTimeFormatter ^ dateTimeFormatter = LocalizationService::GetInstance()->GetRegionalSettingsAwareDateTimeFormatter(
        L"day month year",
        localizationSettings.GetCalendarIdentifier(),
        ClockIdentifiers::TwentyFourHour); // Clock Identifier is not used

    DateDiff_FromDate->DateFormat = L"day month year";
    DateDiff_ToDate->DateFormat = L"day month year";

    auto placeholderText = dateTimeFormatter->Format(today);

    DateDiff_FromDate->PlaceholderText = placeholderText;
    DateDiff_ToDate->PlaceholderText = placeholderText;

    CopyMenuItem->Text = AppResourceProvider::GetInstance().GetResourceString(L"copyMenuItem");
    m_dateCalcOptionChangedEventToken = DateCalculationOption->SelectionChanged +=
        ref new SelectionChangedEventHandler(this, &DateCalculator::DateCalcOption_Changed);
}

void DateCalculator::FromDate_DateChanged(_In_ CalendarDatePicker ^ sender, _In_ CalendarDatePickerDateChangedEventArgs ^ e)
{
    if (e->NewDate != nullptr)
    {
        auto dateCalcViewModel = safe_cast<DateCalculatorViewModel ^>(this->DataContext);
        dateCalcViewModel->FromDate = e->NewDate->Value;
        TraceLogger::GetInstance().LogDateCalculationModeUsed(false /* AddSubtractMode */);
    }
    else
    {
        ReselectCalendarDate(sender, e->OldDate->Value);
    }
}

void DateCalculator::ToDate_DateChanged(_In_ CalendarDatePicker ^ sender, _In_ CalendarDatePickerDateChangedEventArgs ^ e)
{
    if (e->NewDate != nullptr)
    {
        auto dateCalcViewModel = safe_cast<DateCalculatorViewModel ^>(this->DataContext);
        dateCalcViewModel->ToDate = e->NewDate->Value;
        TraceLogger::GetInstance().LogDateCalculationModeUsed(false /* AddSubtractMode */);
    }
    else
    {
        ReselectCalendarDate(sender, e->OldDate->Value);
    }
}

void DateCalculator::AddSubtract_DateChanged(_In_ CalendarDatePicker ^ sender, _In_ CalendarDatePickerDateChangedEventArgs ^ e)
{
    if (e->NewDate != nullptr)
    {
        auto dateCalcViewModel = safe_cast<DateCalculatorViewModel ^>(this->DataContext);
        dateCalcViewModel->StartDate = e->NewDate->Value;
        TraceLogger::GetInstance().LogDateCalculationModeUsed(true /* AddSubtractMode */);
    }
    else
    {
        ReselectCalendarDate(sender, e->OldDate->Value);
    }
}

void CalculatorApp::DateCalculator::OffsetValue_Changed(_In_ Platform::Object ^ sender, _In_ SelectionChangedEventArgs ^ e)
{
    auto dateCalcViewModel = safe_cast<DateCalculatorViewModel ^>(this->DataContext);
    // do not log diagnostics for no-ops and initialization of combo boxes
    if (dateCalcViewModel->DaysOffset != 0 || dateCalcViewModel->MonthsOffset != 0 || dateCalcViewModel->YearsOffset != 0)
    {
        TraceLogger::GetInstance().LogDateCalculationModeUsed(true /* AddSubtractMode */);
    }
}

void DateCalculator::OnCopyMenuItemClicked(_In_ Object ^ sender, _In_ RoutedEventArgs ^ e)
{
    auto calcResult = safe_cast<TextBlock ^>(ResultsContextMenu->Target);

    CopyPasteManager::CopyToClipboard(calcResult->Text);
}

void CalculatorApp::DateCalculator::OnLoaded(_In_ Platform::Object ^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs ^ e)
{
    // bw:20190920 ViewModelのプロパティが変化したときのイベントハンドラーをセットする
    auto dateCalcViewModel = safe_cast<DateCalculatorViewModel ^>(this->DataContext);
    dateCalcViewModel->PropertyChanged +=
        ref new Windows::UI::Xaml::Data::PropertyChangedEventHandler(this, &CalculatorApp::DateCalculator::OnPropertyChangedEventHandler);
}

// bw:20190920 ViewModelのプロパティが変化したときのイベントハンドラー
void DateCalculator::OnPropertyChangedEventHandler(_In_ Platform::Object ^ s, _In_ Windows::UI::Xaml::Data::PropertyChangedEventArgs ^ e)
{
    auto propertyName = e->PropertyName;
    if (propertyName == L"CalendarIdentifier")
    {
        // CalendarIdentifierは依存関係プロパティであるのにバインドが効かないようなので、
        // コードビハインドからセットする
        SetCalendarIdentifiers();
    }
}

// bw:20190917 西暦⇔和暦表示切替
void DateCalculator::SetCalendarIdentifiers()
{
    // CalendarIdentifierをバインドしても、表示が切り替わってくれない。(おそらくスタイル定義の問題)
    // そのため、コードビハインドから切り替える。

    auto dateCalcViewModel = safe_cast<DateCalculatorViewModel ^>(this->DataContext);
    auto calendarIdentifier = dateCalcViewModel->CalendarIdentifier;

    DateDiff_FromDate->CalendarIdentifier = calendarIdentifier;
    DateDiff_ToDate->CalendarIdentifier = calendarIdentifier;
    if (AddSubtract_FromDate != nullptr) // AddSubtractDateGridはLazy
    {
        AddSubtract_FromDate->CalendarIdentifier = calendarIdentifier;
    }
}



void DateCalculator::CloseCalendarFlyout()
{
    if (DateDiff_FromDate->IsCalendarOpen)
    {
        DateDiff_FromDate->IsCalendarOpen = false;
    }

    if (DateDiff_ToDate->IsCalendarOpen)
    {
        DateDiff_ToDate->IsCalendarOpen = false;
    }

    if ((AddSubtract_FromDate != nullptr) && (AddSubtract_FromDate->IsCalendarOpen))
    {
        AddSubtract_FromDate->IsCalendarOpen = false;
    }
}

void DateCalculator::SetDefaultFocus()
{
    DateCalculationOption->Focus(::FocusState::Programmatic);
}

void DateCalculator::DateCalcOption_Changed(_In_ Platform::Object ^ sender, _In_ Windows::UI::Xaml::Controls::SelectionChangedEventArgs ^ e)
{
    FindName("AddSubtractDateGrid");
    DateCalculationOption->SelectionChanged -= m_dateCalcOptionChangedEventToken;
}

// bw:20190917 DateDiffGridがロードされた時のイベントハンドラー
void CalculatorApp::DateCalculator::DateDiffGrid_Loaded(_In_ Platform::Object ^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs ^ e)
{
    // Date プロパティに何もセットされていないと、西暦／和暦の表示が変わらない。
    // 既定値である今日の日付をセットする。
    auto today = (ref new Windows::Globalization::Calendar())->GetDateTime();
    DateDiff_FromDate->Date = today;
    DateDiff_ToDate->Date = today;
}

void CalculatorApp::DateCalculator::AddSubtractDateGrid_Loaded(_In_ Platform::Object ^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs ^ e)
{
    const auto& localizationSettings = LocalizationSettings::GetInstance();

    AddSubtract_FromDate->PlaceholderText = DateDiff_FromDate->PlaceholderText;
    AddSubtract_FromDate->CalendarIdentifier = localizationSettings.GetCalendarIdentifier();
    AddSubtract_FromDate->FirstDayOfWeek = localizationSettings.GetFirstDayOfWeek();
    AddSubtract_FromDate->Language = localizationSettings.GetLocaleName();

    AddSubtract_FromDate->MinDate = DateDiff_FromDate->MinDate;
    AddSubtract_FromDate->MaxDate = DateDiff_FromDate->MaxDate;
    AddSubtract_FromDate->DateFormat = L"day month year";

    // bw:20190917 CalendarDatePicker の不具合対策
    // Date プロパティに何もセットされていないと、西暦／和暦の表示が変わらない。
    // 既定値である今日の日付をセットする。
    auto today = (ref new Windows::Globalization::Calendar())->GetDateTime();
    AddSubtract_FromDate->Date = today;
    // 西暦⇔和暦表示切替（AddSubtractDateGridはlazy loadなので、改めて設定する必要がある）
    AddSubtract_FromDate->CalendarIdentifier = (safe_cast<DateCalculatorViewModel ^>(this->DataContext))->CalendarIdentifier;
}

void DateCalculator::ReselectCalendarDate(_In_ Windows::UI::Xaml::Controls::CalendarDatePicker ^ calendarDatePicker, Windows::Foundation::DateTime dateTime)
{
    // Reselect the unselected Date
    calendarDatePicker->Date = ref new Box<DateTime>(dateTime);

    // Dismiss the Calendar flyout
    calendarDatePicker->IsCalendarOpen = false;
}

void DateCalculator::OffsetDropDownClosed(_In_ Object ^ sender, _In_ Object ^ e)
{
    RaiseLiveRegionChangedAutomationEvent(/* DateDiff mode */ false);
}

void DateCalculator::CalendarFlyoutClosed(_In_ Object ^ sender, _In_ Object ^ e)
{
    auto dateCalcViewModel = safe_cast<DateCalculatorViewModel ^>(this->DataContext);
    RaiseLiveRegionChangedAutomationEvent(dateCalcViewModel->IsDateDiffMode);

    //bw:20190922 CalendarDatePicker の不具合対策
    // CalendarIdentifier を変更→ドロップダウン→また CalendarIdentifier を変更
    // すると、次にカレンダードロップダウンを開くときにメモリーリークするとともに、
    // ドロップダウンが正しく表示されない。
    // そこで、ドロップダウンを1回開いたら、切り替えスイッチは無効とする。
    UseJapaneseCalendarSwitch->IsEnabled = false;
}

void DateCalculator::RaiseLiveRegionChangedAutomationEvent(_In_ bool isDateDiffMode)
{
    TextBlock ^ resultTextBlock = (isDateDiffMode ? DateDiffAllUnitsResultLabel : DateResultLabel);
    String ^ automationName = AutomationProperties::GetName(resultTextBlock);
    TextBlockAutomationPeer::FromElement(resultTextBlock)->RaiseAutomationEvent(AutomationEvents::LiveRegionChanged);
}

void DateCalculator::AddSubtractOption_Checked(_In_ Object ^ sender, _In_ RoutedEventArgs ^ e)
{
    RaiseLiveRegionChangedAutomationEvent(/* DateDiff mode */ false);
}

void CalculatorApp::DateCalculator::OnVisualStateChanged(Platform::Object ^ sender, Windows::UI::Xaml::VisualStateChangedEventArgs ^ e)
{
    auto state = std::wstring(e->NewState->Name->Begin());
    TraceLogger::GetInstance().LogVisualStateChanged(ViewMode::Date, state);
}
