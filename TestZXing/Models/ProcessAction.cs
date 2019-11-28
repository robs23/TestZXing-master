using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Interfaces;

namespace TestZXing.Models
{
    public class ProcessAction : Entity<ProcessAction>, IActionKeeper, INotifyPropertyChanged
    {
        public int ProcessActionId { get; set; }
        public override int Id
        {
            set => value = ProcessActionId;
            get => ProcessActionId;
        }

        public Nullable<int> ProcessId { get; set; }
        public Nullable<DateTime> PlannedStart { get; set; }
        public Nullable<DateTime> PlannedFinish { get; set; }
        public string PlaceName { get; set; }
        public Nullable<int> ActionId { get; set; }
        public string ActionName { get; set; }
        public int? GivenTime { get; set; }
        public int? HandlingTime { get; set; }
        public string Type { get; set; }
        public int? PlaceId { get; set; }

        public bool IsRequired
        {
            get
            {
                if (PlannedStart == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public bool IsChecked { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
