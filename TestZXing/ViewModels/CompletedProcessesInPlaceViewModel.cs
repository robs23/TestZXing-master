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
    public class CompletedProcessesInPlaceViewModel : INotifyPropertyChanged
    {
        private bool _isWorking { get; set; }
        private Place _place { get; set; }
        private ObservableCollection<Process> _items { get; set; }

        public CompletedProcessesInPlaceViewModel(Place place)
        {
            _place = place;
        }

        public ObservableCollection<Process> Items
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
                        PopupNavigation.Instance.PopAsync(true); // Hide loading screen
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

        public Place Place
        {
            get
            {
                return _place;
            }
            set
            {
                if (value != _place)
                {
                    _place = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task Initialize()
        {
            IsWorking = true;
            Items = new ObservableCollection<Process>(await Place.GetProcesses(false));
            IsWorking = false;
            OnPropertyChanged(nameof(Items));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
