using Microsoft.AppCenter.Crashes;
using Rg.Plugins.Popup.Services;
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

        private async void UpdateList()
        {
            PopupNavigation.Instance.PushAsync(new LoadingScreen(), true); // Show loading screen
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
                Crashes.TrackError(ex);
            }
            finally
            {

            }
            vm.SelectedItem = null;
            if (PopupNavigation.Instance.PopupStack.Any()) { PopupNavigation.Instance.PopAllAsync(true); }
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

        private async void lstProcesses_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            bool goOn = true;  

            if (vm.SelectedItem != null)
            {
                if (vm.SelectedItem.Id == 0)
                {
                    //create new
                    await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage(Place.PlaceId));
                }
                else
                {
                    if(Pros.Count == 0)
                    {
                        try
                        {
                            
                            PopupNavigation.Instance.PushAsync(new LoadingScreen(), true); // Show loading screen
                            Pros = await Place.GetProcesses(true);
                        }
                        catch (Exception ex)
                        {
                            goOn = false;
                            await DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
                        }
                        if (PopupNavigation.Instance.PopupStack.Any()) { PopupNavigation.Instance.PopAllAsync(true); }
                    }
                    if(goOn)
                    {
                        Process process = Pros.Where(p => p.ProcessId == vm.SelectedItem.Id).FirstOrDefault();
                        if (process != null)
                        {
                            await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage(Place.PlaceId, process));
                        }
                    }                    
                }
            }
            else
            {
                await DisplayAlert("Nie zaznaczono elementu", "Najpierw zaznacz nowy lub istniejący element listy!", "OK");
            }
        }

        private void btnShowCompleted_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new CompletedProcessesForPlace(Place), true);
        }
    }
}