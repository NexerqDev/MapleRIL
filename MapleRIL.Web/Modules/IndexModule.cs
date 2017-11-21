using Nancy;
using Nancy.Responses.Negotiation;
using Newtonsoft.Json;

namespace MapleRIL.Web.Modules
{
    public class IndexModule : NancyModule
    {
        // cache this stuff so we dont always call it
        private string _spd = null;
        private string staticPassData
        {
            get
            {
                if (_spd == null)
                    _spd = JsonConvert.SerializeObject(new
                    {
                        regions = RILManager.RegionData
                    });

                return _spd;
            }
        }

        public Negotiator IndexVR => View["Index", new
        {
            JsonPassthru = staticPassData
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