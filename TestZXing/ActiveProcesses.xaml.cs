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
    public partial class ActiveProcesses : ContentPage
    {
        private ActiveProcessesViewModel vm
        {
            get { return (ActiveProcessesViewModel)BindingContext; }
            set { BindingContext = value; }
        }

        public ActiveProcesses(ActiveProcessesViewModel model)
        {
            InitializeComponent();
            vm = model;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            string _Result;

            try
            {
                _Result = await vm.ExecuteLoadDataCommand();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Brak połączenia", "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz", "OK");
                Navigation.PopAsync();
            }
            
        }

        private void StateImage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Source"))
            {
                var image = sender as Image;
                image.Opacity = 0;
                image.FadeTo(1, 1000);
            }
        }

        private async void btnOpenProcess_Clicked(object sender, EventArgs e)
        {
            if(vm.SelectedItem != null)
            {
                Process process = vm.SelectedItem._process;
                await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage(process.PlaceId, process));
            }
            else
            {
                await DisplayAlert("Nie zaznaczono elementu", "Najpierw zaznacz zgłoszenie, które chcesz wyświetlić!", "OK");
            }
        }
    }
}