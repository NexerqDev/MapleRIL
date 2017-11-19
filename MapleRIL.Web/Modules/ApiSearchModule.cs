using MapleRIL.Common;
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
            Get["/api"] = p => Response.AsText(": )");

            Get["/api/search"] = p =>
            {
                string query = this.Request.Query["q"];
                if (string.IsNullOrEmpty(query))
                    return Response.AsJson(new WebError("No lookup given."));

                string region = this.Request.Query["region"];
                if (string.IsNullOrEmpty(region)
                 || !RILManager.Searchers.ContainsKey(region))
                    return Response.AsJson(new WebError("Invalid region."));

                RILItem[] lookup = RILManager.Searchers[region].Search(query);
                var items = lookup.Select(l =>
                {
                    System.Drawing.Bitmap icon = l.Icon;
                    return new SearchedItem()
                    {
                        Id = l.Id,
                        Name = l.Name,
                        Category = l.Category//,
                        //Icon = icon == null ? null : "data:image/png;base64," + Convert.ToBase64String(Util.BitmapToBytes(icon))
                    };
                });

                return Response.AsJson(new SearchedQuery(region, items.ToArray()));
            };
        }
    }
}