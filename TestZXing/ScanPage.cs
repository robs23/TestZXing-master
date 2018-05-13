﻿using System;
using TestZXing.Models;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace TestZXing
{
    public class ScanPage : ContentPage
	{
		ZXingScannerPage scanPage;
		Button buttonScanDefaultOverlay;

		public ScanPage() : base()
		{
			buttonScanDefaultOverlay = new Button
			{
				Text = "Skanuj",
				AutomationId = "scanWithDefaultOverlay",
			};

            
            buttonScanDefaultOverlay.Clicked += async (sender, e) =>
            {
                
                await DisplayAlert("Wynik skanowania", "blabla", "OK");
            };

			var stack = new StackLayout();
			stack.Children.Add(buttonScanDefaultOverlay);


			Content = stack;
		}
	}
}

