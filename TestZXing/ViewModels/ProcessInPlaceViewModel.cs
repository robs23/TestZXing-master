﻿using System;
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
    public class ProcessInPlaceViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<ProcessItem> Items { get; set; }

        public ProcessInPlaceViewModel(List<Process> nItems)
        {
            Items = new ObservableCollection<ProcessItem>();
            ProcessItem pii = new ProcessItem { Id = 0, Name = "Nowy", Description = "Dodaj nowe zlecenie" };
            Items.Add(pii);
            if (nItems.Any())
            {
                try
                {
                    foreach (Process p in nItems)
                    {
                        ProcessItem pi = new ProcessItem();
                        pi.Id = p.ProcessId;
                        pi.Name = p.ActionTypeName;
                        pi.Description = "Status: " + p.Status + ". Utworzono " + p.CreatedOn.ToString() + " przez " + p.CreatedByName;
                        Items.Add(pi);
                    }
                }catch(Exception ex)
                {
                    Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "ProcessInPlaceViewModel", Time = DateTime.Now, Message = ex.Message };
                }
                
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
                    OnPropertyChanged();
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