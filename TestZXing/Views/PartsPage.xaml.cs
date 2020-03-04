using SQLite;
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
    public partial class PartsPage : ContentPage
    {
        //SQLiteConnection db = new SQLiteConnection(Static.RuntimeSettings.LocalDbPath);
        PartsPageViewModel vm;

        public PartsPage()
        {
            InitializeComponent();
            vm = new PartsPageViewModel();
            BindingContext = vm;
        }

        public PartsPage(PartsPageViewModel _vm)
        {
            InitializeComponent();
            vm = _vm;
            BindingContext = vm;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!vm.IsInitilized)
            {
                vm.Initialize();
            }
            
            txtSearch.Focus();
        }

        private void lstSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() != null)
            {
                vm.SelectedItems.Add((Part)e.CurrentSelection.FirstOrDefault());
                if(vm.Mode== PartsPageMode.PartsBrowser)
                {
                    //Open Part form
                }
                else
                {
                    Navigation.PopAsync();
                }
            }
            lstSuggestions.SelectedItem = null;
        }
    }
}