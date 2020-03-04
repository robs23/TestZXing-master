using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestZXing.Classes;
using TestZXing.Models;
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class AssignedPartsViewModel : BaseViewModel
    {

        public AssignedPartsViewModel()
        {
            ScanCommand = new AsyncCommand(Scan);
            SearchCommand = new AsyncCommand(Search);
            PartsPageViewModel = new PartsPageViewModel();
            Items = new ObservableRangeCollection<Part>();
        }

        public async Task Initialize()
        {
            base.Initialize();
            PartsPageViewModel.Initialize();
            PartsPageViewModel.Mode = PartsPageMode.PartsPicker;
        }

        public PartsPageViewModel PartsPageViewModel { get; set; }

        public async Task Scan()
        {
            try
            {
                QrHandler qr = new QrHandler();
                string res = await qr.Scan();
                res = res.Replace("<Part>", "");
            }
            catch (Exception ex)
            {

            }
        }

        public ICommand ScanCommand { get; }

        public ICommand SearchCommand { get; }

        private async Task Search()
        {
            Application.Current.MainPage.Navigation.PushAsync(new PartsPage(PartsPageViewModel));
        }


        ObservableRangeCollection<Part> _Items;

        public ObservableRangeCollection<Part> Items
        {
            get
            {
                return _Items;
            }
            set
            {
                SetProperty(ref _Items, value);
            }
        }
    }
}
