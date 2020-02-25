using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestZXing.Models;
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class PartsPageViewModel : INotifyPropertyChanged
    {
        public PartKeeper Keeper { get; set; }

        public ICommand ReloadCommand { get; }

        public PartsPageViewModel()
        {
            Keeper = new PartKeeper();
            ReloadCommand = new AsyncCommand(Reload);
        }

        private async Task Reload()
        {
            await Keeper.Reload(null, 1, 50);

        }

        private  ObservableCollection<Part> _Items { get; set; }

        public ObservableCollection<Part> Items
        {
            get
            {
                return _Items;
            }
            set
            {
                if (value != _Items)
                {
                    _Items = value;
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
