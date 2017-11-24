using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Common.RILJson
{
    public class RILJsonFormat
    {
        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("categories")]
        public CategoryData[] Categories { get; set; }

        public class CategoryData
        {
            [JsonProperty("category")]
            public string Category { get; set; }

            [JsonProperty("items")]
            public CatItem[] Items { get; set; }


            public class CatItem
            {
                [JsonProperty("id")]
                public string Id { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("description")]
                public string Description { get; set; }

                [JsonProperty("icon")]
                public string Icon { get; set; }
            }
        }
    }
}
