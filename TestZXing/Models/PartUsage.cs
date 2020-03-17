﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace TestZXing.Models
{
    public class PartUsage : Part, INotifyPropertyChanged
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

        public int Amount { get; set; } = 0;

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