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
    public partial class AssignedProcesses : Rg.Plugins.Popup.Pages.PopupPage
    {
        public AssignedProcesses(AssignedProcessesViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        private async void LstProcesses_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            Process Proc = ((Process)((ListView)sender).SelectedItem);
            await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage(Proc.PlaceId, Proc));

            ((ListView)sender).SelectedItem = null;

            
        }
    }
}