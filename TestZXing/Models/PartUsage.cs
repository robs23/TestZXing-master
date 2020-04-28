﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using TestZXing.Interfaces;
using Xamarin.Forms;

namespace TestZXing.Models
{
    public class PartUsage : Entity<PartUsage>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public PartUsage()
        {
            IncreaseAmountCommand = new Command(IncreaseAmount);
            DecreaseAmountCommand = new Command(DecreaseAmount);
        }

        public int PartUsageId { get; set; }
        public override int Id
        {
            set => value = PartUsageId;
            get => PartUsageId;
        }

        public int? HandlingId { get; set; }
        public int? ProcessId { get; set; }
        public int? PlaceId { get; set; }

        public int Amount { get; set; } = 0;

        public int PartId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> ProducerId { get; set; }
        public string ProducerName { get; set; }
        public string Symbol { get; set; }

        public string Image { get; set; }

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
