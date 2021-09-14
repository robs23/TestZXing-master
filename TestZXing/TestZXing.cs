using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TestZXing.Classes;
using TestZXing.Models;
using TestZXing.Static;
using Xamarin.Forms;


namespace TestZXing
{
    public class App : Application
    {

        public App()
        {
            MainPage = new NavigationPage(new LoginPage());
            Xamarin.Essentials.VersionTracking.Track();
        }

        public App(string filePath)
        {
            MainPage = new NavigationPage(new LoginPage());
            Static.RuntimeSettings.LocalDbPath = filePath;
            Xamarin.Essentials.VersionTracking.Track();
        }

        protected async override void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start($"android={Static.Secrets.AppCenterSecret}", typeof(Analytics), typeof(Crashes));
            Analytics.SetEnabledAsync(true);
            FileKeeper files = new FileKeeper(uploadKeeper: true);
            RuntimeSettings.UploadKeeper = files; 
            //if (await Crashes.HasCrashedInLastSessionAsync())
            //{
            //    Functions.CreateZipFile();
            //    if (!string.IsNullOrEmpty(RuntimeSettings.ZippedLogFile))
            //    {
            //        Crashes.GetErrorAttachments = (ErrorReport report) =>
            //        {
            //            // Your code goes here.
            //            return new ErrorAttachmentLog[]
            //            {
            //            ErrorAttachmentLog.AttachmentWithBinary(File.ReadAllBytes(RuntimeSettings.ZippedLogFile), "logs.zip", "application/x-zip-compressed")
            //            };
            //        };
            //    }
                
            //}
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            MessagingCenter.Send<object, string>(this, "OnSleepKey", "GoingSleep");
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }



    }
}
