﻿using System;
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
                //vm.SelectedItems.Clear();
                //if (e.CurrentSelection != null)
                //{
                //    if (e.CurrentSelection.Count > 0)
                //    {
                //        if (e.CurrentSelection.Count == 1)
                //        {
                //            foreach (var f in e.CurrentSelection)
                //            {
                //                vm.ActiveElementPath = ((File)f).Link;
                //            }
                //            vm.IsElementActive = true;
                //        }
                //        else
                //        {
                //            foreach (var f in e.CurrentSelection)
                //            {
                //                vm.SelectedItems.Add((File)f);
                //            }
                //            vm.IsElementActive = false;
                //        }
                //        vm.RemovableSelected = true;
                        
                //    }
                //    else
                //    {
                //        vm.IsElementActive = false;
                //        vm.RemovableSelected = false;
                //    }
                //}

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

        private async void btnPick_Clicked(object sender, EventArgs e)
        {
            string res = await DisplayActionSheet("Co chcesz wybrać?", "Anuluj", null, "Zdjęcie", "Wideo");
            if (res == "Zdjęcie")
            {
                await vm.PickPhoto();
            }
            else if(res=="Wideo")
            {
                await vm.PickVideo();
            }
        }

        private async void btnCapture_Clicked(object sender, EventArgs e)
        {
            string res = await DisplayActionSheet("Co chcesz nagrać?", "Anuluj", null, "Zdjęcie", "Wideo");
            if (res == "Zdjęcie")
            {
                await vm.CapturePhoto();
            }
            else if (res == "Wideo")
            {
                await vm.CaptureVideo();
            }
        }

        //private void Switch_Toggled(object sender, ToggledEventArgs e)
        //{
        //    if (e.Value)
        //    {

        //    }
        //    else
        //    {
        //        //disabled
        //        var tapGestureRecognizer = new TapGestureRecognizer();
        //        tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, "TapCommand");
                
                
        //    }
        //}


    }
}