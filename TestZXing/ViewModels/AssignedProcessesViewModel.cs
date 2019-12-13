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
using TestZXing.Views;

namespace TestZXing.ViewModels
{
    public class AssignedProcessesViewModel : INotifyPropertyChanged
    {

        public AssignedProcessesViewModel()
        {

        }

        public async void Initialize()
        {
            ProcessKeeper keeper = new ProcessKeeper();
            await keeper.GetUsersOpenProcesses("IsActive=false and IsFrozen=false");
            if (keeper.Items.Any())
            {
                //there are planned processes assigned to the user
                Items = new ObservableCollection<Process>(keeper.Items.OrderBy(i => i.PlannedStart));
                WelcomeText = $"Poniżej znajduje się lista planowanych konserwacji w bieżącym okresie, które zostały przypisane min. do Ciebie. Liczba pozycji: {Items.Count}";
                Display();
            }
        }

        private ObservableCollection<Process> _Items { get; set; }
        public ObservableCollection<Process> Items
        {
            get
            {
                return _Items;
            }
            set
            {
                if(value != _Items)
                {
                    _Items = value;
                    OnPropertyChanged();
                }
            }
        }


        private string _WelcomeText { get; set; }

        public string WelcomeText
        {
            get
            {
                return _WelcomeText;
            }
            set
            {
                if (value != _WelcomeText)
                {
                    _WelcomeText = value;
                    OnPropertyChanged();
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
        
        public void Display()
        {
            PopupNavigation.Instance.PushAsync(new AssignedProcesses(this), true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
