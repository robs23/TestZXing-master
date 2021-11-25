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
    public partial class SyncQueue : ContentPage
    {
        SyncQueueViewModel vm;
        bool IsShowing = false;
        public SyncQueue()
        {
            InitializeComponent();
            vm = new SyncQueueViewModel();
            BindingContext = vm;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!IsShowing)
            {
                IsShowing = true;
                if (!vm.IsInitialized)
                {
                    vm.Initialize();
                }
                IsShowing = false;
            }
        }

        private void lstFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                vm.SelectedItems.Clear();
                if (e.CurrentSelection != null)
                {
                    if (e.CurrentSelection.Count > 0)
                    {

                        foreach (var f in e.CurrentSelection)
                        {
                            vm.SelectedItems.Add((UserLog)f);
                        }
                        vm.RemovableSelected = true;

                    }
                    else
                    {
                        vm.RemovableSelected = false;
                    }
                }

            }
            catch (Exception ex)
            {
            }
        }

        private async void btnRemoveAll_Clicked(object sender, EventArgs e)
        {
            var res = await DisplayAlert("Potwierdź usunięcie", "Czy jesteś pewny, że chcesz usunąć wszystkie pozycje przeznaczone do synchronizacji? Usunięcie spowoduje, że wszystkie zmiany, które wprowadziłeś i nie zostały jeszcze zsynchronizowane, zostaną bezpowrotnie usunięte!", "Usuń pozycje", "Anuluj");
            if (res == true)
            {
                string _Result = await vm.RemoveAll();
                if (_Result != "OK")
                {
                    await DisplayAlert("Napotkano błędy", _Result, "OK");
                }
            }
        }

        private async void btnRemoveSelected_Clicked(object sender, EventArgs e)
        {
            var res = await DisplayAlert("Potwierdź usunięcie", "Czy jesteś pewny, że chcesz usunąć zaznaczone pliki przeznaczone do synchronizacji? Usunięcie spowoduje, że pliki te zostaną również usunięte z odpowiednich zgłoszeń/maszyn/części", "Usuń pliki", "Anuluj");
            if (res == true)
            {
                string _Result = await vm.RemoveSelected();
                if (_Result != "OK")
                {
                    await DisplayAlert("Napotkano błędy", _Result, "OK");
                }
            }
        }
    }
}