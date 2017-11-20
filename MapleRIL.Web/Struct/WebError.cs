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
        [JsonProperty("_errid")]
        public string ErrorId { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        public WebError(string error, string errId = null)
        {
            Error = error;
            ErrorId = errId;
        }
    }
}
