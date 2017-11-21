using Nancy;
using Nancy.Responses.Negotiation;

namespace MapleRIL.Web.Modules
{
    public class IndexModule : NancyModule
    {
        public Negotiator IndexVR => View["Index", new
        {
            Regions = RILManager.JsonRegionData,
            Query = (string)this.Request.Query["q"],
            Region = (string)this.Request.Query["region"],
            Id = (string)this.Request.Query["id"]
        }];

        public IndexModule()
        {
            //Get["/"] = p => View["Index", new {
            //    Title = "Home",
            //    Regions = RILManager.JsonRegionData
            //}];

            // catch all for our SPA
            Get["/"] = p => IndexVR;
            Get[@"^(.*)$"] = p => IndexVR;
        }
    }
}