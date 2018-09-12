using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class ProcessViewModel
    {
        public Process _process;

        public ProcessViewModel(Process process)
        {
            _process = process;
        }

        public int Id { get { return _process.ProcessId; } }
        public string Name { get { return _process.ActionTypeName; } }
        public string Description { get { return "Utworzono " + _process.CreatedOn + " przez " + _process.CreatedByName; } }
        public string Status { get { return _process.Status; } }
        public Color StatusColor
        {
            get
            {
                if (Status == "Rozpoczęty")
                {
                    return Color.Green;
                }
                else if (Status == "Wstrzymany")
                {
                    return Color.Yellow;
                }
                else if (Status == "Zakończony" && Status == "Zrealizowany")
                {
                    return Color.Red;
                }
                else
                {
                    return Color.Transparent;
                }
            }
        }
    }
}
