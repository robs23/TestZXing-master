﻿using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        protected override void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start($"android={Static.Secrets.AppCenterSecret}", typeof(Analytics), typeof(Crashes), typeof(Distribute));
            Analytics.SetEnabledAsync(true);
            Distribute.SetEnabledAsync(true);
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
