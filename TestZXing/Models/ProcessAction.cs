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
        public int? HandlingId { get; set; }
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

        public bool? _IsChecked { get; set; }
        public bool? IsChecked
        {
            get
            {
                if (_IsChecked == null)
                {
                    return false;
                }
                return _IsChecked;
            }
            set
            {
                if (value != _IsChecked)
                {
                    _IsChecked = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<DateTime?> LastChecks { get; set; }
        public DateTime? LastCheck
        {
            get
            {
                if (LastChecks.Any())
                {
                    return LastChecks.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
