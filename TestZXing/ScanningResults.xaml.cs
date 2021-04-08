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
        bool IsQrConfirmed;

        public ScanningResults (Place nPlace, bool isQrConfirmed=false)
		{
			InitializeComponent ();
            //Pros = Proses;
            Place = nPlace;
            IsQrConfirmed = isQrConfirmed;
            Keeper = new PlacesKeeper();
            vm = new ProcessInPlaceViewModel(Place);
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
                vm = new ProcessInPlaceViewModel(Place);
                vm.Update(Pros);
                BindingContext = vm;
                vm.Initialize();
            }
            catch (Exception ex)
            {
                Static.Functions.CreateError(ex, "No connection", nameof(this.UpdateList), this.GetType().Name);
                await DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
            }
            finally
            {

            }
            vm.SelectedItem = null;
            if (PopupNavigation.Instance.PopupStack.Any()) { await PopupNavigation.Instance.PopAllAsync(true); }
        }

        protected override void OnAppearing()
        {
            vm.Initialize();
            vm.RefreshStatus();
            
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
                    await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage(Place.PlaceId, IsQrConfirmed));
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
                        if (PopupNavigation.Instance.PopupStack.Any()) { await PopupNavigation.Instance.PopAllAsync(true); }
                    }
                    if(goOn)
                    {
                        Process process = Pros.Where(p => p.ProcessId == vm.SelectedItem.Id).FirstOrDefault();
                        if (process != null)
                        {
                            await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage(Place.PlaceId, process, IsQrConfirmed));
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
            Application.Current.MainPage.Navigation.PushAsync(new CompletedProcessesForPlace(Place), true);
        }

        private void UserStatus_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushAsync(new DiaryPage());
        }

        private void btnSave_Clicked(object sender, EventArgs e)
        {

        }
    }
}