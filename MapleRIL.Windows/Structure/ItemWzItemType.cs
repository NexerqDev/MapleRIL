using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapleLib.WzLib;

namespace MapleRIL.Windows.Structure
{
    public class ItemWzItemType : WzItemType
    {
        public ItemWzItemType(string name) : base(name) { }
        public ItemWzItemType(string name, string friendlyName) : base(name, friendlyName) { }

        public override List<WzImageProperty> GetAllStringIdProperties(Dictionary<string, WzFile> wzFiles)
        {
            // String.wz/Consume.img/ID
            return wzFiles["String.wz"].WzDirectory.GetImageByName(Name + ".img").WzProperties;
        }

        public override WzImageProperty GetInfoPropertyById(Dictionary<string, WzFile> wzFiles, string id)
        {
            // Item.wz/Consume/0200.img/02000000
            string paddedId = "0" + id;
            WzDirectory dir = wzFiles["Item.wz"].WzDirectory[Name] as WzDirectory;
            WzImage itemImg = dir.GetImageByName(paddedId.Substring(0, 4) + ".img");
            return itemImg[paddedId]["info"];
        }

        public override WzImageProperty GetStringPropertyById(Dictionary<string, WzFile> wzFiles, string id)
        {
            return wzFiles["String.wz"].WzDirectory.GetImageByName(Name + ".img")[id];
        }
    }
}
