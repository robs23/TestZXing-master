﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;
using System.Linq;
using Xamarin.Forms;
using TestZXing.Interfaces;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using TestZXing.Views;

namespace TestZXing.ViewModels
{
    public class ProcessesPageViewModel : BaseViewModel
    {
        public ActiveProcessesViewModel UserProcesses { get; set; }
        public ActiveProcessesViewModel AllProcesses { get; set; }
        public ProcessesFilterViewModel Filter { get; set; }
        public ProcessesPageViewModel()
        {
            OpenFilterPageCommand = new Command(
                execute: () =>
                {
                    if (IsFilterSetUp)
                    {
                        PopupNavigation.Instance.PushAsync(new ProcessesFilter(Filter));
                    }
                    else
                    {
                        DependencyService.Get<IToaster>().ShortAlert("Filtr nie jest jeszcze gotowy. Spróbuj za chwilę..");
                    }
                    
                });
            Filter = new ProcessesFilterViewModel();
        }

        public ActiveProcessesViewModel ActiveVm { get; set; }

        public ICommand OpenFilterPageCommand { get; private set; }

        public async Task Initialize()
        {
            base.Initialize();
            Task.Run(() => SetUpFilter());
        }

        public async Task OnFilterUpdate()
        {
            //called when FilterString of Filter has been updated
            OnPropertyChanged(nameof(FilterIcon));
            AllProcesses.FilterString = Filter.FilterString;
            UserProcesses.FilterString = Filter.FilterString;
            await ActiveVm.ExecuteLoadDataCommand();
        }

        public async Task SetUpFilter()
        {
            ProcessKeeper pk = new ProcessKeeper();
            await pk.Reload("IsCompleted = false and IsSuccessfull = false");

            foreach(Process p in pk.Items)
            {
                if (!Filter.ActionTypes.Any(i => i.ActionTypeId == p.ActionTypeId))
                {
                    Filter.ActionTypes.Add(new ActionType() { ActionTypeId = p.ActionTypeId, Name = p.ActionTypeName });
                }
                if (!Filter.Places.Any(i => i.PlaceId == p.PlaceId))
                {
                    Filter.Places.Add(new Place() { PlaceId = p.PlaceId, Name = p.PlaceName });
                }
                if (!Filter.Areas.Any(i => i.AreaId == p.AreaId))
                {
                    Filter.Areas.Add(new Area() { AreaId = (int)p.AreaId, Name = p.AreaName });
                }
            }
            Filter.SetCaller(this);
            IsFilterSetUp = true;
        }

        bool _IsFilterSetUp = false;
        public bool IsFilterSetUp
        {
            get { return _IsFilterSetUp; }
            set { SetProperty(ref _IsFilterSetUp, value); }
        }


        public string Icon
        {
            get
            {
                return Static.RuntimeSettings.CurrentUser.Icon;
            }
        }

        public string FilterIcon
        {
            get
            {
                if (Filter.IsSet)
                {
                    return "filter_full.png";
                }
                else
                {
                    return "filter_empty.png";
                }
            }
        }
    }
}
