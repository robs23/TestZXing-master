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
    public partial class DiaryPage : ContentPage
    {
        DiaryViewModel vm;

        public DiaryPage()
        {
            InitializeComponent();
            vm = new DiaryViewModel();
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            await PopupNavigation.Instance.PushAsync(new LoadingScreen(), true); // Show loading screen
            Handling handling = ((Handling)((ListView)sender).SelectedItem);

            try
            {
                Process p = await new ProcessKeeper().GetProcess(handling.ProcessId);
                if (PopupNavigation.Instance.PopupStack.Any()) { await PopupNavigation.Instance.PopAllAsync(true); }  // Hide loading screen
                await Navigation.PushAsync(new ProcessPage(handling.PlaceId, p));

            }
            catch (Exception ex)
            {
                if (PopupNavigation.Instance.PopupStack.Any()) { await PopupNavigation.Instance.PopAllAsync(true); }  // Hide loading screen
                await DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
            }

            ((ListView)sender).SelectedItem = null;
        }

        protected async override void OnAppearing()
        {
            try
            {
                await vm.Initialize();
                BindingContext = vm;
            }
            catch (Exception ex)
            {
                vm.IsWorking = false;
                DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
            }

        }
    }
}
