using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestZXing.Models;
using TestZXing.Static;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class ProcessAttachmentsViewModel: BaseViewModel
    {
        FileKeeper FileKeeper = new FileKeeper();

        public ProcessAttachmentsViewModel()
        {
            CapturePhotoCommand = new AsyncCommand(CapturePhoto);
            PickPhotoCommand = new AsyncCommand(PickPhoto);
            RemoveItemsCommand = new AsyncCommand(RemoveItems);
            Items = new ObservableRangeCollection<File>();
            SelectedItems = new ObservableRangeCollection<File>();
            FileKeeper = new FileKeeper();
        }

        public ICommand CapturePhotoCommand { get; }

        public async Task CapturePhoto()
        {
            
        }

        public ICommand PickPhotoCommand { get; }
        public async Task PickPhoto()
        {
            FileResult result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Wybierz zdjęcie"
            });

            if (result != null)
            {
                var stream = await result.OpenReadAsync();
                ActiveElementPath = ImageSource.FromStream(() => stream);
                Items.Add(new File
                {
                    Name = result.FileName,
                    Link = result.FullPath
                }) ;
                IsElementActive = true;
            }
        }

        public ICommand RemoveItemsCommand { get; }

        public async Task Initialize(int? processId = null)
        {
            base.Initialize();

            if (processId != null)
            {
                await FileKeeper.Reload($"ProcessId={processId} and CreatedBy={RuntimeSettings.CurrentUser.UserId}");
                Items = new ObservableRangeCollection<File>(FileKeeper.Items);
                Task.Run(() => TakeSnapshot());
            }
        }

        public async Task TakeSnapshot()
        {
            //remember what you've already saved
            SavedItems = this.Items.CloneJson<ObservableRangeCollection<File>>();
        }

        ImageSource _ActiveElementPath;

        public ImageSource ActiveElementPath
        {
            get
            {
                return _ActiveElementPath;
            }
            set
            {
                SetProperty(ref _ActiveElementPath, value);
            }
        }

        public async Task Update()
        {


        }


        public async Task<bool> IsDirty()
        {
            bool res = false;
            if (Items != null && SavedItems != null)
            {
                if (Items.Count() > SavedItems.Count())
                {
                    res = true;
                }
                else
                {
                    foreach (File f in Items)
                    {
                        if (f.FileId == 0)
                        {
                            //on the whole, items without id definitely haven't been saved yet
                            res = true;
                            break;
                        }
                        else if (!SavedItems.Any(i => i.FileId == f.FileId))
                        {
                            //Current item hasn't been saved yet
                            res = true;
                            break;
                        }
                    }
                    foreach (File f in SavedItems)
                    {
                        if (!Items.Any(i => i.FileId == f.FileId))
                        {
                            //Current item must hvae been deleted but this info hasn't been saved
                            res = true;
                            break;
                        }
                    }
                }
            }


            return res;
        }

        ObservableRangeCollection<File> _Items;

        public ObservableRangeCollection<File> Items
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

        ObservableRangeCollection<File> _SavedItems;

        public ObservableRangeCollection<File> SavedItems
        {
            get
            {
                return _SavedItems;
            }
            set
            {
                SetProperty(ref _SavedItems, value);
            }
        }

        private async Task RemoveItems()
        {
            if (SelectedItems.Count > 0)
            {
                for (int i = SelectedItems.Count; i > 0; i--)
                {
                    if (SelectedItems[i - 1].FileId > 0)
                    {
                        //if saved, add it to RemovedItems collection for further removal
                        RemovedItems.Add(SelectedItems[i - 1]);
                    }
                    Items.Remove(SelectedItems[i - 1]);
                    SelectedItems.Remove(SelectedItems[i - 1]);
                }
            }
        }

        public async Task<string> Save(int processId)
        {
            string res = "OK";

            if (res == "OK")
            {
                List<Task<string>> SaveTasks = new List<Task<string>>();

                foreach (File f in Items)
                {
                    if (f.FileId == 0)
                    {
                        f.CreatedBy = RuntimeSettings.CurrentUser.UserId;
                        f.CreatedOn = DateTime.Now;
                        SaveTasks.Add(f.Add($"ProcessId={processId}"));
                    }
                    else
                    {
                        //if (!pu.IsSaved)
                        //{
                        //SaveTasks.Add(f.Edit());
                        //}

                    }
                }
                if (RemovedItems.Any())
                {
                    foreach (File f in RemovedItems)
                    {
                        SaveTasks.Add(f.Remomve());
                    }
                }

                IEnumerable<string> results = await Task.WhenAll<string>(SaveTasks);
                for (int i = RemovedItems.Count; i > 0; i--)
                {
                    if (RemovedItems[i - 1].IsSaved)
                    {
                        RemovedItems.Remove(RemovedItems[i - 1]);
                    }
                }
                FileKeeper.Items = Items;
                FileKeeper.AddToUploadQueue();
                Task.Run(() => TakeSnapshot());
                if (results.Where(r => r != "OK").Any())
                {
                    return string.Join("; ", results.Where(r => r != "OK"));
                }
                else
                {
                    return "OK";
                }
            }
            return res;
        }

        public string Validate()
        {
            string res = "OK";
            try
            {
                foreach (File f in Items)
                {
      
                }
            }
            catch (Exception ex)
            {
                res = $"Wystąpił nieoczekiwany błąd. {ex.Message}";
            }

            return res;
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

        bool _IsElementActive = false;
        public bool IsElementActive
        {
            get
            {
                return _IsElementActive;
            }
            set
            {
                SetProperty(ref _IsElementActive, value);
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

        ObservableRangeCollection<File> _RemovedItems = new ObservableRangeCollection<File>();

        public ObservableRangeCollection<File> RemovedItems
        {
            get { return _RemovedItems; }
            set { SetProperty(ref _RemovedItems, value); }
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
                    return "Nie dodano żadnego zdjęcia do tego zgłoszenia. Dodaj pierwsze zdjęcie korzystając z przycisków poniżej. Przycisk LUPY pozwala dodać wcześniej zrobione zdjęcie, przycisk APARAT pozwala zrobić nowe zdjęcie..";
                }
            }
        }
    }
}
