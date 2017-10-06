using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Structure
{
    public class SearchedItem
    {
        public WzImageProperty SourceWzProperty; // this is the overall property, its name is the items id

        public string Id => SourceWzProperty.Name;
        public string Name => SourceWzProperty["name"].GetString();
        public string Category => ItemType.FriendlyName;

        public WzItemType ItemType { get; private set; }

        public SearchedItem(WzImageProperty wzProp, WzItemType category)
        {
            SourceWzProperty = wzProp;
            ItemType = category;
        }
    }
}
