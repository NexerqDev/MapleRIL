using MapleRIL.Common;
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
                    _jrd = new ApiRegions(Program.Config.Regions.Select(r => r.Region).ToArray()).Regions;
                return _jrd;
            }
        }

        public static Dictionary<string, RILFileManager> Rfms = new Dictionary<string, RILFileManager>();
        public static Dictionary<string, RILSearcher> Searchers = new Dictionary<string, RILSearcher>();

        public static RILFileManager LoadRfm(string region, string path)
        {
            var rfm = new RILFileManager(region, path);
            Rfms.Add(region, rfm);
            return rfm;
        }

        public static RILSearcher LoadSearcher(string region, string path = null)
        {
            RILFileManager rfm;
            if (!Rfms.ContainsKey(region))
                rfm = LoadRfm(region, path);
            else
                rfm = Rfms[region];

            var searcher = new RILSearcher(rfm);
            Searchers.Add(region, searcher);
            return searcher;
        }
    }
}
