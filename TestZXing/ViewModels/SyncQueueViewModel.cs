using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestZXing.Models;
using TestZXing.Static;

namespace TestZXing.ViewModels
{
    public class SyncQueueViewModel: BaseViewModel
    {
        UserLogKeeper keeper = new UserLogKeeper();
        ObservableCollection<UserLog> _Items = new ObservableCollection<UserLog>();

        public SyncQueueViewModel()
        {
            StartSyncCommand = new AsyncCommand(StartSync);
            SelectedItems = new ObservableRangeCollection<UserLog>();
        }

        public async Task Initialize()
        {
            IsWorking = true;
            base.Initialize();
            keeper = new UserLogKeeper();
            if (!keeper.IsWorking)
            {
                await keeper.RestoreSyncQueue();
            }

            Items = keeper.Items;
            if (Items.Any())
            {
                HasItems = true;
            }
            IsWorking = false;
        }


        public async Task StartSync()
        {
            IsWorking = true;
            await keeper.Sync();
            IsWorking = false;
        }

        public ICommand StartSyncCommand { get; }

        public async Task<string> RemoveSelected()
        {
            string res = "OK";

            if (SelectedItems.Count > 0)
            {
                res = await RemoveItems(SelectedItems);
            }

            return res;
        }

        public async Task<string> RemoveAll()
        {
            string res = "OK";

            if (Items.Count > 0)
            {
                res = await RemoveItems(new ObservableRangeCollection<UserLog>(Items));
            }

            return res;
        }

        public async Task<string> RemoveItems(ObservableRangeCollection<UserLog> rItems)
        {
            IsWorking = true;
            string result = "OK";

            try
            {
                List<Task<string>> ToRemove = new List<Task<string>>();
                foreach (var i in rItems)
                {
                    ToRemove.Add(i.Remove());
                }
                IEnumerable<string> res = await Task.WhenAll<string>();
                if (res.Any(i => i != "OK"))
                {
                    result = string.Join(Environment.NewLine, res.Where(i => i != "OK"));
                }

                for (int i = rItems.Count; i > 0; i--)
                {
                    await rItems[i - 1].RemoveFromSyncQueue();
                    Items.Remove(rItems[i - 1]);
                    if (SelectedItems.Any(x => x.Id == rItems[i - 1].Id))
                    {
                        //remove it also from selected list
                        SelectedItems.Remove(rItems[i - 1]);
                    }
                }
            }
            catch (Exception ex)
            {
                result = $"Błąd podczas próby usunięcia danych offline.. Szczegóły: {ex.ToString()}";
            }
            IsWorking = false;

            return result;
        }

        bool _HasItems = false;
        public bool HasItems
        {
            get { return _HasItems; }
            set
            {
                SetProperty(ref _HasItems, value);
            }
        }

        public ObservableCollection<UserLog> Items
        {
            get
            {
                return _Items;
            }
            set
            {
                SetProperty(ref _Items, value);
            }
        }
        ObservableRangeCollection<UserLog> _SelectedItems = new ObservableRangeCollection<UserLog>();
        public ObservableRangeCollection<UserLog> SelectedItems
        {
            get { return _SelectedItems; }
            set
            {
                bool changed = SetProperty(ref _SelectedItems, value);
                if (changed)
                {
                    OnPropertyChanged(nameof(RemovableSelected));
                }
            }
        }

        bool _RemovableSelected;

        public bool RemovableSelected
        {
            get
            {
                return _RemovableSelected;
            }
            set
            {
                SetProperty(ref _RemovableSelected, value);
            }
        }

        public string EmptyViewCaption
        {
            get
            {
                if (IsWorking)
                {
                    return "Trwa wczytywanie..";
                }
                else
                {
                    return "Brak danych offline czekających na wysyłkę..";
                }
            }
        }
    }
}
