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
        public ProcessPage(int PlaceId)
        {
            //new one
            InitializeComponent();
            vm = new ProcessPageViewModel(PlaceId);
            BindingContext = vm;
        }

        public ProcessPage(int PlaceId, Process Process)
        {
            //existing
            InitializeComponent();
            vm = new ProcessPageViewModel(PlaceId, Process);
            BindingContext = vm;
        }

        private async void btnEnd_Clicked(object sender, EventArgs e)
        {

        }

        private async void btnChangeState_Clicked(object sender, EventArgs e)
        {
            await vm.Save();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}