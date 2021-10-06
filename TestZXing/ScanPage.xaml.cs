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
using TestZXing.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;

namespace TestZXing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanPage : ContentPage
    {
        ZXingScannerPage scanPage;
        Place Place;
        PlacesKeeper Keeper;
        User vm;

        public ScanPage()
        {
            InitializeComponent();
            Keeper = new PlacesKeeper();
            Place = new Place();
            vm = Static.RuntimeSettings.CurrentUser;
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
                var options = new ZXing.Mobile.MobileBarcodeScanningOptions
                {
                    PossibleFormats = new List<ZXing.BarcodeFormat>
                    {
                        ZXing.BarcodeFormat.QR_CODE
                    },
                    TryHarder = false,
                    AutoRotate = false,
                    TryInverted = false,
                };
                scanPage = new ZXingScannerPage();
                scanPage.AutoFocus();
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
                                        await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage(ms, nProcess, true));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Static.Functions.CreateError(ex, "No connection", nameof(scanPage.OnScanResult), this.GetType().Name);
                                }

                            }
                            else if (result.Text.Contains("<Part>"))
                            {
                                //it's a part
                                string res = result.Text.Replace("<Part>", "");
                                
                                Part Part = await new PartKeeper().GetByToken(res);
                                if (Part != null)
                                {
                                    if (Part.IsArchived == true)
                                    {
                                        await Application.Current.MainPage.DisplayAlert("Część zarchiwizowana", "Ta część została zarchiwizowana! Być może ta część występuje teraz pod nowym numerem, w takim przypadku należy wydrukować i okleić ją nowym kodem", "OK");
                                    }
                                    else
                                    {
                                        await Application.Current.MainPage.Navigation.PushAsync(new PartPage(Part));
                                    }
                                    
                                }
                                else
                                {
                                    DependencyService.Get<IToaster>().ShortAlert($"Nie znaleziono części oznaczonej kodem {res}..");
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
                                    try
                                    {
                                        if (Place.IsArchived == true)
                                        {
                                            await Application.Current.MainPage.DisplayAlert("Zasób zarchiwizowany", "Ten zasób został zarchiwizowany! Być może ten zasób występuje teraz pod nowym numerem, w takim przypadku należy wydrukować i okleić go nowym kodem", "OK");
                                        }
                                        else
                                        {
                                            await Navigation.PushAsync(new ScanningResults(Place, true));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                    if (PopupNavigation.Instance.PopupStack.Any()) { await PopupNavigation.Instance.PopAllAsync(true); }  // Hide loading screen
                                        await DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
                                    }
                                }
                                        
                            }
                            
       
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
                        }
                        if (PopupNavigation.Instance.PopupStack.Any()) { await PopupNavigation.Instance.PopAllAsync(true); }  // Hide loading screen
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
            //RuntimeSettings.CurrentUser.RemoveUserCredentials();
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
            //WiFiInfo wi = await DependencyService.Get<IWifiHandler>().GetConnectedWifi(true);
            var apiPing = await DependencyService.Get<IWifiHandler>().PingHost();
            string pingStatus = $"{Static.Secrets.ServerIp} : ";
            if (apiPing) { pingStatus += "Dostępny"; } else { pingStatus += "Niedostępny"; }
            List<WiFiInfo> wis = await DependencyService.Get<IWifiHandler>().GetAvailableWifis(true);
            if (wis == null)
            {
                await DisplayAlert("Odmowa", "Aby sprawdzić nazwę sieci i moc sygnału, potrzebne jest uprawnienie do lokalizacji. Użytkownik odmówił tego uprawnienia. Spróbuj jeszcze raz i przyznaj odpowiednie uprawnienie", "OK");
            }
            else
            {
                string status = "";

                foreach (WiFiInfo w in wis.OrderByDescending(i => i.Signal))
                {
                    string con = "";
                    if (w.IsConnected)
                    {
                        con = "(P)";
                    }
                    status += w.SSID + $" [{w.BSSID}] ({w.Signal}){con}, \n";
                }

                WiFiInfo wi = await DependencyService.Get<IWifiHandler>().GetConnectedWifi();

                await DisplayAlert("Connection status", $"Podłączona sieć: {wi.SSID} [{wi.BSSID}].\nDostępne sieci: {status}.\nPing: {pingStatus}", "OK");
            }
        }

        private void UserStatus_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushAsync(new DiaryPage());
        }

        private void BtnParts_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushAsync(new PartsPage());
        }

        private void btnVersion_Clicked(object sender, EventArgs e)
        {
            string ver = "";
            ver = VersionTracking.CurrentVersion;
            DependencyService.Get<IToaster>().LongAlert($"Zainstalowana wersja: {ver}");
        }

        private async void btnSyncFiles_Clicked(object sender, EventArgs e)
        {
            //FileKeeper keeper = new FileKeeper();
            //await keeper.RestoreUploadQueue();
            //await keeper.Upload();
            Application.Current.MainPage.Navigation.PushAsync(new UploadQueue());
        }

        private async void btnSendLog_Clicked(object sender, EventArgs e)
        {
            var path = await Functions.GetLogName();
            if(path != null)
            {
                await Functions.SendLogByEmail(path);
            }
            
        }
    }
}