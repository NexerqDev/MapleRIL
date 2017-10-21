using MapleLib.WzLib;
using MapleRIL.Common.RILItemType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Common
{
    public class RILItem
    {
        public RILFileManager OriginatingFileManager;

        public WzImageProperty StringWzProperty; // this is the overall property, its name is the items id

        private WzImageProperty _iwp = null;
        public WzImageProperty InfoWzProperty
        {
            get
            {
                if (_iwp == null)
                    _iwp = ItemType.GetInfoPropertyById(OriginatingFileManager, Id);
                return _iwp;
            }
        }

        public RILBaseItemType ItemType { get; private set; }

        public string Id => StringWzProperty.Name;
        public string Name => StringWzProperty["name"].GetString();
        public string Category => ItemType.FriendlyName;
        public string Description => ItemType.GetDescription(StringWzProperty, InfoWzProperty);
        public System.Drawing.Bitmap Icon => ItemType.GetIcon(InfoWzProperty);

        // take the rfm as well to load info stuff for this item
        public RILItem(RILFileManager rfm, WzImageProperty wzStrProp, RILBaseItemType type)
        {
            OriginatingFileManager = rfm;
            StringWzProperty = wzStrProp;
            ItemType = type;
        }

        public RILItem TryLoadInOtherFileManager(RILFileManager rfm)
        {
            WzImageProperty remoteProp = ItemType.GetStringPropertyById(rfm, Id);
            if (remoteProp == null)
                return null;
            return new RILItem(rfm, remoteProp, ItemType);
        }
    }
}
