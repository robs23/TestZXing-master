using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ModernHttpClient;
using MvvmHelpers;
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
        public ObservableRangeCollection<PlaceViewModel> List { get; private set; }
        = new ObservableRangeCollection<PlaceViewModel>();

        public ICommand HeaderClickCommand { get; private set; }
        private bool _IsWorking { get; set; }
        private bool UProcesses { get; set; }
        private string _title { get; set; }

        public ActiveProcessesViewModel(bool UsersProcesses = false)
        {
            UProcesses = UsersProcesses;
            if (UProcesses)
            {
                Title = "MOJE";
            }
            else
            {
                Title = "WSZYSTKIE";
            }
            this.HeaderClickCommand = new Command<PlaceViewModel>((item) => ExecuteHeaderClickCommand(item));
        }

        public async Task<string> ExecuteLoadDataCommand()
        {
            string _Result = "OK";
            string url = "";

            if (UProcesses)
            {
                url = Secrets.ApiAddress + "GetUsersOpenProcesses?token=" + Secrets.TenantToken + $"&UserId={RuntimeSettings.UserId}";
            }
            else
            {
                url = Secrets.ApiAddress + "GetProcesses?token=" + Secrets.TenantToken + "&query=IsCompleted=false and IsSuccessfull=false";
            }
            
            DataService ds = new DataService();
            List<Process> Items = new List<Process>();
            List<Place> Places;

            IsWorking = true;

            if (List.Any())
            {
                //if there are any existent items, delete them
                List.Clear();
            }

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var responseMsg = await httpClient.SendAsync(request);
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
                throw;
            }

            if (Items.Any())
            {
                Places = new List<Place>();
 
                //get list of places
                foreach(Process p in Items)
                {
                    int pId = p.PlaceId;
                    
                    if (Places.Where(pl=>pl.PlaceId==pId).Any())
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
                        nPlace.SetId = (int)p.SetId;
                        nPlace.SetName = p.SetName;
                        nPlace.AreaId = (int)p.AreaId;
                        nPlace.AreaName = p.AreaName;
                        nPlace.Processes.Add(p);
                        Places.Add(nPlace);
                    }
                }
                if (Places.Any())
                {
                    foreach(Place pl in Places)
                    {
                        PlaceViewModel vm = new PlaceViewModel(pl);
                        List.Add(vm);
                    }
                }
            }
            IsWorking = false;
            return _Result;
        }

        private void ExecuteHeaderClickCommand(PlaceViewModel item)
        {
            item.Expanded = !item.Expanded;
            if (!item.Expanded)
            {
                SelectedItem = null;
            }
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
                        PopupNavigation.Instance.PopAsync(true); // Hide loading screen
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
