using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Classes;
using TestZXing.Models;
using TestZXing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        DataService ds;
        UsersKeeper keeper;
        string output;
        LoginViewModel vm;

        public LoginPage()
        {
            InitializeComponent();
            ds = new DataService();
            keeper = new UsersKeeper();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                Looper.IsVisible = true;
                Looper.IsRunning = true;
                output = await ds.ReloadUsers();
                keeper.Users = JsonConvert.DeserializeObject<List<User>>(output);
                if (keeper.Users.Any())
                {
                    vm = new LoginViewModel(keeper.Users);
                    BindingContext = vm;
                    //foreach (User user in keeper.Users)
                    //{
                    //    pick.Items.Add(user.FullName);
                    //}
                }
                else
                {
                    await DisplayAlert("Brak użytkowników", "Brak użytkowników na liście!", "OK");
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                await DisplayAlert("Błąd", error, "OK");
            }
            finally
            {
                Looper.IsVisible = false;
                Looper.IsRunning = false;
            }


        }

        private async void btnLogin_Clicked(object sender, EventArgs e)
        {
            if (cmbUsernames == null || String.IsNullOrEmpty(txtPassword.Text) == true)
            {
                await DisplayAlert("Podaj dane", "Nie wybrano użytkownika z listy rozwijanej lub nie podano hasła!", "OK");
            }
            else
            {
                if (vm.SelectedUser.Password == txtPassword.Text)
                {
                    //password matches, let user in
                    await Application.Current.MainPage.Navigation.PushAsync(new ScanPage());
                }
                else
                {
                    await DisplayAlert("Błędne dane", "Podano błędne hasło!", "OK");
                }
            }
            
        }
    }
}