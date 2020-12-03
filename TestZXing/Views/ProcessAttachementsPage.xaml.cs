using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProcessAttachementsPage : ContentPage
    {
        public ProcessAttachementsPage()
        {
            InitializeComponent();
        }

        private void lstSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void lstAttachments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}