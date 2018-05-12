using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Classes;
using TestZXing.Models;
using Xamarin.Forms;

namespace TestZXing
{
    public class LoginPage: ContentPage
    {
        Button btnLogin;
        Picker pick;
        Label lbl;
        Entry entry;
        DataService ds;
        UsersKeeper keeper;
        string output;

        public LoginPage() : base()
        {
            keeper = new UsersKeeper();
            ds = new DataService();
            btnLogin = new Button() { Text = "Zaloguj" };
            pick = new Picker { HorizontalOptions = LayoutOptions.Center, Title = "Wybierz użytkownika" };
            lbl = new Label { HorizontalTextAlignment = TextAlignment.Center, Text = "Wybierz użytkownika i podaj hasło" };
            entry = new Entry { HorizontalTextAlignment = TextAlignment.Center, Placeholder = "Hasło", IsPassword = true };

            btnLogin.Clicked += async (sender, e) =>
            {
                if(pick != null && String.IsNullOrEmpty(entry.Text)==false)
                {

                    await Application.Current.MainPage.Navigation.PushAsync(new ScanPage(output));
                }
                else
                {
                    await DisplayAlert("Błąd","Nie wybrano użytkownika z listy rozwijanej lub nie podano hasła!", "OK");
                }
                
            };

            StackLayout layout = new StackLayout { VerticalOptions = LayoutOptions.Center };
            layout.Children.Add(lbl);
            layout.Children.Add(pick);
            layout.Children.Add(entry);
            layout.Children.Add(btnLogin);
            Title = "Logowanie";
            Content = layout;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                output = await ds.ReloadUsers();
                keeper.Users = JsonConvert.DeserializeObject<List<User>>(output);
                if (keeper.Users.Any())
                {
                    foreach (User user in keeper.Users)
                    {
                        pick.Items.Add(user.FullName);
                    }
                }
            }catch(Exception ex)
            {
                string error = ex.Message;
                await DisplayAlert("Błąd", error, "OK");
            }
            
        }

    }
}
