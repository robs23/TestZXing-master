using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingScreen : Rg.Plugins.Popup.Pages.PopupPage
    {
        LoadingScreenViewModel vm;

        public LoadingScreen(string Message = "Wczytywanie..")
        {
            InitializeComponent();
            CloseWhenBackgroundIsClicked = false;
            vm = new LoadingScreenViewModel(Message);
            BindingContext = vm;
            
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // Disable back button
        }
    }
}