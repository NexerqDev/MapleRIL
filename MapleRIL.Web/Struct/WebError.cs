using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Web.Struct
{
    public class WebError
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        public WebError(string error)
        {
            Error = error;
        }
    }
}
