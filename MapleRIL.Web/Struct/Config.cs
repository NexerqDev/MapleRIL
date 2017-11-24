using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Web.Struct
{
    public class Config
    {
        [DefaultValue(3579)]
        [JsonProperty("port", DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Port { get; set; }

        [JsonProperty("base_url")]
        public string BaseUrl { get; set; }

        [JsonProperty("nancy_admin_password")]
        public string NancyAdminPassword { get; set; }

        [JsonProperty("regions")]
        public ConfigRegion[] Regions { get; set; }

        public class ConfigRegion
        {
            [JsonProperty("region")]
            public string Region { get; set; }

            [JsonProperty("json")]
            public string JsonPath { get; set; }
        }
    }
}
