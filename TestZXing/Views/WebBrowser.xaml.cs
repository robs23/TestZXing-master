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
    public partial class WebBrowser : ContentPage
    {
        WebBrowserViewModel vm;

        public WebBrowser(string url)
        {
            InitializeComponent();
            vm = new WebBrowserViewModel(url);
            BindingContext = vm;
        }
    }
}