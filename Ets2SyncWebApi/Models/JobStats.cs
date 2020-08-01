using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PrismSyncLibrary;

namespace Ets2SyncWebApi.Models
{
    public class JobStats
    {
        public DateTime LastSync { get; set; }
        public Dictionary<GameModType, int> TotalOffers { get; set; }
    }
}