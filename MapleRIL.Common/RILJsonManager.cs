using MapleRIL.Common.RILJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Common
{
    public class RILJsonManager
    {
        public Dictionary<string, RILJsonEngine> RegionJsons = new Dictionary<string, RILJsonEngine>();

        public RILJsonEngine LoadJsonEngine(string region, string path)
        {
            var rj = new RILJsonEngine(region, path);
            RegionJsons.Add(region, rj);
            return rj;
        }
    }
}
