using Nancy;

namespace MapleRIL.Web.Modules
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            Get["/"] = p => View["Index"];
            Get["/about"] = p => View["About"];
            Get["/contact"] = p => View["Contact"];
        }
    }
}