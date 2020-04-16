using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestZXing.Models;
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class ProcessesFilterViewModel : BaseViewModel
    {

        public ProcessesFilterViewModel()
        {
            ActionTypes = new ObservableCollection<ActionType>();
            Places = new ObservableCollection<Place>();
            Areas = new ObservableCollection<Area>();

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
            ClearAllCommand = new Command(
                execute: () =>
                {
                    SelectedArea = null;
                    SelectedPlace = null;
                    SelectedArea = null;
                },
                canExecute: () =>
                {
                    return !IsWorking;
                });
            SetFilterCommand = new Command(
                execute: () =>
                {

                },
                canExecute: () =>
                {
                    return !IsWorking;
                });
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

        //int _SelectedActionTypeIndex;

        //public int SelectedActionTypeIndex
        //{
        //    get{ return _SelectedActionTypeIndex; }
        //    set
        //    {
        //        bool changed = SetProperty(ref _SelectedActionTypeIndex, value);
        //        if (changed) { SelectedActionType = ActionTypes[value]; }
        //    }
        //}

        ActionType _SelectedActionType;

        public ActionType SelectedActionType
        {
            get { return _SelectedActionType; }
            set { SetProperty(ref _SelectedActionType, value); }
        }

        //int _SelectedPlaceIndex;

        //public int SelectedPlaceIndex
        //{
        //    get { return _SelectedPlaceIndex; }
        //    set
        //    {
        //        bool changed = SetProperty(ref _SelectedPlaceIndex, value);
        //        if (changed) { _SelectedPlace = Places[value]; }
        //    }
        //}

        Place _SelectedPlace;

        public Place SelectedPlace
        {
            get { return _SelectedPlace; }
            set { SetProperty(ref _SelectedPlace, value); }
        }

        //int _SelectedAreaIndex;

        //public int SelectedAreaIndex
        //{
        //    get { return _SelectedAreaIndex; }
        //    set
        //    {
        //        bool changed = SetProperty(ref _SelectedAreaIndex, value);
        //        if (changed) { SelectedArea = Areas[value]; }
        //    }
        //}

        Area _SelectedArea;

        public Area SelectedArea
        {
            get { return _SelectedArea; }
            set { SetProperty(ref _SelectedArea, value); }
        }

        public ICommand SetFilterCommand { get; private set; }
        public ICommand ClearAllCommand { get; private set; }
        public ICommand ClearActionTypeCommand { get; private set; }
        public ICommand ClearPlaceCommand { get; private set; }
        public ICommand ClearAreaCommand { get; private set; }
    }
}
