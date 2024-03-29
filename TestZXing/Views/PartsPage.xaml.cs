﻿using SQLite;
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
    public partial class PartsPage : ContentPage
    {
        //SQLiteConnection db = new SQLiteConnection(Static.RuntimeSettings.LocalDbPath);
        PartsPageViewModel vm;
        bool IsShowing = false;

        public PartsPage()
        {
            InitializeComponent();
            vm = new PartsPageViewModel();
            BindingContext = vm;
        }

        public PartsPage(PartsPageViewModel _vm)
        {
            InitializeComponent();
            vm = _vm;
            BindingContext = vm;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!IsShowing)
            {
                IsShowing = true;
                if (!vm.IsInitialized)
                {
                    vm.Initialize();
                    txtSearch.Focus();
                }
                IsShowing = false;
            }
            
            
        }

        private void lstSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() != null)
            {
                
                if(vm.Mode== PartsPageMode.PartsBrowser)
                {
                    //Open Part form
                    Navigation.PushAsync(new PartPage((Part)e.CurrentSelection.FirstOrDefault()));
                }
                else
                {
                    vm.SelectedItems.Add((Part)e.CurrentSelection.FirstOrDefault());
                    Navigation.PopAsync();
                }
            }
            lstSuggestions.SelectedItem = null;
        }
    }
}