using Nancy;

namespace MapleRIL.Web.Modules
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            Get["/"] = p => View["Index", new { Title = "Home" }];
            Get["/search"] = p => View["Search", new
            {
                Title = "Search",
                Query = (string)this.Request.Query["q"],
                Region = (string)this.Request.Query["region"]
            }];
        }
    }
}