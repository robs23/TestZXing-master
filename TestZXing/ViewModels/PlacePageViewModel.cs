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
    public class PlacePageViewModel: BaseViewModel
    {
        public Place _this { get; set; }

        public ProcessAttachmentsViewModel ProcessAttachmentsVm { get; set; }

        public PlacePageViewModel(Place place)
        {
            _this = place;
            ShowAttachmentsCommand = new AsyncCommand(ShowAttachments);
            ChangeImageCommand = new AsyncCommand(ChangeImage);
            ShowHistoryCommand = new AsyncCommand(ShowHistory);

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
        public string AreaName
        {
            get { return _this.AreaName; }
            set
            {
                if (value != _this.AreaName)
                {
                    _this.AreaName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Priority
        {
            get { return _this.Priority; }
            set
            {
                if (value != _this.Priority)
                {
                    _this.Priority = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SetName
        {
            get { return _this.SetName; }
            set
            {
                if (value != _this.SetName)
                {
                    _this.SetName = value;
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
                if (_ImageUrl != value)
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
                if (_IsSaveable != value)
                {
                    _IsSaveable = value;
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

        public ICommand ShowAttachmentsCommand { get; }
        public ICommand ChangeImageCommand { get; }
        public ICommand ShowHistoryCommand { get; }

        public async Task Save()
        {
            string _Result = "OK";
            IsWorking = true;
            if (!string.IsNullOrEmpty(ImageUrl.ToString()))
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
            if (_Result == "OK")
            {
                _Result = await ProcessAttachmentsVm.Save(placeId: _this.PlaceId);
                IsWorking = false;
                if (_Result == "OK")
                {
                    await Application.Current.MainPage.DisplayAlert("Zapisano", "Zapis zakończony powodzeniem!", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd zapisu", $"Zapis załączników zakończony błędem: {_Result}", "OK");
                }
            }
            else
            {
                IsWorking = false;
                await Application.Current.MainPage.DisplayAlert("Błąd zapisu", $"Zapis danych zasobu zakończony błędem: {_Result}", "OK");

            }

        }

        public async Task ShowAttachments()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ProcessAttachementsPage(ProcessAttachmentsVm));
        }

        public async Task ShowHistory()
        {
            Application.Current.MainPage.Navigation.PushAsync(new CompletedProcessesForPlace(_this), true);
        }

        public async Task Initialize()
        {
            base.Initialize();
            IsSaveable = false;
            Task.Run(() => InitializeProcessAttachments());
            if (string.IsNullOrEmpty(_this.PlaceToken))
            {
                //got to download it
                PlacesKeeper placesKeeper = new PlacesKeeper();
                Place p = await placesKeeper.GetPlace(_this.PlaceId);
                _this = p;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Priority));
                OnPropertyChanged(nameof(SetName));
                OnPropertyChanged(nameof(AreaName));

            }
            if (!string.IsNullOrWhiteSpace(_this.Image))
            {
                ImageUrl = Static.Secrets.ApiAddress + Static.RuntimeSettings.FilesPath + _this.Image;
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

                if (_this.PlaceId > 0)
                {
                    Task.Run(() => ProcessAttachmentsVm.Initialize(placeId: _this.PlaceId));
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
