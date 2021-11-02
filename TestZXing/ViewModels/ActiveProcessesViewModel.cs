using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter.Crashes;
using ModernHttpClient;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using TestZXing.Classes;
using TestZXing.Models;
using TestZXing.Static;
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class ActiveProcessesViewModel : INotifyPropertyChanged
    {

        public string FilterString { get; set; } = null;


        public ObservableRangeCollection<PlaceViewModel> List { get; private set; }
        = new ObservableRangeCollection<PlaceViewModel>();

        public List<Process> Items { get; set; } = new List<Process>();
        public List<ActionType> ActionTypeFilterItems { get; set; } = new List<ActionType>();

        public ICommand HeaderClickCommand { get; private set; }
        private bool _IsWorking { get; set; }
        private ActiveProcessesPageMode Mode { get; set; }
        private string _title { get; set; }
        public bool _HidePlanned { get; set; }
        public bool HidePlanned
        {
            get
            {
                return _HidePlanned;
            }
            set
            {
                if (value != _HidePlanned)
                {
                    _HidePlanned = value;
                    OnPropertyChanged();
                }
            }
        }

        public ActiveProcessesViewModel(ActiveProcessesPageMode _mode)
        {
            
            Mode = _mode;
            if (Mode == ActiveProcessesPageMode.UsersProcesses)
            {
                Title = "MOJE";
                HidePlanned = false;
            }
            else if (Mode == ActiveProcessesPageMode.AllProcesses)
            {
                Title = "WSZYSTKIE";
                HidePlanned = true;
            }
            else
            {
                Title = "KONSERWACJE";
                HidePlanned = false;
            }
            this.HeaderClickCommand = new Xamarin.Forms.Command<PlaceViewModel>((item) => ExecuteHeaderClickCommand(item));
        }

        

        public async Task<string> ExecuteLoadDataCommand()
        {
            string _Result = "OK";
            string url = "";
            string baseUrl = "";


            if (Mode == ActiveProcessesPageMode.UsersProcesses)
            {
                baseUrl = Secrets.ApiAddress + "GetUsersOpenProcesses?token=" + Secrets.TenantToken + $"&UserId={RuntimeSettings.UserId}";
                if (FilterString != null)
                {
                    url = $"{baseUrl}&query={FilterString}";
                }
                else
                {
                    url = baseUrl;
                }
            }
            else if(Mode == ActiveProcessesPageMode.AllProcesses)
            {
                baseUrl = Secrets.ApiAddress + "GetProcesses?token=" + Secrets.TenantToken + "&query=IsCompleted=false and IsSuccessfull=false";
                if (FilterString != null)
                {
                    HidePlanned = false;
                    url = $"{baseUrl} AND {FilterString}";
                }
                else
                {
                    url = baseUrl;
                }
            }
            else
            {
                baseUrl = Secrets.ApiAddress + "GetProcesses?token=" + Secrets.TenantToken + "&query=ActionTypeId=2 and IsCompleted=false and IsSuccessfull=false";
                if (FilterString != null)
                {
                    HidePlanned = false;
                    url = $"{baseUrl} AND {FilterString}";
                }
                else
                {
                    url = baseUrl;
                }
            }
            
            DataService ds = new DataService();
            Items = new List<Process>();   

            IsWorking = true;

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                HttpResponseMessage responseMsg = await Static.Functions.GetPostRetryAsync(()=> httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
                //var responseMsg = await httpClient.SendAsync(request);
                string output = await ds.readStream(responseMsg);
                if (responseMsg.IsSuccessStatusCode)
                {
                    Items = JsonConvert.DeserializeObject<List<Process>>(output);
                }
                else
                {
                    _Result = responseMsg.ReasonPhrase;
                }
                
            }
            catch (Exception ex)
            {
                IsWorking = false;  
                _Result = "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz";
                Static.Functions.CreateError(ex, "No connection", nameof(this.ExecuteLoadDataCommand), this.GetType().Name);
                throw;
            }
            PopulateListbox();
            
            IsWorking = false;
            return _Result;
        }

        public async void PopulateListbox()
        {
            if (List.Any())
            {
                //if there are any existent items, delete them
                List.Clear();
            }

            List<Place> Places;
            if (Items.Any())
            {
                Places = new List<Place>();

                if (HidePlanned)
                {
                    //get list of places
                    foreach (Process p in Items.Where(i=>i.LastStatus != ProcessStatus.Planowany))
                    {
                        int pId = p.PlaceId;

                        if (Places.Where(pl => pl.PlaceId == pId).Any())
                        {
                            //there's already such a place
                            Place nPlace = Places.Where(pl => pl.PlaceId == pId).FirstOrDefault();
                            nPlace.Processes.Add(p);
                        }
                        else
                        {
                            //no place like that yet. Have to create new one
                            Place nPlace = new Place();
                            nPlace.PlaceId = p.PlaceId;
                            nPlace.Name = p.PlaceName;
                            nPlace.SetId = p.SetId ?? 0;
                            nPlace.SetName = p.SetName;
                            nPlace.AreaId = p.AreaId ?? 0;
                            nPlace.AreaName = p.AreaName;
                            nPlace.Image = p.PlaceImage;
                            nPlace.Processes.Add(p);
                            Places.Add(nPlace);
                        }
                    }
                    if (Places.Any())
                    {
                        foreach (Place pl in Places)
                        {
                            PlaceViewModel vm = new PlaceViewModel(pl);
                            List.Add(vm);
                        }
                    }
                }
                else
                {
                    //get list of places
                    foreach (Process p in Items)
                    {
                        int pId = p.PlaceId;

                        if (Places.Where(pl => pl.PlaceId == pId).Any())
                        {
                            //there's already such a place
                            Place nPlace = Places.Where(pl => pl.PlaceId == pId).FirstOrDefault();
                            nPlace.Processes.Add(p);
                        }
                        else
                        {
                            //no place like that yet. Have to create new one
                            Place nPlace = new Place();
                            nPlace.PlaceId = p.PlaceId;
                            nPlace.Name = p.PlaceName;
                            nPlace.SetId = p.SetId ?? 0;
                            nPlace.SetName = p.SetName;
                            nPlace.AreaId = p.AreaId ?? 0;
                            nPlace.AreaName = p.AreaName;
                            nPlace.Image = p.PlaceImage;
                            nPlace.Processes.Add(p);
                            Places.Add(nPlace);
                        }
                    }
                    if (Places.Any())
                    {
                        foreach (Place pl in Places)
                        {
                            PlaceViewModel vm = new PlaceViewModel(pl);
                            List.Add(vm);
                        }
                    }
                }
                
            }
        }

        private void ExecuteHeaderClickCommand(PlaceViewModel item)
        {
            item.Expanded = !item.Expanded;
            if (!item.Expanded)
            {
                SelectedItem = null;
            }
        }

        public async void HidePlannedToggled()
        {
            PopulateListbox();
        }

        public bool IsWorking
        {
            get
            {
                return _IsWorking;
            }
            set
            {
                if (_IsWorking != value)
                {
                    if (value == false)
                    {
                        
                        if (PopupNavigation.Instance.PopupStack.Any()) { PopupNavigation.Instance.PopAllAsync(true); }  // Hide loading screen
                    }
                    else
                    {
                        PopupNavigation.Instance.PushAsync(new LoadingScreen(), true); // Show loading screen
                    }
                    _IsWorking = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if(_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        private ProcessViewModel _selectedItem { get; set; }
        public ProcessViewModel SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

    public enum ActiveProcessesPageMode
    {
        UsersProcesses,
        AllProcesses,
        MaintenanceOnly
    }
}
