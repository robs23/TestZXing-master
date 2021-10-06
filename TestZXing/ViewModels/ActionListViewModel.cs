using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestZXing.Interfaces;
using TestZXing.Models;
using TestZXing.Static;
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class ActionListViewModel : INotifyPropertyChanged
    {
        private readonly NLog.ILogger Logger = NLog.LogManager.GetCurrentClassLogger();
        public ActionListViewModel(int ProcessId, int PlaceId)
        {
            this.ProcessId = ProcessId;
            this.PlaceId = PlaceId;
            ProcessActionKeeper = new ProcessActionKeeper();
            PlaceActionKeeper = new PlaceActionKeeper();
        }

        public ActionListViewModel(int PlaceId)
        {
            this.PlaceId = PlaceId;
            ProcessActionKeeper = new ProcessActionKeeper();
            PlaceActionKeeper = new PlaceActionKeeper();
        }

        public async Task<bool> Initialize()
        {
            //get all the placeActions of this placeId and processActions for this process
            //and fill the list in

            try
            {
                CheckedItems = new ObservableCollection<ProcessAction>();
                Items = new ObservableCollection<IActionKeeper>();

                if (ProcessId != 0)
                {
                    var processReload = Task.Run(() => ProcessActionKeeper.Reload($"ProcessId={ProcessId}"));
                    var placeReload = Task.Run(() => PlaceActionKeeper.Reload($"PlaceId={PlaceId}"));

                    //go no further till both tasks complete
                    await Task.WhenAll(processReload, placeReload);

                    CheckedItems = new ObservableCollection<ProcessAction>(ProcessActionKeeper.Items);
                    Items = new ObservableCollection<IActionKeeper>(CheckedItems);
                    foreach (var item in Items)
                    {
                        if ((bool)item.IsChecked)
                        {
                            //if item has been saved as checked, don't let anyone change it
                            item.IsMutable = false;
                        }
                    }
                }
                else
                {
                    var placeReload = Task.Run(() => PlaceActionKeeper.Reload($"PlaceId={PlaceId}"));

                    //go no further till task complete
                    await Task.WhenAll(placeReload);
                }


                foreach (PlaceAction p in PlaceActionKeeper.Items)
                {
                    if (!CheckedItems.Any(i => i.ActionId == p.ActionId))
                    {
                        Items.Add(p);
                    }

                }
                IsInitialized = true;
                Task.Run(() => TakeSnapshot());
                if (Items.Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private bool _IsInitialized { get; set; }

        public bool IsInitialized
        {
            get
            {
                return _IsInitialized;
            }
            set
            {
                if (value != _IsInitialized)
                {
                    _IsInitialized = value;
                }
            }
        }

        public async Task<bool> IsDirty()
        {
            bool res = false;
            if(CheckedItems != null && SavedItems != null)
            {
                int checkedItems = CheckedItems.Where(i => i.IsChecked == true).Count();
                int savedItems = SavedItems.Count(i => i.IsChecked == true);
                if (checkedItems > savedItems)
                {
                    res = true;
                }
                else
                {
                    foreach (ProcessAction pa in CheckedItems.Where(i => i.IsChecked == true))
                    {
                        if (pa.ProcessActionId == 0)
                        {
                            //it's action user checked voluntarily
                            //on the whole, items without id definitely haven't been saved yet
                            res = true;
                            break;
                        }
                        else if (SavedItems.Any(i => i.ProcessActionId == pa.ProcessActionId && i.IsChecked == false))
                        {
                            //there's at least 1 item that IsChecked=true in CheckedItems that wasn't checked in saved items

                            res = true;
                            break;
                        }
                    }
                }
            }
            
            return res;
        }

        public async Task TakeSnapshot()
        {
            SavedItems = new ObservableCollection<ProcessAction>();
            SavedItems = this.CheckedItems.CloneJson<ObservableCollection<ProcessAction>>();
        }

        public PlaceActionKeeper PlaceActionKeeper { get; set; }
        public ProcessActionKeeper ProcessActionKeeper { get; set; }

        private int _ProcessId { get; set; }
        public int ProcessId
        {
            get
            {
                return _ProcessId;
            }
            set
            {
                if (value != _ProcessId)
                {
                    _ProcessId = value;
                }
            }
        }

        private int _PlaceId { get; set; }
        public int PlaceId
        {
            get
            {
                return _PlaceId;
            }
            set
            {
                if (value != _PlaceId)
                {
                    _PlaceId = value;
                }
            }
        }

        private bool _isWorking { get; set; }

        public bool IsWorking
        {
            get
            {
                return _isWorking;
            }
            set
            {
                if (_isWorking != value)
                {
                    if (value == false)
                    {
                        if (PopupNavigation.Instance.PopupStack.Any()) { PopupNavigation.Instance.PopAllAsync(true); }  // Hide loading screen
                    }
                    else
                    {
                        PopupNavigation.Instance.PushAsync(new LoadingScreen(), true); // Show loading screen
                    }
                    _isWorking = value;
                    OnPropertyChanged();
                }
            }
        }


        private ObservableCollection<IActionKeeper> _items { get; set; }
        public ObservableCollection<IActionKeeper> Items
        {
            get
            {
                return _items;
            }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ProcessAction> _checkedItems { get; set; }
        public ObservableCollection<ProcessAction> CheckedItems
        {
            get
            {
                return _checkedItems;
            }
            set
            {
                if (_checkedItems != value)
                {
                    _checkedItems = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ProcessAction> _savedItems { get; set; }
        public ObservableCollection<ProcessAction> SavedItems
        {
            get
            {
                return _savedItems;
            }
            set
            {
                if (value != _savedItems)
                {
                    _savedItems = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Validate()
        {
            string _res = "OK";

            foreach (IActionKeeper i in Items)
            {
                if (i.IsRequired && !(bool)i.IsChecked)
                {
                    //at least 1 required action hasn't been checked
                    _res = "ActionListViewModelSkippable";
                    break;
                }
            }

            return _res;
        }

        public async Task<string> Save(int handlingId, int processId, int? abandonReason=null)
        {
            try
            {
                Logger.Info("ActionListViewModel is starting. HandlingId={handlingId}, ProcessId={processId}, AbandonReason={abandonReason}",handlingId, processId, abandonReason);

                List<Task<string>> listOfTask = new List<Task<string>>();
                foreach (ProcessAction pa in CheckedItems.Where(i => i.IsMutable == true))
                {
                    Logger.Info("ProcessAction: ID={ProcessActionId}, IsChecked={IsChecked}, IsMutable={IsMutable}", pa.ProcessActionId, pa.IsChecked, pa.IsMutable);
                    pa.HandlingId = handlingId;
                    pa.ProcessId = processId;
                    if (pa.IsChecked == false || pa.IsChecked == null)
                    {
                        pa.AbandonReasonId = abandonReason;
                    }

                    if (pa.IsMutable == true && (pa.IsChecked == true || (pa.IsChecked == false && abandonReason != null)))
                    {
                        //save changes to process action only when it's mutable and:
                        //- it's checked
                        //- it's not checked and the user provided a reason why he abandoned it

                        if (pa.ProcessActionId == 0)
                        {
                            listOfTask.Add(pa.Add());
                        }
                        else
                        {
                            listOfTask.Add(pa.Edit());
                        }
                    }
                }

                IEnumerable<string> results = await Task.WhenAll<string>(listOfTask);
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
            catch(InvalidCastException ex)
            {
                Logger.Error(ex);
                string additionalInfo = $"handlingId={handlingId}, processId={processId}, abandonReason={abandonReason}";
                Static.Functions.CreateError(ex, "InvalidCast", nameof(this.Save), this.GetType().Name, additionalInfo);
                return ex.Message;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                string additionalInfo = $"handlingId={handlingId}, processId={processId}, abandonReason={abandonReason}";
                Static.Functions.CreateError(ex, "No connection", nameof(this.Save), this.GetType().Name, additionalInfo);
                return ex.Message;
            }

        }

        public bool IsSaved
        {
            get
            {
                return CheckedItems.Any(i => i.IsSaved == false);//if any item is not saved, then whole ViewModel is not saved
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
