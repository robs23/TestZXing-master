using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestZXing.ViewModels
{
    public class BaseViewModel : ObservableObject
    {

        bool _IsWorking;
        public virtual bool IsWorking
        {
            get { return _IsWorking; }
            set
            {
                if (SetProperty(ref _IsWorking, value) == true)
                {
                    OnPropertyChanged(nameof(IsIdle));
                }
            }
        }

        public virtual bool IsIdle
        {
            get
            {
                if (IsWorking)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }


        public async Task Initialize()
        {
            IsInitialized = true;
        }

        bool _IsInitialized = false;

        public bool IsInitialized
        {
            get { return _IsInitialized; }
            set { SetProperty(ref _IsInitialized, value); }
        }
    }
}
