using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;
using TestZXing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScanningResults : ContentPage
	{
        Place Place;
        PlacesKeeper Keeper;
        ProcessInPlaceViewModel vm;
        List<Process> Pros;

        public ScanningResults (List<Process>Proses, Place nPlace)
		{
			InitializeComponent ();
            Pros = Proses;
            Place = nPlace;
            Keeper = new PlacesKeeper();
            vm = new ProcessInPlaceViewModel(Pros);
            BindingContext = vm;
            lblScanResult.Text = Place.Name;
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

        private async void UpdateList()
        {
            Looper.IsVisible = true;
            Looper.IsRunning = true;
            Pros = new List<Process>();
            try
            {
                Pros = await Place.GetProcesses(true);
                vm = new ProcessInPlaceViewModel(Pros);
                BindingContext = vm;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
            }
            finally
            {

            }
            vm.SelectedItem = null;
            Looper.IsRunning = false;
            Looper.IsVisible = false;
        }

        protected override void OnAppearing()
        {
            if (Place != null)
            {
                if (Place.PlaceId != 0)
                {
                    UpdateList();
                }
            }
        }
    }
}