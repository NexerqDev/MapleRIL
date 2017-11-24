using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Web.Struct
{
    public class ApiRegions
    {
        [JsonProperty("regions")]
        public ApiRegion[] Regions;

        public ApiRegions(string[] r)
        {
            Regions = r.Select(f => new ApiRegion()
            {
                Region = f,
                Version = RILManager.RegionJsons[f].Data.Version
            }).ToArray();
        }

        public class ApiRegion
        {
            [JsonProperty("region")]
            public string Region;

            [JsonProperty("version")]
            public string Version;
        }
    }
}
