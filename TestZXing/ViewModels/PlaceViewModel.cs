﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using MvvmHelpers.Commands;
using TestZXing.Models;
using TestZXing.Views;
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class PlaceViewModel : ObservableRangeCollection<ProcessViewModel>, INotifyPropertyChanged
    {
        private Place _place;

        // It's a backup variable for storing ProcessViewModel objects
        private ObservableRangeCollection<ProcessViewModel> Processes =
            new ObservableRangeCollection<ProcessViewModel>();

        public PlaceViewModel(Place place, bool expanded = false)
        {
            ShowPlaceCommand = new AsyncCommand(ShowPlace);
            this._place = place;
            this._expanded = expanded;
            // Place has many processes. Once we get it, init ProcessViewModel and store it in a backup variable
            foreach(Process p in place.Processes)
            {
                Processes.Add(new ProcessViewModel(p));
            }
            // PlaceViewModel add a range with ProcessViewModel
            if (expanded)
            {
                this.AddRange(Processes);
            }
            if (!string.IsNullOrWhiteSpace(_place.Image))
            {
                ImageUrl = Static.Secrets.ApiAddress + Static.RuntimeSettings.ThumbnailsPath + _place.Image;
            }
        }

        public string Name { get { return _place.Name; } }
        public string Set { get { return _place.SetName; } }
        public string Area { get { return _place.AreaName; } }
        public ICommand ShowPlaceCommand { get; }

        public async Task ShowPlace()
        {
            if(_place != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new PlacePage(_place));
            }
            
        }


        private ImageSource _ImageUrl { get; set; } = "image_placeholder_128.png";
        public ImageSource ImageUrl
        {
            get
            {
                return _ImageUrl;
            }
            set
            {
                if (_ImageUrl != value)
                {
                    _ImageUrl = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ImageUrl"));
                }
            }
        }


        private bool _expanded;
        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                if (_expanded != value)
                {
                    _expanded = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Expanded"));
                    OnPropertyChanged(new PropertyChangedEventArgs("StateIcon"));
                    if (_expanded)
                    {
                        this.AddRange(Processes);
                    }
                    else
                    {
                        this.Clear();
                    }
                }
            }
        }

        public string StateIcon
        {
            get { return Expanded ? "up" : "down"; }
        }
    }
}
