using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TestZXing.Models;
using TestZXing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LastPlaces : ContentPage
    {
        LastPlacesViewModel vm;
        public LastPlaces()
        {
            InitializeComponent();
            vm = new LastPlacesViewModel();
        }

        protected async override void OnAppearing()
        {
            await vm.Initialize();
            BindingContext = vm;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            await PopupNavigation.Instance.PushAsync(new LoadingScreen(), true); // Show loading screen
            Place Place = ((Place)((ListView)sender).SelectedItem);
            List<Process> Pros = new List<Process>();
            try
            {
                Pros = await Place.GetProcesses(true);
                PopupNavigation.Instance.PopAllAsync(true); // Hide loading screen
                await Navigation.PushAsync(new ScanningResults(Pros, Place));

            }
            catch (Exception ex)
            {
                PopupNavigation.Instance.PopAllAsync(true); // Hide loading screen
                await DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
            }

            ((ListView)sender).SelectedItem = null;
        }
    }
}
