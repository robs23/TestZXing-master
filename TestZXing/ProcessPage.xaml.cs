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

        public ProcessPage(MesString ms)
        {
            //From MES, new one
            InitializeComponent();
            vm = new ProcessPageViewModel(ms);
            BindingContext = vm;
        }

        public ProcessPage(MesString ms, Process Process)
        {
            //From MES, existing
            InitializeComponent();
            vm = new ProcessPageViewModel(ms, Process);
            BindingContext = vm;
        }




        private async void btnEnd_Clicked(object sender, EventArgs e)
        {
            bool _ToClose = false;
            bool _toPause = false;
            string _Res = vm.Validate(true);
            if (_Res=="OK")
            {
                if(await vm.AreThereOpenHandlingsLeft() == "No")
                {
                    //prompt user if to close the process
                    if (await DisplayAlert("Zamknąć zgłoszenie?", "Jesteś ostatnią osobą obsługującą to zgłoszenie. Możesz pozostawić to zgłoszenie otwarte lub zamknąć je. Co mam zrobić?", "Zamknij", "Pozostaw"))
                    {
                        _ToClose = true;
                    }
                    else
                    {
                        _toPause = true;
                    }
                }
                string _Result = await vm.End(_ToClose, _toPause);
                if (_Result == "OK")
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

                await DisplayAlert("Dane niepełne", _Res , "OK");
            }
        }

        private async void btnChangeState_Clicked(object sender, EventArgs e)
        {
            string _Res = vm.Validate();
            if (_Res == "OK")
            {
                string _Result = await vm.Save();
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
                await DisplayAlert("Dane niepełne", _Res, "OK");
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                if (vm.IsNew)
                {
                    await vm.Initialize();
                }
                else
                {

                    await vm.Initialize(vm._thisProcess.ActionTypeId);
                }
                BindingContext = vm;

            }
            catch (Exception ex)
            {
                btnChangeState.IsEnabled = false;
                btnEnd.IsEnabled = false;
                DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
            }
        }

        private async void btnCloseProcess_Clicked(object sender, EventArgs e)
        {
            //allows user to completely close order and all outstanding handlings
            string _Result = "OK";

            if (await vm.AreThereOpenHandlingsLeft() != "No")
            {
                if (!await DisplayAlert("Potwierdź", "Czy na pewno chcesz zamknąć to zgłoszenie? Jeśli tak, wszystkie obsługi tego zgłoszenia zostaną zakończone.", "Zamknij", "Pozostaw"))
                {
                    _Result = "No";
                }
            }
            if (_Result == "OK")
            {
                _Result = await vm.End(true, false);
                if (_Result == "OK")
                {
                    await DisplayAlert("Powodzenie", "Zgłoszenie zostało zakończone!", "OK");
                }
                else
                {
                    await DisplayAlert("Wystąpił błąd", _Result, "OK");
                }
            }
        }
    }
}