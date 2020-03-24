using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;
using TestZXing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PartPage : ContentPage
    {
        PartPageViewModel vm;

        public PartPage(Part part)
        {
            InitializeComponent();
            vm = new PartPageViewModel(part);
            BindingContext = vm;
        }
    }
}