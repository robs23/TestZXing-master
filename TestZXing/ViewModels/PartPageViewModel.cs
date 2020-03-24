using System;
using System.Collections.Generic;
using System.Text;
using TestZXing.Models;

namespace TestZXing.ViewModels
{
    public class PartPageViewModel : BaseViewModel
    {
        public Part _this { get; set; }

        public PartPageViewModel(Part part)
        {
            _this = part;
        }

        public string Name
        {
            get { return _this.Name; }
            set
            {
                if (value != _this.Name)
                {
                    _this.Name = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Description
        {
            get { return _this.Description; }
            set
            {
                if (value != _this.Description)
                {
                    _this.Description = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ProducerName
        {
            get { return _this.ProducerName; }
            set
            {
                if (value != _this.ProducerName)
                {
                    _this.ProducerName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Symbol
        {
            get { return _this.Symbol; }
            set
            {
                if (value != _this.Symbol)
                {
                    _this.Symbol = value;
                    OnPropertyChanged();
                }
            }
        }


        public string ImageUrl
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_this.Image))
                {
                    return Static.Secrets.ApiAddress + Static.RuntimeSettings.ThumbnailsPath + _this.Image;
                }
                else
                {
                    return "image_placeholder_128.png";
                }
            }
        }
    }
}
