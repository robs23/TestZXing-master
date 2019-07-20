using Microsoft.AppCenter.Distribute;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        
    }
}
