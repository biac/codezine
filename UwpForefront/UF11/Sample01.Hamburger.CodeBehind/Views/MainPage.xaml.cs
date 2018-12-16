using Microsoft.Toolkit.Uwp.Notifications;
using Sample01.Hamburger.CodeBehind.Helpers;
using Sample01.Hamburger.CodeBehind.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Sample01.Hamburger.CodeBehind.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



        // bw: トースト機能を使う
        private void OnSendToast(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //Singleton<ToastNotificationsService>.Instance.ShowToastNotificationSample();
            // bw: 引数にテキストボックスの内容を渡す
            Singleton<ToastNotificationsService>.Instance
                .ShowToastNotificationSample(this.TextBox1.Text);
        }




        // bw: テキストボックスの内容をLocalSettingsに保存／復元する

        // bw: LocalSettingsに保存するデータのキー
        const string SettingsKey = "MainPage_ToastMessage";

        // bw: ページが表示されるときに呼び出されるメソッド
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // サスペンドに入るときのイベントハンドラーをセット（Suspend and Resume機能）
            Singleton<SuspendAndResumeService>.Instance.OnBackgroundEntering
                += OnBackgroundEntering;

            // ページ表示時にテキストボックスの内容を復元（SettingsStorage機能）
            // このように表示内容を復元するなら、ページに NavigationCacheMode="Required" を指定しなくて済む（＝メモリを節約できる）。
            if (await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey)
                    is string msg)
                this.TextBox1.Text = msg;
        }

        // bw: 他のページに移動するときに呼び出されるメソッド
        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // サスペンドに入るときのイベントハンドラーを解除（Suspend and Resume機能）
            Singleton<SuspendAndResumeService>.Instance.OnBackgroundEntering
                -= OnBackgroundEntering;

            // ページから離れる時にテキストボックスの内容を保存（SettingsStorage機能）
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, this.TextBox1.Text);
        }

        // bw: サスペンドに入るときに呼び出されるイベントハンドラー
        // これは、後から追加したSuspend and Resume機能によって呼び出される
        private async void OnBackgroundEntering(object sender, OnBackgroundEnteringEventArgs e)
        {
            // サスペンドに入るときにテキストボックスの内容を保存（SettingsStorage機能）
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, this.TextBox1.Text);
        }
    }
}
