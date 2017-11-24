using MapleRIL.Web.Struct;
using Nancy;
using System.Linq;

namespace MapleRIL.Web.Modules
{
    public class ApiModule : NancyModule
    {
        public ApiModule()
        {
            Get["/api"] = p => Response.AsText(": )");
        }
    }
}