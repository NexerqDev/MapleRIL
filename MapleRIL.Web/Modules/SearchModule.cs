using Nancy;

namespace MapleRIL.Web.Modules
{
    public class SearchModule : NancyModule
    {
        public SearchModule()
        {
            Get["/search"] = p => View["Search", new
            {
                Title = "Search",
                Query = (string)this.Request.Query["q"],
                Region = (string)this.Request.Query["region"],
                Regions = RILManager.JsonRegionData
            }];

            Get["/lookup"] = p => View["Lookup", new
            {
                Title = "Search",
                Id = (string)this.Request.Query["id"],
                Region = (string)this.Request.Query["region"],
                Regions = RILManager.JsonRegionData
            }];
        }
    }
}