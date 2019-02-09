using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Classes;
using TestZXing.Models;
using TestZXing.Static;
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
                await PopupNavigation.Instance.PushAsync(new LoadingScreen(),true); // Show loading screen
                string _Result = await keeper.Reload();
                if (_Result == "OK")
                {
                    if (keeper.Items.Any())
                    {
                        vm = new LoginViewModel(keeper.Items);
                        BindingContext = vm;
                    }
                    else
                    {
                        await DisplayAlert("Brak użytkowników", "Brak użytkowników na liście!", "OK");
                    }
                }
                else
                {
                    PopupNavigation.Instance.PopAsync(true); // Hide loading screen
                    await DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
                    var closer = DependencyService.Get<ICloseApplication>();
                    closer?.closeApplication();
                }
                
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                await DisplayAlert("Błąd", error, "OK");
            }
            finally
            {
                
            }
            PopupNavigation.Instance.PopAsync(true);
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
                    RuntimeSettings.UserId = vm.SelectedUser.UserId;
                    RuntimeSettings.CurrentUser = vm.SelectedUser;
                    RuntimeSettings.TenantId = vm.SelectedUser.TenantId;
                    vm.SelectedUser.Login();
                    await Application.Current.MainPage.Navigation.PushAsync(new ScanPage());
                }
                else
                {
                    await DisplayAlert("Błędne dane", "Podano błędne hasło!", "OK");
                }
            }
            
        }

        protected override bool OnBackButtonPressed()
        {
            // Begin an asyncronous task on the UI thread because we intend to ask the users permission.
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("Koniec pracy?", "Czy chcesz wyjść z aplikacji?", "Tak", "Nie"))
                {
                    var closer = DependencyService.Get<ICloseApplication>();
                    closer?.closeApplication();
                }
            });

            return true;
        }
    }
}