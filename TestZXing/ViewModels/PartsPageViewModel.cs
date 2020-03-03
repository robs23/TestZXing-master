using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestZXing.Interfaces;
using TestZXing.Models;
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class PartsPageViewModel : ObservableObject
    {
        public PartKeeper Keeper { get; set; }

        

        public PartsPageViewModel()
        {
            Keeper = new PartKeeper();
            ItemTreshold = 5;
            PageSize = 15;
            ReloadCommand = new AsyncCommand(Reload);
            ItemTresholdReachedCommand = new AsyncCommand(ItemTresholdReached);
        }

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        string _SearchQuery;
        public string SearchQuery
        {
            get
            {
                return _SearchQuery;
            }
            set
            {
                SetProperty(ref _SearchQuery, value);
            }
        }
        public ICommand ReloadCommand { get; }

        public async Task Reload()
        {
            try
            {
                string query = null;
                CurrentPage = 1;
                Items = new ObservableRangeCollection<Part>();

                query = GetQueryString();
                
                await Keeper.Reload(query, CurrentPage, PageSize);
                if (Keeper.Items.Count > 0)
                {
                    Items.AddRange(Keeper.Items);
                }
            }catch(Exception ex)
            {
                DependencyService.Get<IToaster>().ShortAlert($"Error: {ex.Message}");
            }
        }

        private string GetQueryString()
        {
            string query = null;
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                query = "";
                string[] keys = SearchQuery.Split(' ');
                for (int i = 0; i < keys.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(query))
                    {
                        query += " AND ";
                    }
                    query += $"(Name.ToLower().Contains(\"{keys[i].ToLower()}\") OR Symbol.ToLower().Contains(\"{keys[i].ToLower()}\") OR ProducerName.ToLower().Contains(\"{keys[i].ToLower()}\"))";
                }
            }
            return query;
        }

        public ICommand ItemTresholdReachedCommand { get; }
        public async Task ItemTresholdReached()
        {
            try
            {
                string query = null;
                CurrentPage++;

                query = GetQueryString();

                await Keeper.Reload(query, CurrentPage, PageSize);
                if (Keeper.Items.Count > 0)
                {
                    Items.AddRange(Keeper.Items);
                }
            }
            catch (Exception ex)
            {
                DependencyService.Get<IToaster>().ShortAlert($"Error: {ex.Message}");
            }

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


        int _ItemTreshold;

        public int ItemTreshold
        {
            get
            {
                return _ItemTreshold;
            }
            set
            {
                SetProperty(ref _ItemTreshold, value);
            }
        }
    }
}
