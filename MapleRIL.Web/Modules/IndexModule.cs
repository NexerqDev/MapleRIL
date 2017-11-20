using Nancy;

namespace MapleRIL.Web.Modules
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            Get["/"] = p => View["Index", new { Title = "Home" }];
        }
    }
}