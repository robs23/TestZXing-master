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
        User vm;

        public ProcessesPage()
        {
            InitializeComponent();
            Children.Add(new ActiveProcesses(new ActiveProcessesViewModel(true)));
            Children.Add(new ActiveProcesses(new ActiveProcessesViewModel()));
            vm = Static.RuntimeSettings.CurrentUser;
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
    }
}