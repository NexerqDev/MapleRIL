using MapleLib.WzLib;
using MapleRIL.Common.RILItemType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MapleRIL.Common
{
    public class RILSearcher
    {
        private RILFileManager _fm;
        public RILFileManager FileManager
        {
            get
            {
                return _fm;
            }
            set
            {
                _fm = value;
                ItemTypes = RILItemTypes.GetAllItemTypes(_fm);
            }
        }
        public List<RILBaseItemType> ItemTypes { get; set; }

        public RILSearcher(RILFileManager rfm)
        {
            FileManager = rfm;
        }

        public RILItem[] Search(string query)
        {
            var res = new List<RILItem>();
            foreach (RILBaseItemType it in ItemTypes)
                res.AddRange(SearchInType(it, query));

            return res.ToArray();
        }

        public RILItem[] SearchInType(RILBaseItemType type, string query)
        {
            List<WzImageProperty> searchProperties = type.GetAllStringIdProperties(FileManager); // these are the properties we will be looping for the item names

            // look up a property in the image, eg 2000000 where inside the property "name"'s string is what the user is looking for.
            // loose search so use regexes
            Regex r = new Regex("(^| )" + query + "($| |')", RegexOptions.IgnoreCase);
            IEnumerable<WzImageProperty> props = searchProperties.Where(w => {
                var nameProp = w.WzProperties.Where(p => p.Name == "name");
                if (nameProp.Count() < 1)
                    return false;

                return r.IsMatch(nameProp.First().GetString());
            });

            return props.Select(p => new RILItem(FileManager, p, type)).ToArray();
        }
    }
}
