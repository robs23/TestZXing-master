using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;
using TestZXing.Static;

namespace TestZXing.ViewModels
{
    public class ProcessPageViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<ActionType> _actionTypes { get; set; }
        public ObservableCollection<ActionType> ActionTypes { get
            {
                return _actionTypes;
            }
            set
            {
                _actionTypes = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Place> _places { get; set; }
        public ObservableCollection<Place> Places
        {
            get
            {
                return _places;
            }
            set
            {
                _places = value;
                OnPropertyChanged();
            }
        }
        public bool IsSaved { get; set; }
        public bool _IsNew { get; set; }
        private bool _IsWorking { get; set; }
        private bool _IsMesRelated { get; set; }
        private bool _IsProcessOpen { get; set; } //there must be other open handligns for the process to be open
        private bool _RequireInitialDiagnosis { get; set; }
        public MesString MesString { get; set; }
        public Process _thisProcess { get; set; }
        public Handling _this { get; set; }
        public ProcessKeeper Processes = new ProcessKeeper();

        public ProcessPageViewModel(int PlaceId)
        {
            _thisProcess = new Process();
            _thisProcess.PlaceId = PlaceId;
            _this = new Handling();
            IsNew = true;
            IsProcessOpen = false; //not known till we have it checked
            //Initialize();


        }

        public ProcessPageViewModel(int PlaceId, Process Process)
        {
            _thisProcess = Process;
            _thisProcess.PlaceId = PlaceId;
            _this = new Handling();
            IsNew = false;
            IsProcessOpen = true;
            if (!string.IsNullOrEmpty(_thisProcess.MesId))
            {
                IsMesRelated = true;
                MesString = new MesString { SetName = _thisProcess.SetName, ActionTypeName = _thisProcess.ActionTypeName, MesId = _thisProcess.MesId, MesDate = _thisProcess.MesDate, Reason = _thisProcess.Reason };
            }
            //Initialize(_this.ActionTypeId);

        }

        public ProcessPageViewModel(MesString ms)
        {
            _thisProcess = new Process();
            _thisProcess.Reason = ms.Reason;
            _thisProcess.MesDate = ms.MesDate;
            _thisProcess.MesId = ms.MesId;
            _this = new Handling();
            IsNew = true;
            IsProcessOpen = false;
            IsMesRelated = true;
            MesString = ms;
        }

        public ProcessPageViewModel(MesString ms, Process process)
        {
            _thisProcess = process;
            _thisProcess.Reason = ms.Reason;
            _thisProcess.MesDate = ms.MesDate;
            _this = new Handling();
            IsNew = false;
            IsProcessOpen = true;
            IsMesRelated = true;
            MesString = ms;
        }

        public async Task Initialize(int AtId = -1)
        {
            try
            {
                //if IsMesRelated then first 'get' scanned action type just to make sure she'll be added to the list if missing
                ActionType nAt = new ActionType();
                if (IsMesRelated)
                {
                    nAt = await new ActionTypesKeeper().GetActionTypeByName(MesString.ActionTypeName);
                }
                
                //load action types to combobox
                int index=-1;
                int i = 0;
                _selectedIndex = -1;
                ActionTypes = new ObservableCollection<ActionType>();
                ActionTypesKeeper keeper = new ActionTypesKeeper();
                await keeper.Reload();
                foreach (ActionType at in keeper.Items)
                {
                    ActionTypes.Add(at);
                    if (at.ActionTypeId == AtId)
                    {
                        index = i;
                    }else if (IsMesRelated)
                    {
                        if (at.ActionTypeId == nAt.ActionTypeId)
                        {
                            index = i;
                        }
                    }
                    i++;
                }
                if ((AtId >= 0 && index >=0) || (IsMesRelated && index >=0))
                {
                    SelectedIndex = index;
                }

                //load places to combobox
                if (_IsMesRelated)
                {
                    index = -1;
                    i = 0;
                    _selectedPlaceIndex = -1;
                    Places = new ObservableCollection<Place>();
                    PlacesKeeper pKeeper = new PlacesKeeper();
                    List<Place> _p = new List<Place>();
                    _p = await pKeeper.GetPlacesBySetName(MesString.SetName);
                    foreach(Place p in _p)
                    {
                        Places.Add(p);
                        if(p.PlaceId == _thisProcess.PlaceId)
                        {
                            index = i;
                        }
                        i++;
                    }
                }
                if (IsMesRelated && index >= 0)
                {
                    SelectedPlaceIndex = index;
                }
                
                
            }
            catch(Exception ex)
            {
                throw;
            }
            
        }

        public string Description {
            get
            {
                if (IsMesRelated)
                {
                    return _thisProcess.MesId;
                }
                else
                {
                    return _thisProcess.PlaceName;
                }
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
                    _IsWorking = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsIdle));
                    OnPropertyChanged(nameof(IsOpen));
                    OnPropertyChanged(nameof(IsClosable));
                }
            }
        }

        public bool IsIdle
        {
            get
            {
                return !_IsWorking;
            }
        }

        public bool IsMesRelated
        {
            get
            {
                return _IsMesRelated;
            }
            set
            {
                if(_IsMesRelated != value)
                {
                    _IsMesRelated = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsProcessOpen
        {
            get
            {
                return _IsProcessOpen;
            }
            set
            {
                if(_IsProcessOpen != value)
                {
                    _IsProcessOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool RequireInitialDiagnosis
        {
            get
            {
                return _RequireInitialDiagnosis;
            }
            set
            {
                if (_RequireInitialDiagnosis != value)
                {
                    _RequireInitialDiagnosis = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DoesntRequireInitialDiagnosis));
                }
            }
        }

        public bool DoesntRequireInitialDiagnosis
        {
            get
            {
                return !RequireInitialDiagnosis;
            }
        }

        public bool IsClosable
        {
            get
            {
                if(!IsNew && IsOpen && IsIdle)
                {
                    return true;
                }
                else
                {
                    return false;
                }
               
            }
        }

        public bool IsNew
        {
            get
            {
                return _IsNew;
            }
            set
            {
                if (_IsNew != value)
                {
                    _IsNew = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsClosable));
                }
            }
        }

        public string Status {
            get
            {
                return _thisProcess.Status;
            }
            set
            {
                if(_thisProcess.Status != value)
                {
                    _thisProcess.Status = value;
                    OnPropertyChanged();
                }
            }
        }


        public string Output {
            get
            {
                return _thisProcess.Output;
            }
            set
            {
                if(_thisProcess.Output != value)
                {
                    _thisProcess.Output = value;
                    OnPropertyChanged();
                }
            }
        }

        public string InitialDiagnosis
        {
            get
            {
                return _thisProcess.InitialDiagnosis;
            }
            set
            {
                if(_thisProcess.InitialDiagnosis != value)
                {
                    _thisProcess.InitialDiagnosis = value;
                    OnPropertyChanged();
                }
            }
        }

        public string RepairActions
        {
            get
            {
                return _thisProcess.RepairActions;
            }
            set
            {
                if (_thisProcess.RepairActions != value)
                {
                    _thisProcess.RepairActions = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _selectedIndex { get; set; }

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                try
                {
                    if (_selectedIndex != value)
                    {
                        _selectedIndex = value;
                        _thisProcess.ActionTypeId = ActionTypes[value].ActionTypeId;
                        Type = ActionTypes[value];
                        if ((bool)!Type.AllowDuplicates) //check if there's open process of this type ONLY if AllowDuplicates property = false
                        {
                            Process nProcess = null;
                            Task.Run(async () =>
                            {
                                nProcess = await Processes.GetOpenProcessesOfTypeAndResource(_thisProcess.ActionTypeId, _thisProcess.PlaceId);
                                if (nProcess != null)
                                {
                                    //there's open process of this type on the resource, let's use it!
                                    _thisProcess = nProcess;
                                    IsProcessOpen = true;
                                    HandlingKeeper Handlings = new HandlingKeeper();
                                    Handling nHandling = await Handlings.GetUsersOpenHandling(_thisProcess.ProcessId);
                                    if (nHandling == null)
                                    {
                                        //User has no open handlings in this Process, we're creating new one (not editing)
                                        IsNew = true;
                                        _this = new Handling()
                                        {
                                            PlaceId = _thisProcess.PlaceId
                                        };
                                    }
                                    else
                                    {
                                        _this = nHandling;
                                        IsNew = false;
                                        OnPropertyChanged(nameof(NextState));
                                    }
                                }
                            });
                        }
                        else
                        {
                            IsNew = true;
                            _this = new Handling()
                            {
                                PlaceId = _thisProcess.PlaceId
                            };
                            OnPropertyChanged(nameof(NextState));
                        }

                        if ((bool)ActionTypes[value].RequireInitialDiagnosis)
                        {
                            //if chosen action type has RequireInitialDiagnosis=true, change the property so the bound view is changed
                            RequireInitialDiagnosis = true;
                        }
                        else
                        {
                            RequireInitialDiagnosis = false;
                        }
                        OnPropertyChanged();
                    }
                }catch(Exception ex)
                {
                    Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "SelectedIndex", Time = DateTime.Now, Message =  ex.Message };
                    Error.Add();
                }
            }
        }

        private int _selectedPlaceIndex { get; set; }

        public int SelectedPlaceIndex
        {
            get
            {
                return _selectedPlaceIndex;
            }
            set
            {
                try
                {
                    if(_selectedPlaceIndex != value)
                    {
                        _selectedPlaceIndex = value;
                        _thisProcess.PlaceId = Places[value].PlaceId;
                        Place = Places[value];
                        OnPropertyChanged();
                    }
                }catch(Exception ex)
                {
                    Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "SelectedIndex", Time = DateTime.Now, Message = ex.Message };
                    Error.Add();
                }
            }
        }

        private ActionType _type { get; set; }

        public ActionType Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged();
                }
            }
        }

        private Place _place { get; set; }

        public Place Place
        {
            get
            {
                return _place;
            }
            set
            {
                if(_place != value)
                {
                    _place = value;
                    OnPropertyChanged();
                }
            }
        }

        public string NextState
        {
            get
            {
                if (_this.Status == "Planowany")
                {
                    return "Rozpocznij";
                }else if(_this.Status == "Rozpoczęty")
                {
                    return "Wstrzymaj";
                }else if(_this.Status == "Wstrzymany")
                {
                    return "Wznów";
                }else
                {
                    return "Zakończony";
                }
            }
        }


        public bool IsOpen
        {
            get
            {
                if(_this.IsCompleted || IsWorking)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public async Task<string> Save()
        {
            string _Result = "OK";

            IsWorking = true;
            try
            {
                // Taking care of process
                
                if (!this.IsProcessOpen)
                {
                    //if the process doesn't exist yet, open it now
                    _thisProcess.CreatedBy = RuntimeSettings.UserId;
                    _thisProcess.TenantId = RuntimeSettings.TenantId;
                    _thisProcess.StartedBy = RuntimeSettings.UserId;
                    _thisProcess.StartedOn = DateTime.Now;
                    _thisProcess.Status = "Rozpoczęty";
                    _thisProcess.CreatedOn = DateTime.Now;
                    _Result = await _thisProcess.Add();
                    IsProcessOpen = true;
                }
                else
                {
                    // There must be new process edit logic..
                    if (_thisProcess.Status == "Wstrzymany" || _thisProcess.Status == "Planowany")
                    {
                        _thisProcess.Status = "Rozpoczęty";
                    }
                    _Result = await _thisProcess.Edit();
                }

                // Taking care of handling
                if (_Result == "OK")
                {
                    if (this.IsNew)
                    {
                        //this handling is completely new, create it
                        //But first, let's make sure User don't have any other open handligs elsewhere. If he does, let's complete them first
                        HandlingKeeper Handlings = new HandlingKeeper();
                        _Result = await Handlings.CompleteUsersHandlings();
                        if (_Result == "OK")
                        {
                            _this.StartedOn = DateTime.Now;
                            _this.UserId = RuntimeSettings.UserId;
                            _this.TenantId = RuntimeSettings.TenantId;
                            _this.Status = "Rozpoczęty";
                            _this.ProcessId = _thisProcess.ProcessId;
                            _Result = await _this.Add();
                            IsNew = false;
                            OnPropertyChanged(nameof(NextState));
                        }
                    }
                    else
                    {
                        if (_this.Status == "Rozpoczęty")
                        {
                            _this.Status = "Wstrzymany";
                        }
                        else if (_this.Status == "Wstrzymany" || _this.Status == "Planowany")
                        {
                            _this.Status = "Rozpoczęty";
                        }
                        _Result = await _this.Edit();
                        OnPropertyChanged(nameof(NextState));
                    }
                }
            }
            catch (Exception ex)
            {
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "Save", Time = DateTime.Now, Message = ex.Message };
                await Error.Add();
            }
            IsWorking = false;
            return _Result;
            
        }

        public string Validate(bool EndValidation = false)
        {
            string _res = "OK";
            if (EndValidation)
            {
                if (RequireInitialDiagnosis)
                {
                    if (string.IsNullOrEmpty(_thisProcess.InitialDiagnosis))
                    {
                        _res = "Pole Wstępne rozpoznanie nie może być puste. Uzupełnij wstępne rozpoznanie!";
                    }
                    else if (string.IsNullOrEmpty(_thisProcess.RepairActions))
                    {
                        _res = "Pole Czynności naprawcze nie może być puste. Uzupełnij opis czynności naprawczych!";
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(this.Output))
                    {
                        _res = "Pole Rezultat nie może być puste! Opisz co udało się zrobić.";
                    }
                }
                if (IsMesRelated)
                {
                    if (_thisProcess.PlaceId == 0)
                    {
                        _res = "Nie wybrano zasobu! Wybierz zasób z listy rozwijanej!";
                    }
                }
                if (_thisProcess.ActionTypeId == 0)
                {
                    _res = "Nie wybrano typu zgłoszenia! Wybierz typ złgoszenia z listy rozwijanej!";
                }
            }
            else
            {
                if (RequireInitialDiagnosis)
                {
                    if (string.IsNullOrEmpty(_thisProcess.InitialDiagnosis))
                    {
                        _res = "Pole Wstępne rozpoznanie nie może być puste. Uzupełnij wstępne rozpoznanie!";
                    }
                }
                if (IsMesRelated)
                {
                    if (_thisProcess.PlaceId == 0)
                    {
                        _res = "Nie wybrano zasobu! Wybierz zasób z listy rozwijanej!";
                    }
                }
                if (_thisProcess.ActionTypeId == 0)
                {
                    _res = "Nie wybrano typu zgłoszenia! Wybierz typ złgoszenia z listy rozwijanej!";
                }
            }
            
            
            return _res;
        }

        public async Task<string> End(bool toClose = false, bool toPause = false)
        {
            string _Result = "OK";
 
            IsWorking = true;
            try
            {
                string prevStatus = _thisProcess.Status;
                if (toClose)
                {
                    _thisProcess.FinishedOn = DateTime.Now;
                    _thisProcess.FinishedBy = RuntimeSettings.UserId;
                    _thisProcess.Status = "Zakończony";
                    _Result = await _thisProcess.Edit();
                    if (!_Result.Equals("OK"))
                    {
                        _thisProcess.Status = prevStatus;
                    }
                }
                else
                {
                    if (toPause)
                    {
                        _thisProcess.Status = "Wstrzymany";
                        _Result = await _thisProcess.Edit();
                        if (!_Result.Equals("OK"))
                        {
                            _thisProcess.Status = prevStatus;
                        }
                    }
                }

                // Close handling now
                prevStatus = _this.Status;

                _this.FinishedOn = DateTime.Now;
                _this.Status = "Zakończony";
                _Result = await _this.Edit();
                if (!_Result.Equals("OK"))
                {
                    _this.Status = prevStatus;
                }

                OnPropertyChanged(nameof(NextState));
                OnPropertyChanged(nameof(IsOpen));
                OnPropertyChanged(nameof(IsClosable));
            }
            catch (Exception ex)
            {
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "Save", Time = DateTime.Now, Message = ex.Message };
                await Error.Add();
            }
            IsWorking = false;
            return _Result;


        }

        public async Task<string> AreThereOpenHandlingsLeft()
        {
            string _Result = "No";
            IsWorking = true;

            try
            {
                List<Handling> OpenHandligs = null;
                OpenHandligs = await _thisProcess.GetOpenHandlings();
                if(OpenHandligs.Any())
                {
                    if(OpenHandligs.Count > 1)
                    {
                        //there are some open handlings left apart from the current one. Closing this handling won't impact the process then
                        _Result = "Yes";
                    }
                }
            }
            catch (Exception ex)
            {
                _Result = ex.Message;
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "Save", Time = DateTime.Now, Message = ex.Message };
                await Error.Add();
            }
            IsWorking = false;
            return _Result;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
