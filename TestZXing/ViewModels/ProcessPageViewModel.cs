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

        public async Task Initialize(int AtId = -1, int PlId = -1)
        {
            try
            {
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
                    }
                    i++;
                }
                if (AtId >= 0 && index >=0)
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
                        if(p.PlaceId == PlId)
                        {
                            index = i;
                        }
                        i++;
                    }
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
                }
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

        public bool IsNotMesRelated
        {
            get
            {
                return !IsMesRelated;
            }
        }


        public bool IsClosable
        {
            get
            {
                if(!IsNew && IsOpen)
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
                if(_this.IsCompleted || _this.IsSuccessfull)
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
            if (_this.ActionTypeId == 0)
            {
                if (EndValidation)
                {
                    if (IsMesRelated)
                    {
                        if(_this.RepairActions.Length == 0)
                        {
                            _res = "Pole Czynności naprawcze nie może być puste. Uzupełnij opis czynności naprawczych!";
                        }else if (_this.InitialDiagnosis.Length == 0)
                        {
                            _res = "Pole Wstępna diagnoza nie może być puste. Uzupełnij opis wstępnej diagnozy!";
                        }else if(_this.PlaceId == 0)
                        {
                            _res = "Nie wybrano zasobu! Wybierz zasób z listy rozwijanej!";
                        }
                    }
                }
                else
                {
                    _res = "Nie wybrano typu zgłoszenia! Wybierz typ złgoszenia z listy rozwijanej!";
                }
            }
            if (_this.PlaceId == 0)
            {
                _res = "Nie wybrano zasobu! Wybierz zasób z listy rozwijanej!";
            }
            return _res;
        }

        public async Task<string> End(bool isSuccess = false)
        {
            string _Result = "OK";
 
            IsWorking = true;
            try
            {
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
