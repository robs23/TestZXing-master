using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestZXing.Interfaces;
using TestZXing.Models;
using TestZXing.Static;
using TestZXing.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using File = TestZXing.Models.File;

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
            TapCommand = new AsyncCommand<File>(OpenFile, (e) => {
                if (SelectionModeEnabled)
                {
                    return false;
                }
                return true;
                }) ;
            Items = new ObservableRangeCollection<File>();
            SelectedItems = new ObservableRangeCollection<File>();
            FileKeeper = new FileKeeper();
        }

        public ICommand CapturePhotoCommand { get; }

        public async Task OpenFile(File f)
        {
            Uri uri = null;
            if (f.IsUploaded==true)
            {
                if (!string.IsNullOrEmpty(f.Token) && !string.IsNullOrEmpty(f.Type))
                {
                    uri = new Uri(Secrets.ApiAddress + RuntimeSettings.FilesPath + $"{f.Token}.{f.Type.Trim()}");
                }
            }
            else
            {
                if (System.IO.File.Exists(f.Link))
                {
                    uri = new Uri(f.Link);
                }
            }


            if (uri != null)
            {
                if (f.IsUploaded==true)
                {
                    await Browser.OpenAsync(uri);
                }
                else
                {
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(f.Link)
                    });
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Plik niedostępny", $"Plik nie jest jeszcze dostępny na serwerze({ f.Token}.{f.Type.Trim()}) ponieważ {f.CreatedByName} nie zsynchronizował jeszcze plików..", "OK");
            }

        }



        public async Task CapturePhoto()
        {
            FileResult result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = "Zrób zdjęcie"
            });

            await EmbedMedia(result, true);
        }

        public async Task CaptureVideo()
        {
            FileResult result = await MediaPicker.CaptureVideoAsync(new MediaPickerOptions
            {
                Title = "Nagraj wideo"
            });

            await EmbedMedia(result, false);
        }

        public async Task EmbedMedia(FileResult result, bool isPhoto)
        {
            
            if (result != null)
            {
                string fullPath = string.Empty;

                //check if it needs to be saved first
                if(result.FullPath.Contains("com.companyname.TestZXing"))
                {
                    //save
                    fullPath = await Functions.SaveToGallery(result, isPhoto);
                }
                else
                {
                    fullPath = result.FullPath;
                }

                var stream = await result.OpenReadAsync();
                ActiveElementPath = ImageSource.FromStream(() => stream);
                Items.Add(new File
                {
                    Name = result.FileName,
                    Link = fullPath,
                    ImageSource = fullPath
                });
            }
        }


        public ICommand PickPhotoCommand { get; }
        public async Task PickPhoto()
        {
            FileResult result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Wybierz zdjęcie"
            });

            await EmbedMedia(result, true);
        }

        public async Task PickVideo()
        {
            FileResult result = await MediaPicker.PickVideoAsync(new MediaPickerOptions
            {
                Title = "Wybierz film"
            });

            await EmbedMedia(result, false);
        }

        public ICommand RemoveItemsCommand { get; }

        public ICommand TapCommand { get; }

        public async Task Initialize(int? processId = null, int? partId = null, int? placeId=null)
        {
            base.Initialize();

            if (processId != null || partId != null || placeId != null)
            {
                if(processId != null)
                {
                    await FileKeeper.Reload($"ProcessId={processId}");
                }else if(partId != null)
                {
                    await FileKeeper.Reload($"PartId={partId}");
                }
                else
                {
                    await FileKeeper.Reload($"PlaceId={placeId}");
                }
                
                Items = new ObservableRangeCollection<File>(FileKeeper.Items);
                Task.Run(() => TakeSnapshot());
                await LoadImages();
                LoadPreviews();
            }
        }

        public async Task LoadImages()
        {
            if (Items.Any())
            {
                foreach(File item in Items)
                {
                    item.ImageSource = item.ThumbnailPlaceholder;
                }
            }
        }

        public async Task LoadPreviews()
        {
            if (Items.Any())
            {
                foreach (File item in Items.Where(i=>i.IsImage==true))
                {
                    if (item.IsUploaded == false)
                    {
                        //check if file is in Link destination
                        try
                        {
                            if (!string.IsNullOrEmpty(item.Link))
                            {
                                if (System.IO.File.Exists(item.Link))
                                {
                                    item.ImageSource = item.Link;
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        //it's uploaded so take from server
                        if(!string.IsNullOrEmpty(item.Token) && !string.IsNullOrEmpty(item.Type))
                        {
                            item.ImageSource = Secrets.ApiAddress + RuntimeSettings.ThumbnailsPath + $"{item.Token}.{item.Type.Trim()}";
                        }
                        

                    }
                }
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

        private bool _SelectionModeEnabled { get; set; } = false;

        public bool SelectionModeEnabled
        {
            get
            {
                return _SelectionModeEnabled;
            }
            set
            {
                if(value != _SelectionModeEnabled)
                {
                    _SelectionModeEnabled = value;
                    if (value == true)
                    {
                        SelectionMode = SelectionMode.Multiple;
                    }
                    else
                    {
                        SelectionMode = SelectionMode.None;
                        

                    }
                    OnPropertyChanged();
                }
            }
        }

        private SelectionMode _SelectionMode { get; set; } = SelectionMode.None;

        public SelectionMode SelectionMode
        {
            get
            {
                return _SelectionMode;
            }
            set
            {
                if(value != _SelectionMode)
                {
                    _SelectionMode = value;
                    OnPropertyChanged();
                }
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

        public async Task<string> Save(int? processId = null, int? partId = null, int? placeId = null)
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
                        try
                        {
                            f.Type = System.IO.Path.GetExtension(f.Link).Substring(1, 3);
                            var s = new System.IO.FileInfo(f.Link);
                            f.Size = s.Length;
                        }catch(Exception ex)
                        {

                        }
                        if(processId != null)
                        {
                            SaveTasks.Add(f.Add($"ProcessId={processId}"));
                        }else if(partId != null)
                        {
                            SaveTasks.Add(f.Add($"PartId={partId}"));
                        }else if(placeId != null)
                        {
                            SaveTasks.Add(f.Add($"PlaceId={placeId}"));
                        }
                        
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
                        SaveTasks.Add(f.Remove());
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
                    return "Nie dodano jeszcze żadnego załącznika.. Dodaj pierwsze zdjęcie/film korzystając z przycisków poniżej. Przycisk LUPY pozwala dodać wcześniej zrobione zdjęcie/film, przycisk APARAT pozwala zrobić nowe zdjęcie/film..";
                }
            }
        }
    }
}
