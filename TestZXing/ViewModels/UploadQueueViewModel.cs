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
    public class UploadQueueViewModel : BaseViewModel
    {
        FileKeeper fileKeeper = new FileKeeper();
        ObservableCollection<File> _Items = new ObservableCollection<File>();

        public UploadQueueViewModel()
        {
            StartUploadCommand = new AsyncCommand(StartUpload);
            SelectedItems = new ObservableRangeCollection<File>();
        }

        public async Task Initialize()
        {
            IsWorking = true;
            base.Initialize();
            fileKeeper = RuntimeSettings.UploadKeeper;
            if (!fileKeeper.IsWorking)
            {
                await fileKeeper.RestoreUploadQueue();
            }
            
            //await fileKeeper.DeleteAll();
            Items = fileKeeper.Items;
            if (Items.Any())
            {
                HasItems = true;
            }
            IsWorking = false;
        }


        public async Task StartUpload()
        {
            IsWorking = true;
            await fileKeeper.Upload();
            IsWorking = false;
        }

        public ICommand StartUploadCommand { get; }

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
                res = await RemoveItems(new ObservableRangeCollection<File>(Items));
            }

            return res;
        }

        public async Task<string> RemoveItems(ObservableRangeCollection<File> rItems)
        {
            IsWorking = true;
            string result = "OK";

            try
            {
                List<Task<string>> ToRemove = new List<Task<string>>();
                foreach (File f in rItems)
                {
                    ToRemove.Add(f.Remove());
                }
                IEnumerable<string> res = await Task.WhenAll<string>();
                if (res.Any(i => i != "OK"))
                {
                    result = string.Join(Environment.NewLine, res.Where(i => i != "OK"));
                }

                for (int i = rItems.Count; i > 0; i--)
                {
                    await rItems[i-1].RemoveFromUploadQueue();
                    Items.Remove(rItems[i-1]);
                    if (SelectedItems.Any(x => x.FileId == rItems[i-1].FileId))
                    {
                        //remove it also from selected list
                        SelectedItems.Remove(rItems[i-1]);
                    }
                }
            }
            catch (Exception ex)
            {
                result = $"Błąd podczas próby usunięcia pliku.. Szczegóły: {ex.ToString()}";
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
