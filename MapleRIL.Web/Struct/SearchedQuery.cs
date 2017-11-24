using MapleRIL.Common.RILJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Web.Struct
{
    public class SearchedQuery
    {
        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("items")]
        public RILJsonItem[] Items { get; set; }

        public SearchedQuery(string region, RILJsonItem[] items)
        {
            Region = region;
            Items = items;
        }
    }
}
