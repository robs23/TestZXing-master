using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;

namespace TestZXing.ViewModels
{
    class DiaryViewModel : INotifyPropertyChanged
    {
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

        public ObservableCollection<Handling> _handlings { get; set; }
        public ObservableCollection<Handling> Handlings
        {
            get
            {
                return _handlings;
            }
            set
            {
                if (_handlings != value)
                {
                    _handlings = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task Initialize()
        {
            IsWorking = true;
            HandlingKeeper keeper = new HandlingKeeper();
            Handlings = await keeper.GetUserHandlings();
            OnPropertyChanged(nameof(Handlings));
            IsWorking = false;
        }

        private Place _selectedItem { get; set; }
        public Place SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
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
