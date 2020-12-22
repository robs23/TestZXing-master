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

namespace TestZXing.ViewModels
{
    public class UploadQueueViewModel : BaseViewModel
    {
        FileKeeper fileKeeper = new FileKeeper();
        ObservableCollection<File> _Items = new ObservableCollection<File>();

        public UploadQueueViewModel()
        {
            StartUploadCommand = new AsyncCommand(StartUpload);
            RemoveItemsCommand = new AsyncCommand(RemoveItems);
            SelectedItems = new ObservableRangeCollection<File>();
        }

        public async Task Initialize()
        {
            base.Initialize();
            await fileKeeper.RestoreUploadQueue();
            //await fileKeeper.DeleteAll();
            Items = fileKeeper.Items;
            if (Items.Any())
            {
                HasItems = true;
            }
        }

        public async Task StartUpload()
        {
            await fileKeeper.Upload();
        }

        public ICommand StartUploadCommand { get; }
        public ICommand RemoveItemsCommand { get; }

        private async Task RemoveItems()
        {
            if (SelectedItems.Count > 0)
            {
                for (int i = SelectedItems.Count; i > 0; i--)
                {
                    SelectedItems[i - 1].RemoveFromUploadQueue();
                    Items.Remove(SelectedItems[i - 1]);
                    SelectedItems.Remove(SelectedItems[i - 1]);
                }
            }
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

        public ObservableCollection<File> Items
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
        ObservableRangeCollection<File> _SelectedItems = new ObservableRangeCollection<File>();
        public ObservableRangeCollection<File> SelectedItems
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
                    return "Brak plików czekających na wysyłkę..";
                }
            }
        }

    }
}
