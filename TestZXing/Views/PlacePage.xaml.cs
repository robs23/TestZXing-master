using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;
using TestZXing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlacePage : ContentPage
    {
        PlacePageViewModel vm;
        bool IsShowing = false;

        public PlacePage(Place place)
        {
            InitializeComponent();
            vm = new PlacePageViewModel(place);
            BindingContext = vm;
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!IsShowing)
            {
                IsShowing = true;
                try
                {
                    if (!vm.IsInitialized)
                    {
                        //initialize only when not yet initialized
                        await vm.Initialize();

                        BindingContext = vm;
                    }
                    else
                    {
                        if (!vm.IsSaveable)
                        {
                            vm.IsSaveable = await vm.IsDirty();
                        }
                    }

                }
                catch (Exception ex)
                {
                    DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
                }
                IsShowing = false;
            }
        }


        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            await vm.Save();
        }
    }
}