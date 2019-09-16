using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        SQLiteConnection db = new SQLiteConnection(Static.RuntimeSettings.LocalDbPath);

        public SearchPage()
        {
            InitializeComponent();
        }


        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = txtSearch.Text.ToLower();

            var suggestions = db.Table<Part>().Where(v => v.Name.ToLower().Contains(keyword) || v.Symbol.ToLower().Contains(keyword)).Select(v=>v.Name);

            lstSuggestions.ItemsSource = suggestions;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            txtSearch.Focus();
        }
    }
}