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
    public class PlaceAction : Entity<PlaceAction>, IActionKeeper, INotifyPropertyChanged
    {
        public int PlaceActionId { get; set; }
        public override int Id
        {
            set => value = PlaceActionId;
            get => PlaceActionId;
        }

        public Nullable<int> PlaceId { get; set; }
        public string PlaceName { get; set; }
        public Nullable<int> ActionId { get; set; }
        public string ActionName { get; set; }
        public int? GivenTime { get; set; }
        public string Type { get; set; }
        public bool IsRequired { get; set; }
        public bool _IsChecked { get; set; }
        public bool IsChecked
        {
            get
            {
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

        public DateTime?[] LastCheck { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
