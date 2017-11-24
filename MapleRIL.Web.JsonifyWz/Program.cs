using MapleLib.WzLib;
using MapleRIL.Common;
using MapleRIL.Common.RILItemType;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.JsonifyWz
{
    class Program
    {
        const string REGION = "GMS";
        const string GAME_PATH = @"D:\Games\Nexon\Library\maplestory\appdata";

        static void Main(string[] args)
        {
            // load wz
            var rfm = new RILFileManager(REGION, GAME_PATH);
            var typs = RILItemTypes.GetAllItemTypes(rfm);

            var data = new OutputData();
            data.Region = REGION;
            data.Version = rfm.GameVersion;

            var catdata = new List<OutputData.CategoryData>();
            foreach (RILBaseItemType typ in typs)
            {
                Console.WriteLine("Writing " + typ.FriendlyName);
                List<WzImageProperty> props = typ.GetAllStringIdProperties(rfm);

                var items = props.Select(p => new RILItem(rfm, p, typ));
                var category = new OutputData.CategoryData();
                category.Category = typ.FriendlyName;

                var convItems = new List<OutputData.CategoryData.CatItem>();
                foreach (var i in items)
                {
                    try
                    {
                        var icon = i.Icon;
                        convItems.Add(new OutputData.CategoryData.CatItem()
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Description = i.Description,
                            Icon = icon == null ? null : "data:image/png;base64," + Convert.ToBase64String(BitmapToBytes(icon))
                        });
                    }
                    catch { }
                }
                category.Items = convItems.ToArray();

                catdata.Add(category);
            }

            data.Categories = catdata.ToArray();
            File.WriteAllText(@".\" + REGION + ".json", JsonConvert.SerializeObject(data));
        }

        public static byte[] BitmapToBytes(System.Drawing.Bitmap bmp, System.Drawing.Imaging.ImageFormat format = null)
        {
            if (format == null)
                format = System.Drawing.Imaging.ImageFormat.Png;

            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, format);
                return ms.ToArray();
            }
        }

        public class OutputData
        {
            [JsonProperty("region")]
            public string Region { get; set; }

            [JsonProperty("version")]
            public string Version { get; set; }

            [JsonProperty("categories")]
            public CategoryData[] Categories { get; set; }

            public class CategoryData
            {
                [JsonProperty("category")]
                public string Category { get; set; }

                [JsonProperty("items")]
                public CatItem[] Items { get; set; }


                public class CatItem
                {
                    [JsonProperty("id")]
                    public string Id { get; set; }

                    [JsonProperty("name")]
                    public string Name { get; set; }

                    [JsonProperty("description")]
                    public string Description { get; set; }

                    [JsonProperty("icon")]
                    public string Icon { get; set; }
                }
            }
        }
    }
}
