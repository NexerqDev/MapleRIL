using MapleRIL.Web.Struct;
using Nancy;
using System.Linq;

namespace MapleRIL.Web.Modules
{
    public class ApiModule : NancyModule
    {
        public ApiModule()
        {
            Get["/api/regions"] = p => Response.AsJson(
                new ApiRegions(Program.Config.Regions.Select(r => r.Region).ToArray()));
        }
    }
}