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
using TestZXing.Classes;
using TestZXing.Models;
using TestZXing.Static;
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class ActiveProcessesViewModel
    {
        public ObservableRangeCollection<PlaceViewModel> List { get; private set; }
        = new ObservableRangeCollection<PlaceViewModel>();

        public ICommand LoadDataCommand { get; private set; }
        public ICommand HeaderClickCommand { get; private set; }
        private bool _IsWorking { get; set; }

        public ActiveProcessesViewModel()
        {
            this.LoadDataCommand = new Command(async () => await ExecuteLoadDataCommand());
            this.HeaderClickCommand = new Command<PlaceViewModel>((item) => ExecuteHeaderClickCommand(item));
        }

        private async Task ExecuteLoadDataCommand()
        {
            string url = RuntimeSettings.ApiAddress + "GetProcessesExt?token=" + RuntimeSettings.TenantToken;
            DataService ds = new DataService();
            List<Process> Items;
            List<Place> Places;

            IsWorking = true;

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                string output = await ds.readStream(await httpClient.SendAsync(request));
                Items = JsonConvert.DeserializeObject<List<Process>>(output);
            }
            catch (Exception ex)
            {
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
                IsWorking = false;
            }

        }

        private void ExecuteHeaderClickCommand(PlaceViewModel item)
        {
            item.Expanded = !item.Expanded;
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
                    _IsWorking = value;
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
