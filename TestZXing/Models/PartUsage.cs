using MvvmHelpers.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestZXing.Interfaces;
using TestZXing.Views;
using Xamarin.Forms;

namespace TestZXing.Models
{
    public class PartUsage : Entity<PartUsage>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public PartKeeper Keeper { get; set; }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public PartUsage()
        {
            IncreaseAmountCommand = new Xamarin.Forms.Command(IncreaseAmount);
            DecreaseAmountCommand = new Xamarin.Forms.Command(DecreaseAmount);
            ShowPartCommand = new AsyncCommand(ShowPart);
            Keeper = new PartKeeper();
        }

        public ICommand ShowPartCommand { get; }

        public async Task ShowPart()
        {
            Part p = await Keeper.GetById(PartId);
            if(p != null)
            {
                Application.Current.MainPage.Navigation.PushAsync(new PartPage(p));
            }
        }

        public int PartUsageId { get; set; }
        public override int Id
        {
            set => value = PartUsageId;
            get => PartUsageId;
        }
        public int? ProcessId { get; set; }
        public int? PlaceId { get; set; }

        public float Amount { get; set; } = 0;

        public int PartId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> ProducerId { get; set; }
        public string ProducerName { get; set; }
        public string Symbol { get; set; }
        public string Comment { get; set; }
        public string Image { get; set; }

        public string ImageUrl
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Image))
                {
                    return Static.Secrets.ApiAddress + Static.RuntimeSettings.ThumbnailsPath + this.Image;
                }
                else
                {
                    return "image_placeholder_128.png";
                }
            }
        }

        public async override Task<string> Add()
        {
            string res="OK";

            res = await base.Add();


            if (res=="OK")
            {
                try
                {
                    PartUsage _this = JsonConvert.DeserializeObject<PartUsage>(AddedItem);
                    this.PartUsageId = _this.PartUsageId;
                    this.TenantId = _this.TenantId;
                }
                catch (Exception ex)
                {
                    res = ex.Message;
                }
            }
            return res;

        }   

        public ICommand IncreaseAmountCommand { private set; get; }

        public void IncreaseAmount()
        {
            if(Amount < 1000)
            {
                Amount++;
                OnPropertyChanged(nameof(Amount));
            }
            
        }

        public ICommand DecreaseAmountCommand { private set; get; }

        public void DecreaseAmount()
        {
            if (Amount > 1)
            {
                Amount--;
                OnPropertyChanged(nameof(Amount));
            }
                
        }

    }
}
