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
    }
}
