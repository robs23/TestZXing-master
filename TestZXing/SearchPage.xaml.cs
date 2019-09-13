using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        List<string> colors = new List<string>
        {
            "green",
            "purple",
            "red",
            "blue",
            "orange"
        };

        public SearchPage()
        {
            InitializeComponent();
        }


        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = txtSearch.Text;

            var suggestions = colors.Where(c => c.ToLower().Contains(keyword.ToLower()));

            lstSuggestions.ItemsSource = suggestions;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            txtSearch.Focus();
        }
    }
}