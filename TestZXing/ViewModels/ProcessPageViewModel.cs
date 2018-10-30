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
        private bool _RequireInitialDiagnosis { get; set; }
        public MesString MesString { get; set; }
        public Process _this { get; set; }

        public ProcessPageViewModel(int PlaceId)
        {
            _this = new Process();
            _this.PlaceId = PlaceId;
            IsNew = true;
            //Initialize();


        }

        public ProcessPageViewModel(int PlaceId, Process Process)
        {
            _this = Process;
            _this.PlaceId = PlaceId;
            IsNew = false;
            if (!string.IsNullOrEmpty(_this.MesId))
            {
                IsMesRelated = true;
                MesString = new MesString { SetName = _this.SetName, ActionTypeName = _this.ActionTypeName, MesId = _this.MesId, MesDate = _this.MesDate, Reason = _this.Reason };
            }
            //Initialize(_this.ActionTypeId);

        }

        public ProcessPageViewModel(MesString ms)
        {
            _this = new Process();
            _this.Reason = ms.Reason;
            _this.MesDate = ms.MesDate;
            _this.MesId = ms.MesId;
            IsNew = true;
            IsMesRelated = true;
            MesString = ms;
        }

        public ProcessPageViewModel(MesString ms, Process process)
        {
            _this = process;
            _this.Reason = ms.Reason;
            _this.MesDate = ms.MesDate;
            IsNew = false;
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
                        if(p.PlaceId == _this.PlaceId)
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
                return _this.Description;
            }
            set
            {
                if (_this.Description != value)
                {
                    _this.Description = value;
                    OnPropertyChanged();
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
                return _this.Status;
            }
            set
            {
                if(_this.Status != value)
                {
                    _this.Status = value;
                    OnPropertyChanged();
                }
            }
        }


        public string Output {
            get
            {
                return _this.Output;
            }
            set
            {
                if(_this.Output != value)
                {
                    _this.Output = value;
                    OnPropertyChanged();
                }
            }
        }

        public string InitialDiagnosis
        {
            get
            {
                return _this.InitialDiagnosis;
            }
            set
            {
                if(_this.InitialDiagnosis != value)
                {
                    _this.InitialDiagnosis = value;
                    OnPropertyChanged();
                }
            }
        }

        public string RepairActions
        {
            get
            {
                return _this.RepairActions;
            }
            set
            {
                if (_this.RepairActions != value)
                {
                    _this.RepairActions = value;
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
                        _this.ActionTypeId = ActionTypes[value].ActionTypeId;
                        Type = ActionTypes[value];
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
                        _this.PlaceId = Places[value].PlaceId;
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
                if (_this.Status == "Nierozpoczęty")
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
                if(_this.IsCompleted || _this.IsSuccessfull || IsWorking)
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
                if (this.IsNew)
                {
                    _this.CreatedBy = RuntimeSettings.UserId;
                    _this.TenantId = RuntimeSettings.TenantId;
                    _this.StartedBy = RuntimeSettings.UserId;
                    _this.StartedOn = DateTime.Now;
                    _this.Status = "Rozpoczęty";
                    _this.CreatedOn = DateTime.Now;
                    _Result = await _this.Add();
                    IsNew = false;
                    OnPropertyChanged(nameof(NextState));
                }
                else
                {
                    if (_this.Status == "Rozpoczęty")
                    {
                        _this.Status = "Wstrzymany";
                    }else if(_this.Status == "Wstrzymany" || _this.Status == "Nierozpoczęty")
                    {
                        _this.Status = "Rozpoczęty";
                    }
                    _Result = await _this.Edit();
                    OnPropertyChanged(nameof(NextState));
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
                    if (string.IsNullOrEmpty(_this.InitialDiagnosis))
                    {
                        _res = "Pole Wstępne rozpoznanie nie może być puste. Uzupełnij wstępne rozpoznanie!";
                    }
                    else if (string.IsNullOrEmpty(_this.RepairActions))
                    {
                        _res = "Pole Czynności naprawcze nie może być puste. Uzupełnij opis czynności naprawczych!";
                    }
                }
                if (IsMesRelated)
                {
                    if (_this.PlaceId == 0)
                    {
                        _res = "Nie wybrano zasobu! Wybierz zasób z listy rozwijanej!";
                    }
                }
                if (_this.ActionTypeId == 0)
                {
                    _res = "Nie wybrano typu zgłoszenia! Wybierz typ złgoszenia z listy rozwijanej!";
                }
            }
            else
            {
                if (RequireInitialDiagnosis)
                {
                    if (string.IsNullOrEmpty(_this.InitialDiagnosis))
                    {
                        _res = "Pole Wstępne rozpoznanie nie może być puste. Uzupełnij wstępne rozpoznanie!";
                    }
                }
                if (IsMesRelated)
                {
                    if (_this.PlaceId == 0)
                    {
                        _res = "Nie wybrano zasobu! Wybierz zasób z listy rozwijanej!";
                    }
                }
                if (_this.ActionTypeId == 0)
                {
                    _res = "Nie wybrano typu zgłoszenia! Wybierz typ złgoszenia z listy rozwijanej!";
                }
            }
            
            
            return _res;
        }

        public async Task<string> End(bool isSuccess = false)
        {
            string _Result = "OK";
 
            IsWorking = true;
            try
            {
                string prevStatus = _this.Status;
                if (isSuccess)
                {
                    _this.Status = "Zrealizowany";
                }
                else
                {
                    _this.Status = "Zakończony";
                }
                _this.FinishedOn = DateTime.Now;
                _this.FinishedBy = RuntimeSettings.UserId;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
