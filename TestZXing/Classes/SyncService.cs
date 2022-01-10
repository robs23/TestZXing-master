using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Interfaces;
using TestZXing.Static;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TestZXing.Models
{
    public class SyncService
    {
        public List<IOfflineKeeper> Keepers { get; set; }
        public bool IsWorking
        {
            get
            {
                return Keepers.Any(k => k.IsWorking);
            }
        }

        public SyncService()
        {
            Keepers = new List<IOfflineKeeper>();
        }

        public async Task Sync()
        {
            List<IOfflineKeeper> SyncList = Keepers.Where(k => k.IsSynced == false).ToList();
            int attemptsLeftCount = 5;

            while (SyncList.Any() && attemptsLeftCount > 0)
            {
                bool hasSyncedAnything = false;

                foreach (var keeper in SyncList)
                {
                    string masterTable = await keeper.IsDependentOn();
                    if(masterTable == null)
                    {
                        hasSyncedAnything = true;
                        await keeper.Sync();
                        SyncList.Remove(keeper);
                    }
                    else
                    {
                        //check if masterTable isSynced, if yes, we can sync
                        var master = SyncList.Where(l => l.TableName == masterTable).FirstOrDefault();
                        if(master == null)
                        {
                            if (master.IsSynced)
                            {
                                hasSyncedAnything = true;
                                await keeper.Sync();
                                SyncList.Remove(keeper);
                            }
                        }
                    }
                }

                //so we don't go into dead loop
                if (!hasSyncedAnything)
                    attemptsLeftCount--;
            }
        }

        public async Task RestoreSyncQueue()
        {
            foreach(var keeper in Keepers)
            {
                await keeper.RestoreSyncQueue();
            }
        }
    }
}
