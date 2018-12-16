using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Sample02.Tabs.MvvmLight.Helpers;
using Sample02.Tabs.MvvmLight.Services;
using Windows.Storage;

namespace Sample02.Tabs.MvvmLight.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // bw: トーストに表示する文字列（テキストボックスに2ウェイでバインド）
        private string _toastMessage = "トースト通知の機能を使ってみた";
        public string ToastMessage
        {
            get => _toastMessage; 
            set => Set(ref _toastMessage, value);
        }

        // bw: トースト通知を出すコマンド（ボタンにバインド）
        private ICommand _showToastCommand;
        public ICommand ShowToastCommand
        {
            get
            {
                if (_showToastCommand == null)
                {
                    // RelayCommandはMVVM Lightの機能
                    _showToastCommand = new RelayCommand(() =>
                    {
                        Singleton<ToastNotificationsService>.Instance
                            .ShowToastNotificationSample(ToastMessage);
                    });
                }
                return _showToastCommand;
            }
        }

        public MainViewModel()
        {
        }



        // bw: LocalSettingsに保存するデータのキー
        const string SettingsKey = "MainPage_ToastMessage";

        // bw: モデルの初期化
        public async void Initialize()
        {
            // bw: トーストに表示する文字列をLocalSettingsから取得
            if (await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey)
                    is string msg)
                ToastMessage = msg;

            // bw: サスペンドに入るときのイベントハンドラーをセット
            Singleton<SuspendAndResumeService>.Instance.OnBackgroundEntering += OnBackgroundEntering;
            // ※アプリ終了までこのページは生存するので、ハンドラーの解除は不要
        }

        // bw: サスペンドに入るときに呼び出されるイベントハンドラー
        // ※アプリ終了時にも呼び出される
        private async void OnBackgroundEntering(object sender, OnBackgroundEnteringEventArgs e)
        {
            // サスペンドに入るときにトーストに表示する文字列を保存（SettingsStorage機能）
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, ToastMessage);
        }
    }
}
