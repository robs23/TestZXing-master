﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Classes;
using TestZXing.Models;
using TestZXing.Static;
using TestZXing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        DataService ds;
        UsersKeeper keeper;
        LoginViewModel vm;

        public LoginPage()
        {
            InitializeComponent();
            ds = new DataService();
            keeper = new UsersKeeper();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                Looper.IsVisible = true;
                Looper.IsRunning = true;
                await keeper.Reload();
                if (keeper.Items.Any())
                {
                    vm = new LoginViewModel(keeper.Items);
                    BindingContext = vm;
                    //foreach (User user in keeper.Users)
                    //{
                    //    pick.Items.Add(user.FullName);
                    //}
                }
                else
                {
                    await DisplayAlert("Brak użytkowników", "Brak użytkowników na liście!", "OK");
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                await DisplayAlert("Błąd", error, "OK");
            }
            finally
            {
                Looper.IsVisible = false;
                Looper.IsRunning = false;
            }


        }

        private async void btnLogin_Clicked(object sender, EventArgs e)
        {
            if (cmbUsernames == null || String.IsNullOrEmpty(txtPassword.Text) == true)
            {
                await DisplayAlert("Podaj dane", "Nie wybrano użytkownika z listy rozwijanej lub nie podano hasła!", "OK");
            }
            else
            {
                if (vm.SelectedUser.Password == txtPassword.Text)
                {
                    //password matches, let user in
                    RuntimeSettings.UserId = vm.SelectedUser.UserId;
                    RuntimeSettings.TenantId = vm.SelectedUser.TenantId;
                    vm.SelectedUser.Login();
                    await Application.Current.MainPage.Navigation.PushAsync(new ScanPage());
                }
                else
                {
                    await DisplayAlert("Błędne dane", "Podano błędne hasło!", "OK");
                }
            }
            
        }
    }
}