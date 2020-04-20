using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;
using TestZXing.ViewModels;
using TestZXing.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProcessesPage : TabbedPage
    {
        ProcessesPageViewModel vm;


        public ProcessesPage()
        {
            InitializeComponent();
            
            vm = new ProcessesPageViewModel();
            vm.UserProcesses = new ActiveProcessesViewModel(ActiveProcessesPageMode.UsersProcesses);
            vm.AllProcesses = new ActiveProcessesViewModel(ActiveProcessesPageMode.AllProcesses);
            vm.MaintenanceOnly = new ActiveProcessesViewModel(ActiveProcessesPageMode.MaintenanceOnly);
            Children.Add(new ActiveProcesses(vm.UserProcesses));
            Children.Add(new ActiveProcesses(vm.AllProcesses));
            Children.Add(new ActiveProcesses(vm.MaintenanceOnly));

            BindingContext = vm;
        }

        private void UserStatus_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushAsync(new DiaryPage());
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!vm.IsInitilized)
            {
                vm.Initialize();
            }
        }

        private async void TabbedPage_CurrentPageChanged(object sender, EventArgs e)
        {
            string _Res;

            if (this.CurrentPage.Title.ToLower() == "moje")
            {
                vm.ActiveVm = vm.UserProcesses;
                _Res = await vm.UserProcesses.ExecuteLoadDataCommand();

            }
            else if (this.CurrentPage.Title.ToLower() == "wszystkie")
            {
                vm.ActiveVm = vm.AllProcesses;
                _Res = await vm.AllProcesses.ExecuteLoadDataCommand();
            }
            else
            {
                vm.ActiveVm = vm.MaintenanceOnly;
                _Res = await vm.MaintenanceOnly.ExecuteLoadDataCommand();
            }
        }
    }
}