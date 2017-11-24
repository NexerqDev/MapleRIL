using MapleRIL.Web.Struct;
using Nancy;
using System;
using System.Linq;

namespace MapleRIL.Web.Modules
{
    public class ApiSearchModule : NancyModule
    {
        public ApiSearchModule()
        {
            Get["/api/search"] = p =>
            {
                string query = this.Request.Query["q"];
                if (string.IsNullOrEmpty(query))
                    return Response.AsJson(new WebError("No lookup given."));

                string region = this.Request.Query["region"];
                if (string.IsNullOrEmpty(region)
                 || !WebEngine.Rjm.RegionJsons.ContainsKey(region))
                    return Response.AsJson(new WebError("Invalid region."));

                var items = WebEngine.Rjm.RegionJsons[region].Search(query);
                return Response.AsJson(new SearchedQuery(region, items));
            };
        }
    }
}