using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        public ScanPage()
        {
            InitializeComponent();
            Keeper = new PlacesKeeper();
            Place = new Place();
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
                    Looper.IsVisible = true;
                    Looper.IsRunning = true;
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
                                    ms.Reason = mesStr[6];

                                    // you'got all you need, try to get list of places for scanned Set
                                    List<Place> Places = new List<Place>();
                                    Places = await Keeper.GetPlacesBySetName(ms.SetName);
                                    if (Places == null)
                                    {
                                        await DisplayAlert("Nie znaleziono zasobu", string.Format("Nie udało się znaleźć zasobu dla instalacji {0}", ms.SetName), "OK");
                                    }
                                    else
                                    {
                                        //first 'get' scanned action type just to make sure she'll be added to the list if missing
                                        ActionType at = await new ActionTypesKeeper().GetActionTypeByName(ms.ActionTypeName);
                                        //pass everything to ProcessPage.xaml
                                        await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage(Places,ms));
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
                                await DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
                            }
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
                    }
                    Looper.IsVisible = false;
                    Looper.IsRunning = false;
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
            await Application.Current.MainPage.Navigation.PushAsync(new ActiveProcesses(new ActiveProcessesViewModel()));
        }
    }
}