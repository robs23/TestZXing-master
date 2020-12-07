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
    public partial class ProcessAttachementsPage : ContentPage
    {
        ProcessAttachmentsViewModel vm = new ProcessAttachmentsViewModel();
        public ProcessAttachementsPage(ProcessAttachmentsViewModel _vm)
        {
            InitializeComponent();
            vm = _vm;
            BindingContext = vm;
        }

        private void lstAttachments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                vm.SelectedItems.Clear();
                if (e.CurrentSelection != null)
                {
                    if (e.CurrentSelection.Count > 0)
                    {
                        if (e.CurrentSelection.Count == 1)
                        {
                            foreach (var f in e.CurrentSelection)
                            {
                                vm.ActiveElementPath = ((File)f).Source;
                            }
                            
                        }
                        vm.RemovableSelected = true;
                        foreach (var f in e.CurrentSelection)
                        {
                            vm.SelectedItems.Add((File)f);
                        }
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

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!vm.IsInitilized)
            {
                vm.Initialize();
            }
            vm.Update();


        }
    }
}