using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TestZXing.Interfaces;
using TestZXing.Models;
using TestZXing.Static;
using TestZXing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActionList : ContentPage
    {
        ActionListViewModel vm;

        public ActionList(int processId, int placeId)
        {
            InitializeComponent();
            vm = new ActionListViewModel(processId, placeId);
            BindingContext = vm;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                if (!vm.IsInitialized)
                {
                    vm.Initialize();
                }

                

            }catch(Exception ex)
            {
                DisplayAlert(RuntimeSettings.ConnectionErrorTitle, RuntimeSettings.ConnectionErrorText, "OK");
            }
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            if (((IActionKeeper)e.Item).IsChecked)
            {
                ((IActionKeeper)e.Item).IsChecked = false;
            }
            else
            {
                ((IActionKeeper)e.Item).IsChecked = true;
            }
                

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
