using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        ProcessInPlaceViewModel vm;
        List<Process> Pros;

        public ScanPage()
        {
            InitializeComponent();
            lblScanResult.IsVisible = false;
            lblGetOrder.IsVisible = false;
            lstProcesses.IsVisible = false;
            btnOpenProcess.IsVisible = false;
            Keeper = new PlacesKeeper();
            Place = new Place();
        }

        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            scanPage = new ZXingScannerPage();
            lblScanResult.IsVisible = false;
            lblGetOrder.IsVisible = false;
            lstProcesses.IsVisible = false;
            btnOpenProcess.IsVisible = false;
            scanPage.OnScanResult += (result) =>
            {
                scanPage.IsScanning = false;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    Navigation.PopAsync();
                    Looper.IsVisible = true;
                    Looper.IsRunning = true;
                    Place = await Keeper.GetPlace(result.Text);
                    if (Place == null)
                    {
                        await DisplayAlert("Brak dopasowań", string.Format("Zeskanowany kod: {0} nie odpowiada żadnemu istniejącemu zasobowi. Spróbuj zeskanować kod jeszcze raz.", result.Text), "OK");
                    }
                    else
                    {
                        lblScanResult.Text = "Zeskanowano: " + Place.Name;
                        Pros = new List<Process>();
                        try
                        {
                            Pros = await Place.GetProcesses(true);
                        }
                        catch (Exception ex)
                        {
                            Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "btnScan_Clicked", Time = DateTime.Now, Message = ex.Message };
                        }
                        finally
                        {
                            vm = new ProcessInPlaceViewModel(Pros);
                        }
                        BindingContext = vm;
                        Looper.IsVisible = false;
                        Looper.IsRunning = false;
                        lblScanResult.IsVisible = true;
                        lblGetOrder.IsVisible = true;
                        lstProcesses.IsVisible = true;
                        btnOpenProcess.IsVisible = true;
                    }
                });
            };
            await Navigation.PushAsync(scanPage);
            //Looper.IsVisible = true;
            //Looper.IsRunning = true;
            //Place = await Keeper.GetPlace("NasbEYDEGEqDVdxkfhsa9A");
            //lblScanResult.Text = "Zeskanowano: " + Place.Name;
            //Pros = new List<Process>();
            //try
            //{
            //    Pros = await Place.GetProcesses(true);
            //}
            //catch (Exception ex)
            //{
            //    Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "btnScan_Clicked", Time = DateTime.Now, Message = ex.Message };
            //}
            //finally
            //{
            //    vm = new ProcessInPlaceViewModel(Pros);
            //}
            //BindingContext = vm;
            //Looper.IsRunning = false;
            //Looper.IsVisible = false;
            //lblScanResult.IsVisible = true;
            //lblGetOrder.IsVisible = true;
            //lstProcesses.IsVisible = true;
            //btnOpenProcess.IsVisible = true;

        }

        private async void UpdateList()
        {
            Looper.IsVisible = true;
            Looper.IsRunning = true;
            Pros = new List<Process>();
            try
            {
                Pros = await Place.GetProcesses(true);
            }
            catch (Exception ex)
            {
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "btnScan_Clicked", Time = DateTime.Now, Message = ex.Message };
            }
            finally
            {
                vm = new ProcessInPlaceViewModel(Pros);
            }
            BindingContext = vm;
            Looper.IsRunning = false;
            Looper.IsVisible = false;
        }

        private async void btnOpenProcess_Clicked(object sender, EventArgs e)
        {
            if (vm.SelectedItem != null)
            {
                if (vm.SelectedItem.Id == 0)
                {
                    //create new
                    await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage(Place.PlaceId));
                }
                else
                {
                    Process process = Pros.Where(p => p.ProcessId == vm.SelectedItem.Id).FirstOrDefault();
                    await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage(Place.PlaceId, process));
                }
            }
            else
            {
                await DisplayAlert("Nie zaznaczono elementu", "Najpierw zaznacz nowy lub istniejący element listy!", "OK");
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

        protected override void OnAppearing()
        {
            if (Place.PlaceId != 0)
            {
                UpdateList();
            }
        }
    }
}