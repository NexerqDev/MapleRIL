using MapleRIL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Web
{
    public static class RILManager
    {
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
