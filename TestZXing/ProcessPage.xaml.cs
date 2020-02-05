using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;
using TestZXing.ViewModels;
using TestZXing.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProcessPage : ContentPage
    {
        ProcessPageViewModel vm;
        public ProcessPage(int PlaceId, bool IsQrConfirmed=false)
        {
            //new one
            InitializeComponent();
            vm = new ProcessPageViewModel(PlaceId, IsQrConfirmed);
            BindingContext = vm;
        }

        public ProcessPage(int PlaceId, Models.Process Process, bool IsQrConfirmed=false)
        {
            //existing
            InitializeComponent();
            vm = new ProcessPageViewModel(PlaceId, Process, IsQrConfirmed);
            BindingContext = vm;
        }

        public ProcessPage(MesString ms)
        {
            //From MES, new one
            InitializeComponent();
            vm = new ProcessPageViewModel(ms);
            BindingContext = vm;
        }

        public ProcessPage(MesString ms, Models.Process Process, bool IsQrConfirmed=false)
        {
            //From MES, existing
            InitializeComponent();
            vm = new ProcessPageViewModel(ms, Process, IsQrConfirmed);
            BindingContext = vm;
        }




        private async void btnEnd_Clicked(object sender, EventArgs e)
        {
            bool _ToClose = false;
            bool _toPause = false;
            string _Res = await vm.Validate(true);
            if (_Res=="OK")
            {
                if(true)//await vm.AreThereOpenHandlingsLeft() == "No")
                {
                    //if (vm.IsMesRelated)
                    //{
                        //prompt user if to close the process
                        if (await DisplayAlert("Zamknąć zgłoszenie?", "Jesteś ostatnią osobą obsługującą to zgłoszenie. Możesz pozostawić to zgłoszenie otwarte lub zamknąć je. Co mam zrobić?", "Zamknij", "Pozostaw"))
                        {
                            _ToClose = true;
                        }
                        else
                        {
                            _toPause = true;
                        }
                    //}
                    //else
                    //{
                    //    _ToClose = true;
                    //}
                }
                string _Result = await vm.End(_ToClose, _toPause);
                if (_Result == "OK")
                {
                    await DisplayAlert("Powodzenie", "Obsługa zgłoszenia została zakończona!", "OK");
                }
                else
                {
                    await DisplayAlert("Wystąpił błąd", _Result, "OK");
                }

            }
            else
            {

                await DisplayAlert("Uups.. Coś poszło nie tak..", _Res , "OK");
            }
        }

        private async void btnChangeState_Clicked(object sender, EventArgs e)
        {
            string _Res = await vm.Validate();

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
                await DisplayAlert("Uups.. Coś poszło nie tak..", _Res, "OK");
            }

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                if (!vm.IsInitialized)
                {
                    Debug.WriteLine("Not initalized");
                    //initialize only when not yet initialized
                    if (vm.IsNew && vm._thisProcess.Status != "Planowany" && vm._thisProcess.Status != "Zakończony")
                    {
                        await vm.Initialize();
                    }
                    else
                    {

                        await vm.Initialize(vm._thisProcess.ActionTypeId);
                    }
                    BindingContext = vm;
                }
                
            }
            catch (Exception ex)
            {
                btnChangeState.IsEnabled = false;
                btnEnd.IsEnabled = false;
                DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
            }
        }

        private void Analyze()
        {
            Dictionary<string, string> xProps = new Dictionary<string, string>
                {
                    {"Przycisk ZAKOŃCZ aktywny", btnEnd.IsEnabled.ToString() },
                    {"Status zgłoszenia", vm._thisProcess.Status },
                    {"Status obsługi", vm._this.Status }
                };
            if (vm.IsMesRelated)
            {
                xProps.Add("MES ID", vm.MesString.MesId);
            }
            Anal nAnal = new Anal("Wyświetlenie ProcessPage", xProps);
        }

        private async void btnCloseProcess_Clicked(object sender, EventArgs e)
        {
            //allows user to completely close order and all outstanding handlings
            string _Result = "OK";

            if (!await DisplayAlert("Potwierdź", "Czy na pewno chcesz zamknąć to zgłoszenie? Jeśli tak, wszystkie obsługi tego zgłoszenia zostaną zakończone.", "Zamknij", "Anuluj"))
            {
                _Result = "No";
            }

            if (_Result == "OK")
            {
                vm.RepairActions = $"Przymusowe zakończenie przez {Static.RuntimeSettings.CurrentUser.FullName}";
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


        private void btnShowHandlings_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new HandlingsPage(vm._thisProcess.ProcessId), true);
        }

        private void UserStatus_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushAsync(new DiaryPage());
        }

        private void BtnShowActions_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new ActionList(vm.ActionListVm), true);
        }

        private void BtnActions_Clicked(object sender, EventArgs e)
        {
            
            PopupNavigation.Instance.PushAsync(new ActionList(vm.ActionListVm), true);
        }

        private async void BtnActions_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsVisible")
            {
                if (vm.ActionsApplicable)
                {
                    await btnActions.ScaleTo(1.3, 150);
                    await btnActions.ScaleTo(1, 150);
                }
            }
        }
    }
}