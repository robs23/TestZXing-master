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
    public partial class ActiveProcesses : ContentPage
    {
        private ActiveProcessesViewModel vm
        {
            get { return (ActiveProcessesViewModel)BindingContext; }
            set { BindingContext = value; }
        }

        public ActiveProcesses(ActiveProcessesViewModel model)
        {
            InitializeComponent();
            vm = model;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm.LoadDataCommand.Execute(null);
        }

        private void StateImage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Source"))
            {
                var image = sender as Image;
                image.Opacity = 0;
                image.FadeTo(1, 1000);
            }
        }

    }
}