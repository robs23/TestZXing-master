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


        private async Task<string> Save()
        {
            //starting handling
            string _Res = string.Empty;
            _Res = await vm.Validate();

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
            return _Res;
        }

        private async Task End()
        {
            //finish handling
            string _Res = string.Empty;
            bool _ToClose = false;
            bool _toPause = false;
            bool _Continue = true;
            _Res = await vm.Validate(true);
            if (_Res == "OK" || _Res.Contains("Skippable"))
            {
                if (_Res == "ActionListViewModelSkippable")
                {
                    if (!await DisplayAlert("Niezaznaczone czynności", "Nie wszystkie wymagane czynności zostały zaznaczone. Czy na pewno chcesz zakończyć zgłoszenie?", "Zamknij", "Anuluj, chcę poprawić"))
                    {
                        _Continue = false;
                    }
                }
                if (_Continue)
                {
                    //User accepted that not all required actions have been checked and wants to end anyway
                    if (await vm.AreThereOpenHandlingsLeft() == "No")
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
            }
            else
            {

                await DisplayAlert("Uups.. Coś poszło nie tak..", _Res, "OK");
            }
        }

        public async Task Resurrect()
        {
            

            string _Res = await vm.Validate();
            if(_Res == "OK")
            {
                await vm._thisProcess.Resurrect();
                vm.IsProcessOpen = true;
                btnChangeState.SetBinding(Button.TextProperty, new Binding("NextState"));
                btnChangeState.SetBinding(Button.BackgroundColorProperty, new Binding("NextStateColor"));
                btnChangeState.SetBinding(Button.IsEnabledProperty, new Binding("IsOpen"));
                await Save();
            }
            else
            {
                await DisplayAlert("Uups.. Coś poszło nie tak..", _Res, "OK");
            }
        }

        private async void btnChangeState_Clicked(object sender, EventArgs e)
        {
            string _Res = string.Empty;
            if (vm._this.Status == "Rozpoczęty")
            {
                await End();
            }else if (vm._thisProcess.Status == "Zakończony")
            {
                await Resurrect();
            }
            else
            {
                await Save();
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
                DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
            }
        }

        private void Analyze()
        {
            Dictionary<string, string> xProps = new Dictionary<string, string>
                {
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

        private async void btnParts_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushAsync(new AssignedPartsPage(vm.AssignedPartsVm));
        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            await Save();
        }

        private async void btnChangeState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsEnabled")
            {
                if (btnChangeState.IsEnabled)
                {
                    await btnChangeState.ScaleTo(1.3, 150);
                    await btnChangeState.ScaleTo(1, 150);
                }
                else
                {
                    //allow resurrection in 10 seconds
                    //but only if the process was finished in last 7 days
                    if(vm._thisProcess.FinishedOn != null)
                    {
                        if(vm._thisProcess.FinishedOn >= DateTime.Now.AddDays(-7))
                        {
                            await Task.Run(async () =>
                            {
                                for (int i = 10; i >= 1; i--)
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        btnChangeState.Text = $"Wznów za {i}";
                                    });
                                    await Task.Delay(1000);
                                }
                            });

                            btnChangeState.Text = "Wznów zgłoszenie";
                            btnChangeState.BackgroundColor = Color.Green;
                            btnChangeState.IsEnabled = true;
                        }
                    }
                }
                
            }

        }
    }
}