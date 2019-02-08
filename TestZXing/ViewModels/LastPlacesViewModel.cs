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
    public class LastPlacesViewModel : INotifyPropertyChanged
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
                    _isWorking = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Place> _places { get; set; }
        public ObservableCollection<Place> Places
        {
            get
            {
                return _places;
            }
            set
            {
                if (_places != value)
                {
                    _places = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task Initialize()
        {
            IsWorking = true;
            PlacesKeeper keeper = new PlacesKeeper();
            Places = await keeper.GetUsersLastPlaces();
            IsWorking = false;
            OnPropertyChanged(nameof(Places));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
