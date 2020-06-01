using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestZXing.Models;
using Xamarin.Forms;
using AsyncCommand = MvvmHelpers.Commands.AsyncCommand;

namespace TestZXing.ViewModels
{
    public class ProcessesFilterViewModel : BaseViewModel
    {
        public ProcessesPageViewModel CallerVm;
        public ProcessesFilterViewModel()
        {
            ActionTypes = new ObservableCollection<ActionType>();
            Places = new ObservableCollection<Place>();
            Areas = new ObservableCollection<Area>();
            TimeVsPlanStatuses = new ObservableCollection<string>();

            ClearActionTypeCommand = new Command(
                execute: () =>
                {
                    SelectedActionType = null;
                },
                canExecute: () =>
                {
                    return !IsWorking;
                });
            ClearPlaceCommand = new Command(
                execute: () =>
                {
                    SelectedPlace = null;
                },
                canExecute: () =>
                {
                    return !IsWorking;
                });
            ClearAreaCommand = new Command(
                execute: () =>
                {
                    SelectedArea = null;
                },
                canExecute: () =>
                {
                    return !IsWorking;
                });
            ClearSetCommand = new Command(
                execute: () =>
                {
                    SelectedSet = null;
                },
                canExecute: () =>
                {
                    return !IsWorking;
                });
            ClearTimeVsPlanCommand = new Command(
                execute: () =>
                {
                    SelectedTimeVsPlanStatus = null;
                },
                canExecute: () =>
                {
                    return !IsWorking;
                });
            ClearAllCommand = new AsyncCommand(ClearAll);
            SetFilterCommand = new AsyncCommand(SetFilter);
        }

        public string FilterString
        {
            get
            {
                string res = null;
                if (SelectedActionType != null || SelectedArea != null || SelectedPlace != null || SelectedSet !=null || SelectedTimeVsPlanStatus!=null)
                {
                    //There is at least 1 condition in the filter
                    res = "";
                    if (SelectedActionType != null)
                    {
                        res += $"ActionTypeId={SelectedActionType.ActionTypeId}";
                    }
                    if (SelectedArea != null)
                    {
                        if (!string.IsNullOrEmpty(res)) { res += " and "; }
                        res += $"AreaId={SelectedArea.AreaId}";
                    }
                    if (SelectedSet != null)
                    {
                        if (!string.IsNullOrEmpty(res)) { res += " and "; }
                        res += $"SetId={SelectedSet.SetId}";
                    }
                    if (SelectedPlace != null)
                    {
                        if (!string.IsNullOrEmpty(res)) { res += " and "; }
                        res += $"PlaceId={SelectedPlace.PlaceId}";
                    }
                    if (SelectedTimeVsPlanStatus != null)
                    {
                        if (!string.IsNullOrEmpty(res)) { res += " AND "; }
                        res += $"TimingVsPlan.ToLower().Contains(\"{SelectedTimeVsPlanStatus.ToLower()}\")";
                    }
                }
                return res;
            }
        }

        public void SetCaller(ProcessesPageViewModel _CallerVm)
        {
            CallerVm = _CallerVm;
        }

        public async Task ClearAll()
        {
            SelectedActionType = null;
            SelectedPlace = null;
            SelectedArea = null;
            SelectedSet = null;
            SelectedTimeVsPlanStatus = null;
            IsSet = false;
            if (PopupNavigation.Instance.PopupStack.Count > 0) { await PopupNavigation.Instance.PopAllAsync(true); }  // Hide handlings screen
            CallerVm.OnFilterUpdate();
            
        }

        public async Task SetFilter()
        {
            IsSet = !string.IsNullOrEmpty(FilterString);
            if (PopupNavigation.Instance.PopupStack.Count > 0) { await PopupNavigation.Instance.PopAllAsync(true); }  // Hide handlings screen
            CallerVm.OnFilterUpdate();
            
        }

        bool _IsSet=false;
        public bool IsSet
        {
            get { return _IsSet; }
            set { SetProperty(ref _IsSet, value); }
        }

        ObservableCollection<ActionType> _ActionTypes = new ObservableCollection<ActionType>();
        public ObservableCollection<ActionType> ActionTypes
        {
            get { return _ActionTypes; }
            set { SetProperty(ref _ActionTypes, value); }
        }

        ObservableCollection<Place> _Places = new ObservableCollection<Place>();
        public ObservableCollection<Place> Places
        {
            get { return _Places; }
            set { SetProperty(ref _Places, value); }
        }

        ObservableCollection<Area> _Areas = new ObservableCollection<Area>();
        public ObservableCollection<Area> Areas
        {
            get { return _Areas; }
            set { SetProperty(ref _Areas, value); }
        }

        ObservableCollection<Set> _Sets = new ObservableCollection<Set>();
        public ObservableCollection<Set> Sets
        {
            get { return _Sets; }
            set { SetProperty(ref _Sets, value); }
        }

        ObservableCollection<string> _TimeVsPlanStatuses = new ObservableCollection<string>();

        public ObservableCollection<string> TimeVsPlanStatuses
        {
            get { return _TimeVsPlanStatuses; }
            set { SetProperty(ref _TimeVsPlanStatuses, value); }
        }



        ActionType _SelectedActionType;

        public ActionType SelectedActionType
        {
            get { return _SelectedActionType; }
            set { SetProperty(ref _SelectedActionType, value); }
        }

        Place _SelectedPlace;

        public Place SelectedPlace
        {
            get { return _SelectedPlace; }
            set { SetProperty(ref _SelectedPlace, value); }
        }

        Area _SelectedArea;

        public Area SelectedArea
        {
            get { return _SelectedArea; }
            set { SetProperty(ref _SelectedArea, value); }
        }

        Set _SelectedSet;

        public Set SelectedSet
        {
            get { return _SelectedSet; }
            set { SetProperty(ref _SelectedSet, value); }
        }

        string _SelectedTimeVsPlanStatus;

        public string SelectedTimeVsPlanStatus
        {
            get { return _SelectedTimeVsPlanStatus; }
            set { SetProperty(ref _SelectedTimeVsPlanStatus, value); }
        }


        public ICommand SetFilterCommand { get; private set; }
        public ICommand ClearAllCommand { get; private set; }
        public ICommand ClearActionTypeCommand { get; private set; }
        public ICommand ClearPlaceCommand { get; private set; }
        public ICommand ClearAreaCommand { get; private set; }
        public ICommand ClearSetCommand { get; private set; }
        public ICommand ClearTimeVsPlanCommand { get; private set; }
    }
}
