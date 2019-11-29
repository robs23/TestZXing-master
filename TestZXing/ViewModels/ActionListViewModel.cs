using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestZXing.Interfaces;
using TestZXing.Models;

namespace TestZXing.ViewModels
{
    public class ActionListViewModel : INotifyPropertyChanged
    {
        public ActionListViewModel(int ProcessId, int PlaceId)
        {
            this.ProcessId = ProcessId;
            this.PlaceId = PlaceId;
            ProcessActionKeeper = new ProcessActionKeeper();
            PlaceActionKeeper = new PlaceActionKeeper();
        }

        public async Task Initialize()
        {
            //get all the placeActions of this placeId and processActions for this process
            //and fill the list in

            var processReload = Task.Run(() => ProcessActionKeeper.Reload($"ProcessId={ProcessId}"));
            var placeReload = Task.Run(()=> PlaceActionKeeper.Reload($"PlaceId={PlaceId}"));

            await Task.WhenAll(processReload, placeReload);

            //go no further till both tasks complete

            CheckedItems = new ObservableCollection<ProcessAction>(ProcessActionKeeper.Items);
            Items = new ObservableCollection<IActionKeeper>(CheckedItems);
            foreach(PlaceAction p in PlaceActionKeeper.Items)
            {
                if (!CheckedItems.Any(i => i.ActionId == p.ActionId))
                {
                    Items.Add(p);
                }
                
            }
            IsInitialized = true;
        }

        private bool _IsInitialized { get; set; }

        public bool IsInitialized
        {
            get
            {
                return _IsInitialized;
            }
            set
            {
                if (value != _IsInitialized)
                {
                    _IsInitialized = value;
                }
            }
        }

        public PlaceActionKeeper PlaceActionKeeper { get; set; }
        public ProcessActionKeeper ProcessActionKeeper { get; set; }

        private int _ProcessId { get; set; }
        public int ProcessId
        {
            get
            {
                return _ProcessId;
            }
            set
            {
                if(value != _ProcessId)
                {
                    _ProcessId = value;
                }
            }
        }

        private int _PlaceId { get; set; }
        public int PlaceId
        {
            get
            {
                return _PlaceId;
            }
            set
            {
                if (value != _PlaceId)
                {
                    _PlaceId = value;
                }
            }
        }

        private bool _isWorking { get; set; }

        public bool IsWorking
        {
            get
            {
                return _isWorking;
            }
            set
            {
                if (_isWorking != value)
                {
                    if (value == false)
                    {
                        if (PopupNavigation.Instance.PopupStack.Any()) { PopupNavigation.Instance.PopAllAsync(true); }  // Hide loading screen
                    }
                    else
                    {
                        PopupNavigation.Instance.PushAsync(new LoadingScreen(), true); // Show loading screen
                    }
                    _isWorking = value;
                    OnPropertyChanged();
                }
            }
        }


        private ObservableCollection<IActionKeeper> _items { get; set; }
        public ObservableCollection<IActionKeeper> Items
        {
            get
            {
                return _items;
            }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ProcessAction> _checkedItems { get; set; }
        public ObservableCollection<ProcessAction> CheckedItems
        {
            get
            {
                return _checkedItems;
            }
            set
            {
                if (_checkedItems != value)
                {
                    _checkedItems = value;
                    OnPropertyChanged();
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
