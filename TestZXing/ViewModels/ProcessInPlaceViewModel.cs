using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;

namespace TestZXing.ViewModels
{
    public class ProcessInPlaceViewModel
    {
        public List<ProcessItem> Items = new List<ProcessItem>();

        public ProcessInPlaceViewModel(List<Process> nItems)
        {
            ProcessItem pii = new ProcessItem { Id = 0, Name = "Nowy", Description = "Dodaj nowe zlecenie" };
            Items.Add(pii);
            foreach(Process p in nItems)
            {
                ProcessItem pi = new ProcessItem();
                pi.Id = p.ProcessId;
                pi.Name = p.ActionTypeName;
                pi.Description = "Status: " + p.Status + ". Utworzono " + p.CreatedOn.ToString() + " przez " + p.CreatedByName;
                Items.Add(pi);
            }
        }

        private ProcessItem _selectedItem { get; set; }
        public ProcessItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if(_selectedItem != value)
                {
                    _selectedItem = value;
                }
            }
        }
        public override string ToString()
        { 
            string str = "";
            foreach(ProcessItem pi in Items)
            {
                str += "ID: " + pi.Id + ", Name: " + pi.Name + ", Description: " + pi.Description + "\n";
            }
            return str;
        }
    }

    public class ProcessItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
