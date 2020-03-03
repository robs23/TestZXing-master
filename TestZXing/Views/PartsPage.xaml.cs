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

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            vm.Reload();
            txtSearch.Focus();
        }
    }
}