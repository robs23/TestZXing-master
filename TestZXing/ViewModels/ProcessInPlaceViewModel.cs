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
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class ProcessInPlaceViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ProcessItem> Items { get; set; }

        public ProcessInPlaceViewModel()
        {
            
        }

        public string Icon
        {
            get
            {
                return Static.RuntimeSettings.CurrentUser.Icon;
            }
        }

        public void Update(List<Process> nItems)
        {
            Items = new ObservableCollection<ProcessItem>();

            ProcessItem pii = new ProcessItem { Id = 0, Name = "Nowy", Description = "Dodaj nowe zgłoszenie" };
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
                        if (p.LastStatus != null)
                        {
                            //try to show last status if available

                            if (p.LastStatus == ProcessStatus.Planowany)
                            {
                                pi.Description = p.LastStatus.ToString() + " do wykonania " + p.PlannedFor + " dla " + p.AssignedUserNames +  Environment.NewLine + "Aktualnie obsługujących: " + p.OpenHandlings;
                            }
                            else
                            {
                                pi.Description = p.LastStatus.ToString() + " " + p.LastStatusOn + " przez " + p.LastStatusByName + Environment.NewLine + "Aktualnie obsługujących: " + p.OpenHandlings;
                            }
                            

                         }
                        else
                        {
                            pi.Description = "Utworzono " + p.CreatedOn.ToString() + " przez " + p.CreatedByName;
                        }
                        pi.Status = p.Status;
                        Items.Add(pi);
                    }
                }
                catch (Exception ex)
                {

                }

            }
        }

        public void Initialize()
        {
            
            OnPropertyChanged(nameof(Icon));
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
                str += "ID: " + pi.Id + ", Name: " + pi.Name + ", Description: " + pi.Description + ", Status: " + pi.Status + "\n";
            }
            return str;
        }
    }

    public class ProcessItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public Color StatusColor { get
            {
                if (Status == "Rozpoczęty")
                {
                    return Color.Green;
                }else if (Status == "Wstrzymany")
                {
                    return Color.Yellow;
                }else if(Status == "Zakończony" && Status == "Zrealizowany"){
                    return Color.Red;
                }else if(Status == "Planowany")
                {
                    return Color.LightBlue;
                }
                else
                {
                    return Color.Transparent;
                }
            } }
    }
}
