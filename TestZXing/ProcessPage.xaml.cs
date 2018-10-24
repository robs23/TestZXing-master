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
            //From MES
            InitializeComponent();
            vm = new ProcessPageViewModel(ms);
            BindingContext = vm;
        }

        private async void btnEnd_Clicked(object sender, EventArgs e)
        {
            bool _IsSuccess = false;
            string _Res = vm.Validate(true);
            if (_Res=="OK")
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
                    if (vm.IsMesRelated)
                    {
                        await vm.Initialize(vm._this.ActionTypeId, vm._this.PlaceId);
                    }
                    else
                    {
                        await vm.Initialize(vm._this.ActionTypeId);
                    }
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
    }
}