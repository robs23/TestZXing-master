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
    public class HandlingsPageViewModel : INotifyPropertyChanged
    {
        private bool _isWorking { get; set; }
        private int _processId { get; set; }
        private ObservableCollection<Handling> Handlings { get; set; }

        public HandlingsPageViewModel(int processId)
        {
            _processId = processId;
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

        public int ProcessId
        {
            get
            {
                return _processId;
            }
            set
            {
                if (value != _processId)
                {
                    _processId = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Handling> _openHandlings { get; set; }
        public ObservableCollection<Handling> OpenHandlings
        {
            get
            {
                return _openHandlings;
            }
            set
            {
                if (_openHandlings != value)
                {
                    _openHandlings = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Handling> _completedHandlings { get; set; }
        public ObservableCollection<Handling> CompletedHandlings
        {
            get
            {
                return _completedHandlings;
            }
            set
            {
                if (_completedHandlings != value)
                {
                    _completedHandlings = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task Initialize()
        {
            IsWorking = true;
            HandlingKeeper keeper = new HandlingKeeper();
            Handlings = await keeper.GetHandligngsByProcess(ProcessId);
            OpenHandlings = new ObservableCollection<Handling>(Handlings.Where(h=>h.IsCompleted==false));
            CompletedHandlings = new ObservableCollection<Handling>(Handlings.Where(h => h.IsCompleted == true));
            IsWorking = false;
            OnPropertyChanged(nameof(OpenHandlings));
            OnPropertyChanged(nameof(CompletedHandlings));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
