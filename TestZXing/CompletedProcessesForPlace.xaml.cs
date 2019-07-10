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
    public partial class CompletedProcessesForPlace : Rg.Plugins.Popup.Pages.PopupPage
    {
        CompletedProcessesInPlaceViewModel vm;

        public CompletedProcessesForPlace(Place place)
        {
            InitializeComponent();
            vm = new CompletedProcessesInPlaceViewModel(place);
            BindingContext = vm;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await vm.Initialize();
        }

        private void btnClose_Clicked(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Any()) { PopupNavigation.Instance.PopAllAsync(true); }  // Hide this screen
        }

        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return base.OnBackButtonPressed();
        }

        private async void lstProcesses_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            Process process = ((Process)((ListView)sender).SelectedItem);
            if (PopupNavigation.Instance.PopupStack.Any()) { PopupNavigation.Instance.PopAllAsync(true); } // Hide this screen
            await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage(vm.Place.PlaceId, process));
        }
    }
}