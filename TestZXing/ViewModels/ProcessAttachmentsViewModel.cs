using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;
using TestZXing.Static;

namespace TestZXing.ViewModels
{
    class ProcessAttachmentsViewModel: BaseViewModel
    {
        FileKeeper Files = new FileKeeper();

        public ProcessAttachmentsViewModel()
        {

        }
        public async Task Initialize(int? processId = null)
        {
            base.Initialize();

            if (processId != null)
            {
                await Files.Reload($"ProcessId={processId} and CreatedBy={RuntimeSettings.CurrentUser.UserId}");
                Items = new ObservableRangeCollection<File>(Files.Items);
                Task.Run(() => TakeSnapshot());
            }
        }

        public async Task TakeSnapshot()
        {
            //remember what you've already saved
            SavedItems = this.Items.CloneJson<ObservableRangeCollection<File>>();
        }

        public async Task<bool> IsDirty()
        {
            bool res = false;
            if (Items != null && SavedItems != null)
            {
                if (Items.Count() > SavedItems.Count())
                {
                    res = true;
                }
                else
                {
                    foreach (File f in Items)
                    {
                        if (f.FileId == 0)
                        {
                            //on the whole, items without id definitely haven't been saved yet
                            res = true;
                            break;
                        }
                        else if (!SavedItems.Any(i => i.FileId == f.FileId))
                        {
                            //Current item hasn't been saved yet
                            res = true;
                            break;
                        }
                    }
                    foreach (File f in SavedItems)
                    {
                        if (!Items.Any(i => i.FileId == f.FileId))
                        {
                            //Current item must hvae been deleted but this info hasn't been saved
                            res = true;
                            break;
                        }
                    }
                }
            }


            return res;
        }

        ObservableRangeCollection<File> _Items;

        public ObservableRangeCollection<File> Items
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

        ObservableRangeCollection<File> _SavedItems;

        public ObservableRangeCollection<File> SavedItems
        {
            get
            {
                return _SavedItems;
            }
            set
            {
                SetProperty(ref _SavedItems, value);
            }
        }
    }
}
