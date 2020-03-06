using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestZXing.Classes;
using TestZXing.Interfaces;
using TestZXing.Models;
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class AssignedPartsViewModel : BaseViewModel
    {
        public PartKeeper PartKeeper { get; set; }

        public AssignedPartsViewModel()
        {
            ScanCommand = new AsyncCommand(Scan);
            SearchCommand = new AsyncCommand(Search);
            RemoveItemCommand = new AsyncCommand(RemoveIem);
            Items = new ObservableRangeCollection<PartUsage>();
            PartKeeper = new PartKeeper();
        }

        public async Task Initialize()
        {
            base.Initialize();
            PartsPageViewModel = new PartsPageViewModel();
            PartsPageViewModel.Initialize();
            PartsPageViewModel.Mode = PartsPageMode.PartsPicker;
        }

        public async Task Update()
        {
            if (PartsPageViewModel.SelectedItems.Count > 0)
            {
                //add new item and clear selectedItems
                foreach(Part p in PartsPageViewModel.SelectedItems)
                {
                    PartUsage pu = new PartUsage
                    {
                        PartId = p.PartId,
                        Name = p.Name,
                        Symbol = p.Symbol,
                        ProducerId = p.ProducerId,
                        ProducerName = p.ProducerName,
                        Description = p.Description,
                        Image = p.Image,
                        Amount = 1
                    };
                    Items.Add(pu);
                    
                }
                //OnPropertyChanged(nameof(Items));
                PartsPageViewModel.SelectedItems.Clear();
            }
        }

        public PartsPageViewModel PartsPageViewModel { get; set; }

        public async Task Scan()
        {
            try
            {
                QrHandler qr = new QrHandler();
                string res = await qr.Scan();
                res = res.Replace("<Part>", "");
                Part p = await PartKeeper.GetByToken(res);
                if (p == null)
                {
                    DependencyService.Get<IToaster>().ShortAlert($"Część o kodzie {res} nie została znaleziona!");
                }
                else
                {
                    PartUsage pu = new PartUsage
                    {
                        PartId = p.PartId,
                        Name = p.Name,
                        Symbol = p.Symbol,
                        ProducerId = p.ProducerId,
                        ProducerName = p.ProducerName,
                        Description = p.Description,
                        Image = p.Image,
                        Amount = 1
                    };
                    Items.Add(pu);
                }
                

                
            }
            catch (Exception ex)
            {
                DependencyService.Get<IToaster>().ShortAlert($"Error: {ex.Message}");
            }
        }

        public ICommand ScanCommand { get; }

        public ICommand SearchCommand { get; }

        private async Task Search()
        {
            Application.Current.MainPage.Navigation.PushAsync(new PartsPage(PartsPageViewModel));
        }

        public ICommand RemoveItemCommand { get; }

        private Task RemoveIem()
        {
            throw new NotImplementedException();
        }


        ObservableRangeCollection<PartUsage> _Items;

        public ObservableRangeCollection<PartUsage> Items
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

        public bool AreItemsSelected
        {
            get
            {
                return false;
            }
        }

        public string EmptyViewCaption
        {
            get
            {
                if (IsWorking)
                {
                    return "Trwa wczytywanie..";
                }
                else
                {
                    return "Nie użyto żadnej części do tego zgłoszenia. Użyj pierwszą część korzystając z przycisków poniżej. Przycisk LUPY pozwala wyszukać część po nazwie/symbolu, przycisk QR pozwala dodać część skanując jej kod QR";
                }
            }
        }
    }
}
