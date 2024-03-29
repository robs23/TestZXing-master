﻿using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestZXing.Classes;
using TestZXing.Interfaces;
using TestZXing.Models;
using TestZXing.Static;
using TestZXing.Views;
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class PartsPageViewModel : BaseViewModel
    {
        public PartKeeper Keeper { get; set; }

        

        public PartsPageViewModel()
        {
            Keeper = new PartKeeper();
            ItemTreshold = 5;
            PageSize = 15;
            ReloadCommand = new AsyncCommand(Reload);
            ScanCommand = new AsyncCommand(Scan);
            ItemTresholdReachedCommand = new AsyncCommand(ItemTresholdReached);
        }

        public async Task Initialize()
        {
            base.Initialize();
            Reload();
            SelectedItems = new ObservableRangeCollection<Part>();
        }

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        bool _IsWorking;
        public override bool IsWorking
        {
            get { return _IsWorking; }
            set
            {
                bool isChanged = SetProperty(ref _IsWorking, value);
                if (isChanged)
                {
                    OnPropertyChanged(nameof(EmptyViewCaption));
                }
            }
        }

        string _SearchQuery;
        public string SearchQuery
        {
            get
            {
                return _SearchQuery;
            }
            set
            {
                SetProperty(ref _SearchQuery, value);
            }
        }

        string _PrevSearchQuery;
        public string PrevSearchQuery
        {
            get { return _PrevSearchQuery; }
            set { SetProperty(ref _PrevSearchQuery, value); }
        }

        public string EmptyViewCaption
        {
            get
            {
                if (IsWorking)
                {
                    return "Trwa wczytywanie..";
                }
                else
                {
                    return "Brak danych spełniających kryteria.";
                }
            }
        }


        public ICommand ScanCommand { get; }

        public async Task Scan()
        {
            try
            {
                QrHandler qr = new QrHandler();
                string res = await qr.Scan();
                res = res.Replace("<Part>", "");
                Part Part = await Keeper.GetByToken(res);
                if (Part != null)
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new PartPage(Part));
                }
                else
                {
                    DependencyService.Get<IToaster>().ShortAlert($"Nie znaleziono części oznaczonej kodem {res}..");
                }
            }
            catch(Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(RuntimeSettings.ConnectionErrorTitle, RuntimeSettings.ConnectionErrorText, "OK");
            }
        }

        public ICommand ReloadCommand { get; }

        public async Task Reload()
        {
            try
            {
                IsWorking = true;
                string query = null;
                CurrentPage = 1;
                Items = new ObservableRangeCollection<Part>();

                query = GetQueryString();
                
                await Keeper.Reload(query, CurrentPage, PageSize);
                if (Keeper.Items.Count > 0)
                {
                    Items.AddRange(Keeper.Items);
                }
            }catch(Exception ex)
            {
                Static.Functions.CreateError(ex, ex.GetType().ToString(), nameof(this.Reload), this.GetType().Name);
                DependencyService.Get<IToaster>().ShortAlert($"Error: {ex.Message}");
            }
            IsWorking = false;
        }

        private string GetQueryString()
        {
            string query = null;
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                query = "";
                string[] keys = SearchQuery.Split(' ');
                for (int i = 0; i < keys.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(query))
                    {
                        query += " AND ";
                    }
                    query += $"(Name.ToLower().Contains(\"{keys[i].ToLower()}\") OR Symbol.ToLower().Contains(\"{keys[i].ToLower()}\") OR ProducerName.ToLower().Contains(\"{keys[i].ToLower()}\"))";
                }
            }
            return query;
        }

        public ICommand ItemTresholdReachedCommand { get; }
        public async Task ItemTresholdReached()
        {
            try
            {
                IsWorking = true;
                string query = null;
                CurrentPage++;

                query = GetQueryString();

                await Keeper.Reload(query, CurrentPage, PageSize);
                if (Keeper.Items.Count > 0)
                {
                    Items.AddRange(Keeper.Items);
                }
            }
            catch (Exception ex)
            {
                Static.Functions.CreateError(ex, ex.GetType().ToString(), nameof(this.Reload), this.GetType().Name);
                DependencyService.Get<IToaster>().ShortAlert($"Error: {ex.Message}");
            }
            IsWorking = false;

        }

        ObservableRangeCollection<Part> _Items;

        public ObservableRangeCollection<Part> Items
        {
            get
            {
                return _Items;
            }
            set
            {
                SetProperty(ref _Items, value);
            }
        }

        ObservableRangeCollection<Part> _SelectedItems;

        public ObservableRangeCollection<Part> SelectedItems
        {
            get
            {
                return _SelectedItems;
            }
            set
            {
                SetProperty(ref _SelectedItems, value);
            }
        }


        int _ItemTreshold;

        public int ItemTreshold
        {
            get
            {
                return _ItemTreshold;
            }
            set
            {
                SetProperty(ref _ItemTreshold, value);
            }
        }



        public PartsPageMode Mode { get; set; } = PartsPageMode.PartsBrowser;
    }

    public enum PartsPageMode
    {
        PartsBrowser,
        PartsPicker
    }
}
