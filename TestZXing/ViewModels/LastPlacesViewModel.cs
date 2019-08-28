using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

        public string Icon { get
            {
                return Static.RuntimeSettings.CurrentUser.Icon;
            }
            }

        public async Task Initialize()
        {
            IsWorking = true;
            PlacesKeeper keeper = new PlacesKeeper();
            Places = await keeper.GetUsersLastPlaces();
            OnPropertyChanged(nameof(Places));
            //Debug.WriteLine(DateTime.Now + " 'JDE_Scan' - Preparing");
            //Task.Run(async () => {
            //        await Static.RuntimeSettings.CurrentUser.UpdateStatus();
            //        Debug.WriteLine(DateTime.Now + " 'JDE_Scan' - Updated");
            //}
            //);
            //Debug.WriteLine(DateTime.Now + " 'JDE_Scan' - Changing IsWorking to false");
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
