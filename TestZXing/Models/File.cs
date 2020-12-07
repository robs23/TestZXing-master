using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace TestZXing.Models
{
    public class File : Entity<File>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        [Unique]
        public int FileId { get; set; }
        public override int Id
        {
            set => value = FileId;
            get => FileId;
        }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public string Link { get; set; }
        public int? PartId { get; set; }
        public int? PlaceId { get; set; }
        public int? ProcessId { get; set; }
        public string _Source { get; set; }
        public string Source
        {
            get
            {
                return _Source;
            }
            set
            {
                if(_Source != value)
                {
                    _Source = value;
                }
                OnPropertyChanged();
            }
        }
    }
}
