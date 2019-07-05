using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestZXing.Interfaces;
using TestZXing.Models;
using TestZXing.Static;
using TestZXing.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace TestZXing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanPage : ContentPage
    {
        ZXingScannerPage scanPage;
        Place Place;
        PlacesKeeper Keeper;
        UserSettingsViewModel vm;

        public ScanPage()
        {
            InitializeComponent();
            Keeper = new PlacesKeeper();
            Place = new Place();
            vm = new UserSettingsViewModel();
            BindingContext = vm;
        }

        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            btnScan.IsEnabled = false;
            PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync<CameraPermission>();

            if (status != PermissionStatus.Granted)
            {
                status = await CrossPermissions.Current.RequestPermissionAsync<CameraPermission>();
            }
            if(status != PermissionStatus.Granted)
            {
                await DisplayAlert("Odmowa", "Skanowanie kodu QR wymaga uprawnień do użycia kamery. Użytkownik odmówił tego uprawnienia. Spróbuj jeszcze raz i przyznaj odpowiednie uprawnienie", "OK");
            }
            else
            {
                scanPage = new ZXingScannerPage();
                scanPage.OnScanResult += (result) =>
                {
                    //DateTime _start = DateTime.Now;
                    scanPage.IsScanning = false;

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        Navigation.PopAsync();
                        PopupNavigation.Instance.PushAsync(new LoadingScreen(), true);
                        try
                        {

                                //check if this is MES process string
                                string[] mesStr = Regex.Split(result.Text, ";");
                                if (mesStr.Length == 7)
                                {
                                    //there are 7 fields split by ; in the string, there's good chance it comes from MES
                                    MesString ms = new MesString();
                                    try
                                    {
                                        ms.MesId = mesStr[0];
                                        ms.MesDate = DateTime.Parse(mesStr[1]);
                                        ms.SetName = mesStr[2];
                                        ms.ActionTypeName = mesStr[3];
                                        ms.Reason = mesStr[5];
                                    }
                                    catch (Exception ex)
                                    {
                                        await DisplayAlert("Problem z kodem", string.Format("Coś poszło nie tak podczas deserializacji kodu {0}", result.Text) + ". Opis błędu: " + ex.Message, "OK");
                                    }
                                    try
                                    {
                                        //check if such a mesId exists before creating new one
                                        ProcessKeeper pKeeper = new ProcessKeeper();
                                        Process nProcess = await pKeeper.GetProcess(ms.MesId);

                                        //pass everything to ProcessPage.xaml
                                        if (nProcess == null)
                                        {
                                            await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage(ms));
                                        }
                                        else
                                        {
                                            await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage(ms, nProcess));
                                        }
                                    }catch(Exception ex)
                                    {
                                        Static.Functions.CreateError(ex, "No connection", nameof(scanPage.OnScanResult), this.GetType().Name);
                                    }  

                                }
                                else
                                {
                                    //try to find place of such token
                                    Place = await Keeper.GetPlace(result.Text);
                                    if (Place == null)
                                    {
                                        await DisplayAlert("Brak dopasowań", string.Format("Zeskanowany kod: {0} nie odpowiada żadnemu istniejącemu zasobowi. Spróbuj zeskanować kod jeszcze raz.", result.Text), "OK");
                                    }
                                    else
                                    {
                                        List<Process> Pros = new List<Process>();
                                        try
                                        {
                                            Pros = await Place.GetProcesses(true);
                                            //await DisplayAlert("Czas", $"Zajęło {(DateTime.Now - _start).TotalMilliseconds} sekund", "Ok");
                                            await Navigation.PushAsync(new ScanningResults(Pros, Place));

                                        }
                                        catch (Exception ex)
                                        {
                                            PopupNavigation.Instance.PopAsync(true); // Hide loading screen
                                            await DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
                                        }
                                    }
                                        
                                }
                            
       
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
                        }
                        PopupNavigation.Instance.PopAsync(true); // Hide loading screen
                    });
                };
                
                await Navigation.PushAsync(scanPage);
            }
            
            btnScan.IsEnabled = true;
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

        private async void btnOpenProcesses_Clicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ProcessesPage());
            //await Application.Current.MainPage.Navigation.PushAsync(new ActiveProcesses(new ActiveProcessesViewModel()));
        }

        private void btnLogout_Clicked(object sender, EventArgs e)
        {
            RuntimeSettings.UserId = 0;
            RuntimeSettings.CurrentUser = null;
            LoginPage page = new LoginPage();
            NavigationPage.SetHasBackButton(page, false);
            Navigation.PushAsync(page);
        }

        private async void btnLastPlaces_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushAsync(new LastPlaces());

        }

        private void btnClose_Clicked(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }

        private async void BtnWifiStatus_Clicked(object sender, EventArgs e)
        {
            ////WiFiInfo wi = await DependencyService.Get<IWifiHandler>().GetConnectedWifi(true);
            //List<WiFiInfo> wis = await DependencyService.Get<IWifiHandler>().GetAvailableWifis(true);
            //if (wis == null)
            //{
            //    await DisplayAlert("Odmowa", "Aby sprawdzić nazwę sieci i moc sygnału, potrzebne jest uprawnienie do lokalizacji. Użytkownik odmówił tego uprawnienia. Spróbuj jeszcze raz i przyznaj odpowiednie uprawnienie", "OK");
            //}
            //else
            //{
            //    string status = "";

            //    foreach (WiFiInfo w in wis.OrderByDescending(i => i.Signal))
            //    {
            //        string con = "";
            //        if (w.IsConnected)
            //        {
            //            con = "(P)";
            //        }
            //        status += w.SSID + $" [{w.BSSID}] ({w.Signal}){con}, \n";
            //    }

            //    WiFiInfo wi = await DependencyService.Get<IWifiHandler>().GetConnectedWifi();

            //    await DisplayAlert("Connection status", $"Podłączona sieć: {wi.SSID} [{wi.BSSID}].\nDostępne sieci: {status}", "OK");
            //}
            if(!Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi) || Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                //either Wifi is off or there's no connection to the internet
                //connect preferred network
                PopupNavigation.Instance.PushAsync(new LoadingScreen("Brak internetu.. Próbuje nawiązać połączenie"), true);
                var res = await DependencyService.Get<IWifiHandler>().ConnectPreferredWifi();
                PopupNavigation.Instance.PopAsync(true); // Hide loading screen
                if (!res.Item1)
                {
                    //Couldn't connect to the network..
                    await DisplayAlert("Błąd połączenia", res.Item2, "Ok");
                }
            }
        }
    }
}