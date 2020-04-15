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
    public partial class ProcessesPage : TabbedPage
    {
        ProcessesPageViewModel vm;


        public ProcessesPage()
        {
            InitializeComponent();
            
            vm = new ProcessesPageViewModel();
            vm.UserProcesses = new ActiveProcessesViewModel(true);
            vm.AllProcesses = new ActiveProcessesViewModel();
            Children.Add(new ActiveProcesses(vm.UserProcesses));
            Children.Add(new ActiveProcesses(vm.AllProcesses));
            BindingContext = vm;
        }

        private void UserStatus_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushAsync(new DiaryPage());
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void Filter_Clicked(object sender, EventArgs e)
        {

        }
    }
}