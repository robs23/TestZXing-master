using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssignedPartsPage : ContentPage
    {
        AssignedPartsViewModel vm;
        public AssignedPartsPage()
        {
            InitializeComponent();
            vm = new AssignedPartsViewModel();
            BindingContext = vm;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            vm.Initialize();
        }
    }
}