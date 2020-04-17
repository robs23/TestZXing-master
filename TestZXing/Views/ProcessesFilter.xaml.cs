using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProcessesFilter : Rg.Plugins.Popup.Pages.PopupPage
    {
        ProcessesFilterViewModel vm;
        public ProcessesFilter(ProcessesFilterViewModel _vm)
        {
            InitializeComponent();
            vm = _vm;
            BindingContext = vm;
        }
    }
}