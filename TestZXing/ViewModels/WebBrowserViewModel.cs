using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TestZXing.ViewModels
{
    class WebBrowserViewModel: BaseViewModel
    {
        public WebBrowserViewModel(string url)
        {
            Url = url;
        }

        private string _Url { get; set; }
        public string Url
        {
            get
            {
                return _Url;
            }
            set
            {
                if(value != _Url)
                {
                    _Url = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
