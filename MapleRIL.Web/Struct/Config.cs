using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Web.Struct
{
    public class Config
    {
        [JsonProperty("base_url")]
        public string BaseUrl { get; set; }

        [JsonProperty("regions")]
        public ConfigRegion[] Regions { get; set; }

        public class ConfigRegion
        {
            [JsonProperty("region")]
            public string Region { get; set; }

            [JsonProperty("folderPath")]
            public string FolderPath { get; set; }
        }
    }
}
