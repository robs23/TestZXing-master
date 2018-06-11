using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;

namespace TestZXing.ViewModels
{
    public class ProcessPageViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<ActionType> ActionTypes { get; set; }
        public bool IsNew { get; set; }//whether this process is new or from Db
        private Process _this { get; set; }

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
                    _this.ActionTypeId = _type.ActionTypeId;
                    _this.ActionTypeName = _type.Name;
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


        public ProcessPageViewModel()
        {
            _this = new Process();
            Initialize();
        }

        public ProcessPageViewModel(Process Process)
        {
            _this = Process;
            Initialize();
        }

        private async void Initialize()
        {
            ActionTypesKeeper keeper = new ActionTypesKeeper();
            await keeper.Reload();
            foreach(ActionType at in keeper.Items)
            {
                ActionTypes.Add(at);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
