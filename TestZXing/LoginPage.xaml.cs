using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestZXing.Classes;
using TestZXing.Models;
using TestZXing.Static;
using TestZXing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace TestZXing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        DataService ds;
        UsersKeeper keeper;
        LoginViewModel vm;
        ZXingScannerPage scanPage;

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

        private async void btnScanQr_Clicked(object sender, EventArgs e)
        {
            btnScanQr.IsEnabled = false;
            Anal nAnal = new Anal("Próba logowania QR");
            scanPage = new ZXingScannerPage();
            scanPage.OnScanResult += (result) =>
            {
                scanPage.IsScanning = false;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    Navigation.PopAsync();
                    PopupNavigation.Instance.PushAsync(new LoadingScreen(), true);
                    try
                    {
                        string res = result.Text;
                        if(res.Substring(0,1)=="[" && res.Substring(res.Length - 1, 1) == "]")
                        {
                            //it's within []
                            if (res.Substring(1, 3) == "UID")
                            {
                                //it's certainly user's qr code
                                int uid;
                                string pass = "";
                                
                                try
                                {
                                    res = res.Substring(1, res.Length - 2); //trim the brackets
                                    var reses = Regex.Split(res, ";");
                                    var uids = Regex.Split(reses[0], "UID=");
                                    var passes = Regex.Split(reses[1], "PASS=");
                                    bool parsable = Int32.TryParse(uids[1],out uid);
                                    pass = passes[1];
                                    if(keeper.Items.Where(i=>i.UserId==uid && i.Password == pass).Any())
                                    {
                                        //password matches, let user in
                                        User theUser = keeper.Items.Where(i => i.UserId == uid && i.Password == pass).FirstOrDefault();
                                        RuntimeSettings.UserId = theUser.UserId;
                                        RuntimeSettings.CurrentUser = theUser;
                                        RuntimeSettings.TenantId = theUser.TenantId;
                                        theUser.Login();
                                        PopupNavigation.Instance.PopAsync(true); // Hide loading screen
                                        await Application.Current.MainPage.Navigation.PushAsync(new ScanPage());
                                        nAnal = new Anal("Zalogowano kodem QR");
                                    }
                                    else
                                    {
                                        await DisplayAlert("Błędne dane", "Błędny użytkownik lub hasło!", "OK");
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Static.Functions.CreateError(ex, "User QR code could not be parsed", nameof(this.btnScanQr_Clicked), this.GetType().Name);
                                }

                                
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Nieokreślony problem", ex.Message, "OK");
                        Static.Functions.CreateError(ex, "Problem with scanning page", nameof(this.btnScanQr_Clicked), this.GetType().Name);
                    }
                });
            };
            await Navigation.PushAsync(scanPage);
            btnScanQr.IsEnabled = true;
        }
    }
}