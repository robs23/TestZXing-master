﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingScreen : Rg.Plugins.Popup.Pages.PopupPage
    {
        public LoadingScreen()
        {
            InitializeComponent();
            CloseWhenBackgroundIsClicked = false;
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // Disable back button
        }
    }
}