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
using TestZXing.Interfaces;
using TestZXing.Models;
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class PartsPageViewModel : ObservableObject
    {
        public PartKeeper Keeper { get; set; }

        

        public PartsPageViewModel()
        {
            Keeper = new PartKeeper();
            ItemTreshold = 5;
            PageSize = 15;
            ReloadCommand = new AsyncCommand<string>(Reload);
            ItemTresholdReachedCommand = new AsyncCommand(ItemTresholdReached);
        }

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public ICommand ReloadCommand { get; }
        public async Task Reload(string query = null)
        {
            CurrentPage = 1;
            Items = new ObservableRangeCollection<Part>();
            await Keeper.Reload(query, 1, PageSize);
            Items.AddRange(Keeper.Items);
        }

        public ICommand ItemTresholdReachedCommand { get; }
        public async Task ItemTresholdReached()
        {
            try
            {
                CurrentPage++;
                await Keeper.Reload(null, CurrentPage, PageSize);
                Items.AddRange(Keeper.Items);
            }catch(Exception ex)
            {
                //Show IsWorking here
                DependencyService.Get<IToaster>().LongAlert($"Error: {ex.Message}");
            }
            

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
                if (value != _Items)
                {
                    _Items = value;
                }
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
    }
}
