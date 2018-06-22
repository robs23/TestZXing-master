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
        public ObservableCollection<ActionType> ActionTypes { get; set; }
        public bool IsSaved { get; set; }
        public bool IsNew { get; set; }
        private bool _IsWorking { get; set; }
        private Process _this { get; set; }

        public ProcessPageViewModel(int PlaceId)
        {
            _this = new Process();
            _this.PlaceId = PlaceId;
            IsNew = true;
            Initialize();
        }

        public ProcessPageViewModel(int PlaceId, Process Process)
        {
            try
            {
                _this = Process;
                _this.PlaceId = PlaceId;
                IsNew = false;
                Initialize(_this.ActionTypeId);
            }catch(Exception ex)
            {
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "ProcessPageViewModel", Time = DateTime.Now, Message = ex.Message };
                Error.Add();
            }
            
        }

        private async void Initialize(int AtId = -1)
        {
            try
            {
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
            }
            catch(Exception ex)
            {
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "Initialize", Time = DateTime.Now, Message = ex.Message};
                await Error.Add();
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

        public async Task Save()
        {
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
                    await _this.Add();
                }
                else
                {
                    //await _this.Edit();
                }
            }catch(Exception ex)
            {
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "Save", Time = DateTime.Now, Message = ex.Message };
                await Error.Add();
            }
            finally
            {
                IsWorking = false;
            }
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
