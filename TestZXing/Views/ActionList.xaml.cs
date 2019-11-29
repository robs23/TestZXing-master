using Rg.Plugins.Popup.Services;
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
    public partial class ActionList : Rg.Plugins.Popup.Pages.PopupPage
    {
        ActionListViewModel vm;

        public ActionList(ActionListViewModel _vm)
        {
            InitializeComponent();
            vm = _vm;
            BindingContext = vm;
        }

        //protected async override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    try
        //    {
        //        //if (!vm.IsInitialized)
        //        //{
        //        //    vm.Initialize();
        //        //}

                

        //    }catch(Exception ex)
        //    {
        //        DisplayAlert(RuntimeSettings.ConnectionErrorTitle, RuntimeSettings.ConnectionErrorText, "OK");
        //    }
        //}

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

        private void StateImage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Source"))
            {
                var image = sender as Image;
                image.Opacity = 0;
                image.FadeTo(1, 1000);
            }
        }

        private async void BtnOK_Clicked(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Any()) { await PopupNavigation.Instance.PopAllAsync(true); }  // Hide handlings screen
        }
    }
}
