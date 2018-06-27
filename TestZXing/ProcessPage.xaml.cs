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
    public partial class ProcessPage : ContentPage
    {
        ProcessPageViewModel vm;
        public ProcessPage(int PlaceId)
        {
            //new one
            InitializeComponent();
            vm = new ProcessPageViewModel(PlaceId);
            BindingContext = vm;
        }

        public ProcessPage(int PlaceId, Process Process)
        {
            //existing
            InitializeComponent();
            vm = new ProcessPageViewModel(PlaceId, Process);
            BindingContext = vm;
        }

        private async void btnEnd_Clicked(object sender, EventArgs e)
        {
            bool _IsSuccess = false;
            if (vm.Validate())
            {
                if (await DisplayAlert("Zrealizowano?", "Czy udało się zrealizować zlecenie?", "Tak", "Nie"))
                {
                    _IsSuccess = true;
                }
                string _Result = await vm.End(_IsSuccess);
                if (_Result=="OK")
                {
                    await DisplayAlert("Powodzenie", "Zgłoszenie zostało zakończone!", "OK");
                }
                else
                {
                    await DisplayAlert("Wystąpił błąd", _Result, "OK");
                }
            }
            else
            {
                await DisplayAlert("Brak typu zgłoszenia", "Nie wybrano typu zgłoszenia! Najpierw wybierz typ zgłoszenia z listy rozwijanej", "OK");
            }
        }

        private async void btnChangeState_Clicked(object sender, EventArgs e)
        {
            string _Result = await vm.Save();
            if (vm.Validate())
            {
                if (_Result == "OK")
                {
                    await DisplayAlert("Powodzenie", "Zapis zakończony powodzeniem!", "OK");
                }
                else
                {
                    await DisplayAlert("Wystąpił błąd", _Result, "OK");
                }
            }
            else
            {
                await DisplayAlert("Brak typu zgłoszenia", "Nie wybrano typu zgłoszenia! Najpierw wybierz typ zgłoszenia z listy rozwijanej", "OK");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}