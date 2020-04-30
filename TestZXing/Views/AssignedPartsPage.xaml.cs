using MvvmHelpers;
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
    public partial class AssignedPartsPage : ContentPage
    {
        AssignedPartsViewModel vm;

        public AssignedPartsPage(AssignedPartsViewModel _vm)
        {
            InitializeComponent();
            vm = _vm;
            BindingContext = vm;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!vm.IsInitilized)
            {
                vm.Initialize();
            }
            vm.Update();


        }

        private void lstSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                vm.SelectedItems.Clear();
                if (e.CurrentSelection != null)
                {
                    if(e.CurrentSelection.Count > 0)
                    {
                        vm.RemovableSelected = true;
                        foreach (var pu in e.CurrentSelection)
                        {
                            vm.SelectedItems.Add((PartUsage)pu);
                        }
                    }
                    else
                    {
                        vm.RemovableSelected = false;
                    }
                }
                
            }catch(Exception ex)
            {
            }
        }
    }
}