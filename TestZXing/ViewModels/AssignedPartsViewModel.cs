using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestZXing.Classes;
using TestZXing.Interfaces;
using TestZXing.Models;
using TestZXing.Static;
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class AssignedPartsViewModel : BaseViewModel
    {
        public PartKeeper PartKeeper { get; set; }
        public PartUsageKeeper PartUsageKeeper { get; set; }

        public AssignedPartsViewModel()
        {
            ScanCommand = new AsyncCommand(Scan);
            SearchCommand = new AsyncCommand(Search);
            RemoveItemsCommand = new AsyncCommand(RemoveItems);
            Items = new ObservableRangeCollection<PartUsage>();
            SelectedItems = new ObservableRangeCollection<PartUsage>();
            PartKeeper = new PartKeeper();
            PartUsageKeeper = new PartUsageKeeper();
        }

        public async Task Initialize(int? processId=null)
        {
            base.Initialize();
            PartsPageViewModel = new PartsPageViewModel();
            PartsPageViewModel.Initialize();
            PartsPageViewModel.Mode = PartsPageMode.PartsPicker;
            if (processId != null)
            {
                await PartUsageKeeper.Reload($"ProcessId={processId} and CreatedBy={RuntimeSettings.CurrentUser.UserId}");
                Items = new ObservableRangeCollection<PartUsage>(PartUsageKeeper.Items);
                Task.Run(() => TakeSnapshot());
            }
        }

        public async Task TakeSnapshot()
        {
            //remember what you've already saved
            SavedItems = this.Items.CloneJson<ObservableRangeCollection<PartUsage>>();
        }

        public async Task<bool> IsDirty()
        {
            bool res = false;
            if(Items != null && SavedItems != null)
            {
                if (Items.Count() > SavedItems.Count())
                {
                    res = true;
                }
                else
                {
                    foreach (PartUsage pu in Items)
                    {
                        if (pu.PartUsageId == 0)
                        {
                            //on the whole, items without id definitely haven't been saved yet
                            res = true;
                            break;
                        }
                        else if (!SavedItems.Any(i => i.PartUsageId == pu.PartUsageId))
                        {
                            //Current item hasn't been saved yet
                            res = true;
                            break;
                        }
                        else if (SavedItems.Any(i => i.PartUsageId == pu.PartUsageId && (i.Amount != pu.Amount || i.Comment != pu.Comment)))
                        {
                            //Either Amount or Comment has changed for at least single item
                            res = true;
                            break;
                        }
                    }
                    foreach (PartUsage pu in SavedItems)
                    {
                        if (!Items.Any(i => i.PartUsageId == pu.PartUsageId))
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

        public async Task Update()
        {
            if (PartsPageViewModel.SelectedItems.Count > 0)
            {
                //add new item and clear selectedItems
                foreach(Part p in PartsPageViewModel.SelectedItems)
                {
                    PartUsage pu = new PartUsage
                    {
                        PartId = p.PartId,
                        Name = p.Name,
                        Symbol = p.Symbol,
                        ProducerId = p.ProducerId,
                        ProducerName = p.ProducerName,
                        Description = p.Description,
                        Image = p.Image,
                        Amount = 1
                    };
                    Items.Add(pu);
                    
                }
                //OnPropertyChanged(nameof(Items));
                PartsPageViewModel.SelectedItems.Clear();
            }
        }

        public PartsPageViewModel PartsPageViewModel { get; set; }

        

        public async Task Scan()
        {
            try
            {
                QrHandler qr = new QrHandler();
                string res = await qr.Scan();
                res = res.Replace("<Part>", "");
                Part p = await PartKeeper.GetByToken(res);
                if (p == null)
                {
                    DependencyService.Get<IToaster>().ShortAlert($"Część o kodzie {res} nie została znaleziona!");
                }
                else
                {
                    PartUsage pu = new PartUsage
                    {
                        PartId = p.PartId,
                        Name = p.Name,
                        Symbol = p.Symbol,
                        ProducerId = p.ProducerId,
                        ProducerName = p.ProducerName,
                        Description = p.Description,
                        Image = p.Image,
                        Amount = 1
                    };
                    Items.Add(pu);
                }
                

                
            }
            catch (Exception ex)
            {
                DependencyService.Get<IToaster>().ShortAlert($"Error: {ex.Message}");
            }
        }

        public ICommand ScanCommand { get; }

        public ICommand SearchCommand { get; }

        private async Task Search()
        {
            Application.Current.MainPage.Navigation.PushAsync(new PartsPage(PartsPageViewModel));
        }

        public async Task<string> Save(int processId, int placeId)
        {
            string res = "OK";

            if (res=="OK")
            {
                List<Task<string>> SaveTasks = new List<Task<string>>();

                foreach (PartUsage pu in Items)
                {
                    pu.ProcessId = processId;
                    pu.PlaceId = placeId;
                    if (pu.PartUsageId == 0)
                    {
                        pu.CreatedBy = RuntimeSettings.CurrentUser.UserId;
                        SaveTasks.Add(pu.Add());
                    }
                    else
                    {
                        //if (!pu.IsSaved)
                        //{
                            SaveTasks.Add(pu.Edit());
                        //}

                    }
                }
                if (RemovedItems.Any())
                {
                    foreach (PartUsage pu in RemovedItems)
                    {
                        SaveTasks.Add(pu.Remomve());
                    }
                }

                IEnumerable<string> results = await Task.WhenAll<string>(SaveTasks);
                for (int i = RemovedItems.Count; i > 0; i--)
                {
                    if (RemovedItems[i-1].IsSaved)
                    {
                        RemovedItems.Remove(RemovedItems[i - 1]);
                    }
                }
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
                foreach (PartUsage pu in Items)
                {
                    if (string.IsNullOrEmpty(pu.Comment))
                    {
                        res = $"Podaj jaka ilość części \"{pu.Name}\" pozostała na zapasie..";
                        break;
                    }
                }
            }catch(Exception ex)
            {
                res = $"Wystąpił nieoczekiwany błąd. {ex.Message}";
            }
            
            return res;
        }

        public ICommand RemoveItemsCommand { get; }

        private async Task RemoveItems()
        {
            if(SelectedItems.Count > 0)
            {
                for (int i = SelectedItems.Count; i>0; i--)
                {
                    if (SelectedItems[i - 1].PartUsageId >0)
                    {
                        //if saved, add it to RemovedItems collection for further removal
                        RemovedItems.Add(SelectedItems[i - 1]);
                    }
                    Items.Remove(SelectedItems[i - 1]);
                    SelectedItems.Remove(SelectedItems[i-1]);
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

        ObservableRangeCollection<PartUsage> _Items;

        public ObservableRangeCollection<PartUsage> Items
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

        ObservableRangeCollection<PartUsage> _SavedItems;

        public ObservableRangeCollection<PartUsage> SavedItems
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

        ObservableRangeCollection<PartUsage> _SelectedItems = new ObservableRangeCollection<PartUsage>();

        public ObservableRangeCollection<PartUsage> SelectedItems
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

        ObservableRangeCollection<PartUsage> _RemovedItems = new ObservableRangeCollection<PartUsage>();

        public ObservableRangeCollection<PartUsage> RemovedItems
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
                    return "Nie użyto żadnej części do tego zgłoszenia. Dodaj pierwszą część korzystając z przycisków poniżej. Przycisk LUPY pozwala wyszukać część po nazwie/symbolu, przycisk QR pozwala dodać część skanując jej kod QR";
                }
            }
        }

        public bool IsSaved
        {
            get
            {
                return Items.Any(i => i.IsSaved == false);//if any item is not saved, then whole ViewModel is not saved
            }
        }
    }
}
