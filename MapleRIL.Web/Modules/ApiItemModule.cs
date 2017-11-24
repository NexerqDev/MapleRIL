using MapleRIL.Common.RILJson;
using MapleRIL.Web.Struct;
using Nancy;
using System;
using System.Linq;

namespace MapleRIL.Web.Modules
{
    public class ApiItemModule : NancyModule
    {
        public ApiItemModule()
        {
            Get["/api/item"] = p =>
            {
                string id = this.Request.Query["id"];
                if (string.IsNullOrEmpty(id))
                    return Response.AsJson(new WebError("No item id given."));

                string region = this.Request.Query["region"];
                if (string.IsNullOrEmpty(region)
                 || !WebEngine.Rjm.RegionJsons.ContainsKey(region))
                    return Response.AsJson(new WebError("Invalid region."));

                RILJsonItem item = WebEngine.Rjm.RegionJsons[region].GetItemById(id);
                if (item == null)
                    return Response.AsJson(new WebError("No item given by that ID.", "NO_ITEM"));

                return Response.AsJson(new SearchedIdQuery(region, item));
            };
        }
    }
}