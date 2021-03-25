using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestZXing.Interfaces;
using TestZXing.Models;
using TestZXing.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class PartPageViewModel : BaseViewModel
    {
        public Part _this { get; set; }

        public ProcessAttachmentsViewModel ProcessAttachmentsVm { get; set; }

        public PartPageViewModel(Part part)
        {
            _this = part;
            ShowAttachmentsCommand = new AsyncCommand(ShowAttachments);
            ChangeImageCommand = new AsyncCommand(ChangeImage);
        }

        public string Name
        {
            get { return _this.Name; }
            set
            {
                if (value != _this.Name)
                {
                    _this.Name = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Description
        {
            get { return _this.Description; }
            set
            {
                if (value != _this.Description)
                {
                    _this.Description = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ProducerName
        {
            get { return _this.ProducerName; }
            set
            {
                if (value != _this.ProducerName)
                {
                    _this.ProducerName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Symbol
        {
            get { return _this.Symbol; }
            set
            {
                if (value != _this.Symbol)
                {
                    _this.Symbol = value;
                    OnPropertyChanged();
                }
            }
        }

        

        private ImageSource _ImageUrl { get; set; } = "image_placeholder_128.png";
        public ImageSource ImageUrl
        {
            get
            {
                return _ImageUrl;
            }
            set
            {
                if(_ImageUrl != value)
                {
                    _ImageUrl = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsSaveable { get; set; }

        public bool IsSaveable
        {
            get
            {
                return _IsSaveable;
            }
            set
            {
                if(_IsSaveable != value)
                {
                    _IsSaveable = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ShowAttachmentsCommand { get; }
        public ICommand ChangeImageCommand { get; }

        public async Task Save()
        {
            string _Result = await ProcessAttachmentsVm.Save(partId: _this.PartId);
            if(_Result == "OK")
            {
                await Application.Current.MainPage.DisplayAlert("Zapisano", "Zapis zakończony powodzeniem!", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Błąd zapisu", $"Zapis zakończony błędem: {_Result}", "OK");
            }
        }

        public async Task ShowAttachments()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ProcessAttachementsPage(ProcessAttachmentsVm));
        }

        public async Task Initialize()
        {
            base.Initialize();
            IsSaveable = false;
            Task.Run(() => InitializeProcessAttachments());
            if (!string.IsNullOrWhiteSpace(_this.Image))
            {
                ImageUrl = Static.Secrets.ApiAddress + Static.RuntimeSettings.FilesPath + _this.Image;
            }
            
        }

        public async Task ChangeImage()
        {
            FileResult result = null;
            string res = await Application.Current.MainPage.DisplayActionSheet("Co chcesz zrobić?", "Anuluj", null, "Zrób zdjęcie", "Wybierz z galerii");
            if(res == "Zrób zdjęcie")
            {
                result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Zrób zdjęcie"
                });
            }
            else if(res == "Wybierz z galerii")
            {
                result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Wybierz zdjęcie"
                });
            }

            if(result!= null)
            {
                await EmbedMedia(result, true);
            }
            
        }

        public async Task EmbedMedia(FileResult result, bool isPhoto)
        {

            if (result != null)
            {
                string fullPath = string.Empty;

                //check if it needs to be saved first
                if (result.FullPath.Contains("com.companyname.TestZXing"))
                {
                    //save
                    fullPath = await SaveToGallery(result, isPhoto);
                }
                else
                {
                    fullPath = result.FullPath;
                }

                var stream = await result.OpenReadAsync();
                ImageUrl = fullPath; //ImageSource.FromStream(() => stream);
                IsSaveable = true;
                //Items.Add(new File
                //{
                //    Name = result.FileName,
                //    Link = fullPath,
                //    ImageSource = fullPath
                //});
            }
        }

        public async Task<string> SaveToGallery(FileResult result, bool isPhoto)
        {

            string mPath = string.Empty;
            if (isPhoto)
            {
                mPath = DependencyService.Get<IFileHandler>().GetImageGalleryPath();
            }
            else
            {
                mPath = DependencyService.Get<IFileHandler>().GetVideoGalleryPath();
            }

            string nPath = System.IO.Path.Combine(mPath, result.FileName);
            using (var stream = await result.OpenReadAsync())
            {
                using (var nStream = System.IO.File.OpenWrite(nPath))
                {
                    await stream.CopyToAsync(nStream);
                    return nPath;
                }
            }
        }

        public async Task<bool> IsDirty()
        {
            List<Task<bool>> tasks = new List<Task<bool>>();
            try
            {
                Task<bool> AttachmentIsDirtyTask;

                if (ProcessAttachmentsVm != null)
                {
                    AttachmentIsDirtyTask = Task.Run(() => ProcessAttachmentsVm.IsDirty());
                    tasks.Add(AttachmentIsDirtyTask);
                }

                if (tasks.Any())
                {
                    IEnumerable<bool> res = await Task.WhenAll<bool>(tasks);

                    if (res.Any(r => r == true))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;

            }
            catch (Exception ex)
            {
                //Logger.Error(ex, "IsDirty: {ex}");
                Static.Functions.CreateError(ex, "IsDirty throws error", "IsDirty", this.GetType().Name);
                return false;
            }
        }

        public async Task InitializeProcessAttachments()
        {
            try
            {
                ProcessAttachmentsVm = new ProcessAttachmentsViewModel();

                if (_this.PartId > 0)
                {
                    Task.Run(() => ProcessAttachmentsVm.Initialize(partId: _this.PartId));
                }
                else
                {
                    Task.Run(() => ProcessAttachmentsVm.Initialize());
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
