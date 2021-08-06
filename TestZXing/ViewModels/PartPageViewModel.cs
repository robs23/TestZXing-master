using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
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

        public bool IsWorking
        {
            get
            {
                return base.IsWorking;
            }
            set
            {
                base.IsWorking = value;
                if (value == false)
                {
                    //PopupNavigation.Instance.PopAsync(true); // Hide loading screen
                    if (PopupNavigation.Instance.PopupStack.Any()) { PopupNavigation.Instance.PopAllAsync(true); }
                }
                else
                {
                    PopupNavigation.Instance.PushAsync(new LoadingScreen(), true); // Show loading screen
                }
            }
        }

        private ImageSource _ImageUrl { get; set; } = RuntimeSettings.ImagePlaceholderName;
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
                    OnPropertyChanged(nameof(IsSaveable));
                }
            }
        }

        public ICommand ShowAttachmentsCommand { get; }
        public ICommand ChangeImageCommand { get; }

        public async Task Save()
        {
            string _Result = "OK";
            IsWorking = true;
            if (!string.IsNullOrEmpty(ImageUrl.ToString()))
            {
                if (!ImageUrl.ToString().Contains(RuntimeSettings.ImagePlaceholderName))
                {
                    if (!ImageUrl.ToString().Contains("Uri"))
                    {
                        try
                        {
                            string imgPath = ImageUrl.ToString().Split(':')[1].Trim();
                            _Result = await _this.Edit(imgPath);
                        }
                        catch (Exception ex)
                        {
                            _Result = ex.Message;
                        }
                    }
                }
                
            }
            if(_Result == "OK")
            {
                _Result = await ProcessAttachmentsVm.Save(partId: _this.PartId);
                IsWorking = false;
                if (_Result == "OK")
                {
                    IsWorking = false;
                    await Application.Current.MainPage.DisplayAlert("Zapisano", "Zapis zakończony powodzeniem!", "OK");
                }
                else
                {
                    IsWorking = false;
                    await Application.Current.MainPage.DisplayAlert("Błąd zapisu", $"Zapis załączników zakończony błędem: {_Result}", "OK");
                }
            }
            else
            {
                IsWorking = false;
                await Application.Current.MainPage.DisplayAlert("Błąd zapisu", $"Zapis danych części zakończony błędem: {_Result}", "OK");

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
                IsSaveable = true;
            }
            
        }

        public async Task ChangeImage()
        {
            string imagePath = await Functions.ChangeImage();

            if (!string.IsNullOrEmpty(imagePath))
            {
                ImageUrl = imagePath;
            }

            IsSaveable = true;

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
