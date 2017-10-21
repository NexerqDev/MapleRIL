using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapleLib.WzLib;

namespace MapleRIL.Common.RILItemType
{
    public class RILItemItemType : RILBaseItemType
    {
        public RILItemItemType(string name) : base(name) { }
        public RILItemItemType(string name, string friendlyName) : base(name, friendlyName) { }

        public override List<WzImageProperty> GetAllStringIdProperties(RILFileManager files)
        {
            // String.wz/Consume.img/ID
            return files["String.wz"].WzDirectory.GetImageByName(Name + ".img").WzProperties;
        }

        public override WzImageProperty GetInfoPropertyById(RILFileManager files, string id)
        {
            // Item.wz/Consume/0200.img/02000000
            string paddedId = "0" + id;
            WzDirectory dir = files["Item.wz"].WzDirectory[Name] as WzDirectory;
            WzImage itemImg = dir.GetImageByName(paddedId.Substring(0, 4) + ".img");
            return itemImg[paddedId]["info"];
        }

        public override WzImageProperty GetStringPropertyById(RILFileManager files, string id)
        {
            return files["String.wz"].WzDirectory.GetImageByName(Name + ".img")[id];
        }
    }
}
