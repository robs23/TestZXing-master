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
        DataService ds;
        List<User> items;

        public App()
        {
            UsersKeeper keeper = new UsersKeeper();
            ds = new DataService();
            Button b = new Button() { Text = "Zaloguj" };
            string output = ds.ReloadUsers().Result;

            b.Clicked += async (sender, e) =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new ScanPage(output));
            };

            // The root page of your application
            var content = new ContentPage
            {
                Title = "Zgłoszenia serwisowe",
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        new Label {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Wybierz użytkownika i podaj hasło"
                        }, new Picker{
                            HorizontalOptions = LayoutOptions.Center,
                            Title ="Wybierz użytkownika"
                        }, new Entry{
                            HorizontalTextAlignment = TextAlignment.Center,
                            Placeholder = output //RuntimeSettings.TenantToken //"Hasło"
                        }, b

                    }
                }
            };

            MainPage = new NavigationPage(content);
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
