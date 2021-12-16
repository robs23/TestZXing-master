using System;
using System.Collections.Generic;
using System.Text;
using TestZXing.Interfaces;
using TestZXing.Static;
using Xamarin.Forms;

namespace TestZXing.Classes
{
    public class AppUpdateService
    {
        //initial code from https://www.c-sharpcorner.com/article/how-to-download-files-in-xamarin-forms/

        IDownloader downloader = DependencyService.Get<IDownloader>();
        IFileHandler fileHandler = DependencyService.Get<IFileHandler>();

        public void DownloadAPK()
        {
            string url = Secrets.ApiAddress + $"GetApk?token=" + Secrets.TenantToken;
            downloader.OnFileDownloaded += OnFileDownloaded;
            downloader.DownloadFile(url, "JDE Downloader");
        }

        public void OpenApk()
        {
            string filePath = "";
            fileHandler.OpenApk(filePath);
        }

        private async void OnFileDownloaded(object sender, DownloadEventArgs e)
        {
            if (e.FileSaved)
            {
                var approved = await App.Current.MainPage.DisplayAlert("Aktualizator JDE Scan", "Pomyślnie zapisano plik instalacyjnyh", "Aktualizuj", "Nie teraz");
                if (approved)
                {
                    OpenApk();
                }
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Aktualizator JDE Scan", "Zapis pliku instalacyjnego zakończony niepowodzeniem", "Zamknij");
            }
        }
    }
}
