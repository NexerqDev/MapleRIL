using MapleLib.WzLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MapleRIL.Common.RILJson
{
    public class RILJsonEngine
    {
        public string Region;
        public string JsonPath;
        public RILJsonFormat Data;
        public string[] Categories;

        public RILJsonEngine(string region, string path)
        {
            Region = region;
            JsonPath = path;
            Data = JsonConvert.DeserializeObject<RILJsonFormat>(File.ReadAllText(path));
            Categories = Data.Categories.Select(c => c.Category).ToArray();
        }

        private Regex nonEnglishCharacters = new Regex("[^\x00-\x7F]");
        public RILJsonItem[] Search(string query)
        {
            // loose search so use regexes
            // dont match strictly for non-english characters with boundaries
            Regex r;
            if (nonEnglishCharacters.IsMatch(query))
                r = new Regex(Regex.Escape(query), RegexOptions.IgnoreCase);
            else
                r = new Regex(@"\b" + Regex.Escape(query) + @"\b", RegexOptions.IgnoreCase);

            List<RILJsonItem> items = new List<RILJsonItem>();
            foreach (var cat in Data.Categories)
            {
                foreach (var item in cat.Items)
                {
                    if (r.IsMatch(item.Name))
                    {
                        items.Add(new RILJsonItem()
                        {
                            Category = cat.Category,
                            Id = item.Id,
                            Description = item.Description,
                            Icon = item.Icon,
                            Name = item.Name
                        });
                    }
                }
            }

            return items.ToArray();
        }

        public RILJsonItem GetItemById(string id)
        {
            foreach (var cat in Data.Categories)
            {
                foreach (var item in cat.Items)
                {
                    if (item.Id == id)
                        return new RILJsonItem()
                        {
                            Category = cat.Category,
                            Id = item.Id,
                            Description = item.Description,
                            Icon = item.Icon,
                            Name = item.Name
                        };
                }
            }

            return null;
        }
    }
}
