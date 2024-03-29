﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;
using TestZXing.Static;

namespace TestZXing.Interfaces
{
    public interface IOfflineKeeper
    {
        Task Reload(string query, int? page, int? pageSize);
        bool IsWorking { get; set; }
        Task RestoreSyncQueue();
        string TableName { get;}
        Task Sync();
        Task Sync(string args);
        bool IsSynced { get; set; }
        Task<string> IsDependentOn();
        Task DeleteSynced();
        Task DeleteFromSyncQueue();

    }
}
