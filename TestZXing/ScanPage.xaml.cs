using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;
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
            lblScanResult.IsVisible = false;
            lblGetOrder.IsVisible = false;
            btnOrders.IsVisible = false;
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
                    lblScanResult.IsVisible = true;
                    lblGetOrder.IsVisible = true;
                    btnOrders.IsVisible = true;
                });
            };
            await Navigation.PushAsync(scanPage);
        }

        private void btnOrders_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("Uuups", "Teraz powinna wyświetlić się lista otwartych operacji dla tego zasobu..ale póki co nie działa!", "OK");
        }
    }
}