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
    public partial class UploadQueue : ContentPage
    {
        UploadQueueViewModel vm;
        public UploadQueue()
        {
            InitializeComponent();
            vm = new UploadQueueViewModel();
            BindingContext = vm;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!vm.IsInitilized)
            {
                vm.Initialize();
            }
        }

        private void lstFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                vm.SelectedItems.Clear();
                if (e.CurrentSelection != null)
                {
                    if (e.CurrentSelection.Count > 0)
                    {
  
                        foreach (var f in e.CurrentSelection)
                        {
                            vm.SelectedItems.Add((File)f);
                        }
                        vm.RemovableSelected = true;

                    }
                    else
                    {
                        vm.RemovableSelected = false;
                    }
                }

            }
            catch (Exception ex)
            {
            }
        }
    }
}