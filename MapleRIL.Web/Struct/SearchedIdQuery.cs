using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Web.Struct
{
    public class SearchedIdQuery
    {
        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("item")]
        public SearchedItem Item { get; set; }

        public SearchedIdQuery(string region, SearchedItem item)
        {
            Region = region;
            Item = item;
        }
    }
}
