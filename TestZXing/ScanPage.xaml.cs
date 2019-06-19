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
                        Place = await Keeper.GetPlace(result.Text);
                        if (Place == null)
                        {
                            //check if this is MES process string
                            string[] mesStr = Regex.Split(result.Text, ";");
                            if(mesStr.Length == 7)
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
                                }
                                catch(Exception ex)
                                {
                                    await DisplayAlert("Problem z kodem", string.Format("Coś poszło nie tak podczas deserializacji kodu {0}", result.Text) + ". Opis błędu: " + ex.Message, "OK");
                                }
                                
                            }
                            else
                            {
                                await DisplayAlert("Brak dopasowań", string.Format("Zeskanowany kod: {0} nie odpowiada żadnemu istniejącemu zasobowi. Spróbuj zeskanować kod jeszcze raz.", result.Text), "OK");
                            }
                        }
                        else
                        {
                            
                            List<Process> Pros = new List<Process>();
                            try
                            {
                                Pros = await Place.GetProcesses(true);
                                await Navigation.PushAsync(new ScanningResults(Pros,Place));
                                
                            }
                            catch (Exception ex)
                            {
                                PopupNavigation.Instance.PopAsync(true); // Hide loading screen
                                await DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
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
            btnScan.IsEnabled = true;
            //------------Scanning Bypass-------------------
            //Looper.IsVisible = true;
            //Looper.IsRunning = true;
            //try
            //{
            //    Place = await Keeper.GetPlace("0u5TxEpXEGKFuRt3rk0QA");
            //    Place = await Keeper.GetPlace("xx");
            //    if (Place == null)
            //    {
            //        await DisplayAlert("Brak dopasowań", string.Format("Zeskanowany kod: {0} nie odpowiada żadnemu istniejącemu zasobowi. Spróbuj zeskanować kod jeszcze raz.", "xx"), "OK");
            //    }
            //    else
            //    {
            //        lblScanResult.Text = "Zeskanowano: " + Place.Name;
            //        Pros = new List<Process>();
            //        try
            //        {
            //            Pros = await Place.GetProcesses(true);
            //        }
            //        catch (Exception ex)
            //        {
            //            Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "btnScan_Clicked", Time = DateTime.Now, Message = ex.Message };
            //        }
            //        finally
            //        {
            //            vm = new ProcessInPlaceViewModel(Pros);
            //        }
            //        BindingContext = vm;
            //        Looper.IsRunning = false;
            //        Looper.IsVisible = false;
            //        lblScanResult.IsVisible = true;
            //        lblGetOrder.IsVisible = true;
            //        lstProcesses.IsVisible = true;
            //        btnOpenProcess.IsVisible = true;
            //    }

            //}
            //catch (Exception ex)
            //{
            //    await DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
            //}
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
            //Application.Current.MainPage.Navigation.PushAsync(new LastPlaces());

            WiFiInfo wi = await DependencyService.Get<IWifiHandler>().GetConnectedWifi(true);
            if (wi == null)
            {
                await DisplayAlert("Odmowa","Nie można pobrać nazwy sieci w wyniku odmowy udzielenia pozwolenia dostępu do lokacji", "OK");
            }
            else
            {
                await DisplayAlert("Connection status", string.Format("Sieć: {0}, siła sygnału: {1}", wi.SSID, wi.Signal), "OK");
            }
        }

        private void btnClose_Clicked(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }
    }
}