using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TestZXing.Interfaces;
using TestZXing.Models;
using TestZXing.Static;
using TestZXing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActionList : Rg.Plugins.Popup.Pages.PopupPage
    {
        ActionListViewModel vm;

        public ActionList(ActionListViewModel _vm)
        {
            InitializeComponent();
            vm = _vm;
            BindingContext = vm;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            if ((bool)((IActionKeeper)e.Item).IsMutable)
            {
                if ((bool)((IActionKeeper)e.Item).IsChecked)
                {
                    ((IActionKeeper)e.Item).IsChecked = false;
                    IActionKeeper item = ((IActionKeeper)e.Item);
                    if (vm.CheckedItems.Any(i => i.ActionId == item.ActionId))
                    {
                        vm.CheckedItems.Remove(vm.CheckedItems.FirstOrDefault(i => i.ActionId == item.ActionId));
                    }
                }
                else
                {
                    ((IActionKeeper)e.Item).IsChecked = true;
                    IActionKeeper item = ((IActionKeeper)e.Item);
                    if (!vm.CheckedItems.Any(i => i.ActionId == item.ActionId))
                    {
                        vm.CheckedItems.Add(ToProcessAction(item));
                    }

                }
            }
            else
            {
                DependencyService.Get<IToaster>().LongAlert($"Czynność została oznaczona jako wykonana w innej obsłudze i nie można jej zmienić..");
            }
            
                

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private ProcessAction ToProcessAction(IActionKeeper item)
        {
            ProcessAction pa = new ProcessAction();
            pa.ActionId = item.ActionId;
            pa.ActionName = item.ActionName;
            pa.GivenTime = item.GivenTime;
            pa.IsChecked = item.IsChecked;
            pa.LastChecks = item.LastChecks;
            pa.PlaceId = item.PlaceId;
            pa.PlaceName = item.PlaceName;
            pa.CreatedBy = RuntimeSettings.CurrentUser.UserId;
            return pa;
        }

        private async void BtnOK_Clicked(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Any()) { await PopupNavigation.Instance.PopAllAsync(true); }  // Hide handlings screen
            
        }
    }
}
