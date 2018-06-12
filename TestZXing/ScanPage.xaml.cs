using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;
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
            btnOpenProcess.IsVisible = false;
            Keeper = new PlacesKeeper();
            Place = new Place();
        }

        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            scanPage = new ZXingScannerPage();
            
            scanPage.OnScanResult += (result) =>
            {
                scanPage.IsScanning = false;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    Navigation.PopAsync();
                    Place = await Keeper.GetPlace(result.Text);
                    lblScanResult.Text = "Zeskanowano: " + Place.Name;
                    Pros = await Place.GetProcesses(true);
                    vm = new ProcessInPlaceViewModel(Pros);
                    BindingContext = vm;
                    lblScanResult.IsVisible = true;
                    lblGetOrder.IsVisible = true;
                    btnOpenProcess.IsVisible = true;
                });
            };
            await Navigation.PushAsync(scanPage);
        }

        private async void btnOpenProcess_Clicked(object sender, EventArgs e)
        {
            if (vm.SelectedItem.Id == 0)
            {
                //create new
                await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage());
            }
            else
            {
                Process process = Pros.Where(p => p.ProcessId == vm.SelectedItem.Id).FirstOrDefault();
                await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage(process));
            }
        }
    }
}