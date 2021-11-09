using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Static;
using TestZXing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HandlingsPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        HandlingsPageViewModel vm;
        bool IsShowing = false;

        public HandlingsPage(int processId)
        {
            InitializeComponent();
            vm = new HandlingsPageViewModel(processId);
            BindingContext = vm;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!IsShowing)
            {
                IsShowing = true;
                try
                {
                    await vm.Initialize();
                }
                catch (Exception)
                {
                    DisplayAlert(RuntimeSettings.ConnectionErrorTitle, RuntimeSettings.ConnectionErrorText, "OK");
                }
                IsShowing = false;
            }
        }

        private async void btnClose_Clicked(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Any()) { await PopupNavigation.Instance.PopAllAsync(true); }  // Hide handlings screen
        }

        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return base.OnBackButtonPressed();
        }
    }
}