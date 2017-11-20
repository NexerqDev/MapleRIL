using MapleRIL.Common;
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
                 || !RILManager.Searchers.ContainsKey(region))
                    return Response.AsJson(new WebError("Invalid region."));

                RILItem item = RILManager.Searchers[region].GetItemById(id);
                if (item == null)
                    return Response.AsJson(new WebError("No item given by that ID.", "NO_ITEM"));

                System.Drawing.Bitmap icon = item.Icon;
                SearchedDetailedItem sdi = new SearchedDetailedItem()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Category = item.Category,
                    Description = item.Description,
                    Icon = icon == null ? null : "data:image/png;base64," + Convert.ToBase64String(Util.BitmapToBytes(icon))
                };

                return Response.AsJson(new SearchedIdQuery(region, sdi));
            };
        }
    }
}