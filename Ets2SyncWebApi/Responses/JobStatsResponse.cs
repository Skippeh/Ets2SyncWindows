using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Ets2SyncWebApi.Models;
using Ets2SyncWebApi.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PrismSyncLibrary;

namespace Ets2SyncWebApi.Responses
{
    internal class JobStatsResponse : IResponseData<JobStats>
    {
        [JsonProperty("last_sync")]
        public string LastSync { get; set; }

        [JsonProperty("total_offers")]
        public Dictionary<int, int> TotalOffers { get; set; }

        public JobStats ToRealType()
        {
            return new JobStats
            {
                LastSync = DateTime.ParseExact(LastSync, "MMM dd HH:mm:ss", CultureInfo.InvariantCulture),
                TotalOffers = TotalOffers.ToDictionary(kv => ToGameModType(kv.Key), kv => kv.Value)
            };
        }

        private GameModType ToGameModType(int index)
        {
            switch (index)
            {
                case 1: return GameModType.ETS2Vanilla;
                case 2: return GameModType.ATSVanilla;
                case 3: return GameModType.ETS2ProMods;
            }

            throw new NotImplementedException($"Unknown game index: {index}");
        }
    }
}