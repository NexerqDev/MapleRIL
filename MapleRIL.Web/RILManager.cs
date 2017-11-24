using MapleRIL.Web.Struct;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MapleRIL.Web.Struct.ApiRegions;

namespace MapleRIL.Web
{
    public static class RILManager
    {
        // idk caching should be most efficient, kinda ugly though
        public static ApiRegion[] _jrd = null;
        public static ApiRegion[] RegionData
        {
            get
            {
                if (_jrd == null)
                    _jrd = new ApiRegions(Bootstrapper.Config.Regions.Select(r => r.Region).ToArray()).Regions;
                return _jrd;
            }
        }

        public static Dictionary<string, RILJson> RegionJsons = new Dictionary<string, RILJson>();

        public static RILJson LoadJson(string region, string path)
        {
            var rj = new RILJson(region, path);
            RegionJsons.Add(region, rj);
            return rj;
        }
    }
}
