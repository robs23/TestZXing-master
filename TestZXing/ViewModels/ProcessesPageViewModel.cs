using System;
using System.Collections.Generic;
using System.Text;

namespace TestZXing.ViewModels
{
    public class ProcessesPageViewModel : BaseViewModel
    {
        public ActiveProcessesViewModel UserProcesses { get; set; }
        public ActiveProcessesViewModel AllProcesses { get; set; }
        public ProcessesPageViewModel()
        {

        }

        public string Icon
        {
            get
            {
                return Static.RuntimeSettings.CurrentUser.Icon;
            }
        }
    }
}
