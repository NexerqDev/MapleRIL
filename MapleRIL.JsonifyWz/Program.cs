using MapleLib.WzLib;
using MapleRIL.Common;
using MapleRIL.Common.RILItemType;
using MapleRIL.Common.RILJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.JsonifyWz
{
    class Program
    {
        static string Region;
        static string GamePath;

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Invalid args; MapleRIL.JsonifyWz [region name] [game path]");
                return;
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Region = args[0];
            GamePath = args[1];

            // load wz
            var rfm = new RILFileManager(Region, GamePath);
            var typs = RILItemTypes.GetAllItemTypes(rfm);
            Console.WriteLine("Loaded region data");

            var data = new RILJsonFormat();
            data.Region = Region;
            data.Version = rfm.GameVersion;

            var catdata = new List<RILJsonFormat.CategoryData>();
            var c = 1;
            foreach (RILBaseItemType typ in typs)
            {
                Console.WriteLine("Writing " + typ.FriendlyName + $" ({c}/{typs.Count})");
                List<WzImageProperty> props = typ.GetAllStringIdProperties(rfm);

                var items = props.Select(p => new RILItem(rfm, p, typ));
                var category = new RILJsonFormat.CategoryData();
                category.Category = typ.FriendlyName;

                var convItems = new List<RILJsonFormat.CategoryData.CatItem>();
                foreach (var i in items)
                {
                    try
                    {
                        var icon = i.Icon;
                        convItems.Add(new RILJsonFormat.CategoryData.CatItem()
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Description = i.ParsedDescription,
                            Icon = icon == null ? null : "data:image/png;base64," + Convert.ToBase64String(BitmapToBytes(icon))
                        });
                    }
                    catch { }
                }
                category.Items = convItems.ToArray();

                catdata.Add(category);
                c++;
            }

            data.Categories = catdata.ToArray();
            File.WriteAllText(@".\" + Region + ".json", JsonConvert.SerializeObject(data));
            Console.WriteLine($"Wrote to {Region}.json");
            Console.WriteLine("Took " + stopwatch.Elapsed.ToString("c"));
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
    }
}
