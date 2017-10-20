using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Windows.Structure
{
    public class SearchedItem
    {
        public WzImageProperty SourceStringWzProperty; // this is the overall property, its name is the items id

        public string Id => SourceStringWzProperty.Name;
        public string Name => SourceStringWzProperty["name"].GetString();
        public string Category => ItemType.FriendlyName;

        public WzItemType ItemType { get; private set; }

        public SearchedItem(WzImageProperty wzProp, WzItemType category)
        {
            SourceStringWzProperty = wzProp;
            ItemType = category;
        }
    }
}
