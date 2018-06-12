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
    public partial class ProcessPage : ContentPage
    {
        ProcessPageViewModel vm;
        public ProcessPage()
        {
            //new one
            InitializeComponent();
            vm = new ProcessPageViewModel();
            BindingContext = vm;
        }

        public ProcessPage(Process Process)
        {
            //existing
            InitializeComponent();
            vm = new ProcessPageViewModel(Process);
            BindingContext = vm;
        }

        private async void btnEnd_Clicked(object sender, EventArgs e)
        {
            
        }
    }
}