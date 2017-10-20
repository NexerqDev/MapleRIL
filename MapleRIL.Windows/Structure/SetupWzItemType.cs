using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Windows.Structure
{
    public class SetupItemWzItemType : ItemWzItemType
    {
        public SetupItemWzItemType() : base("Ins", "Setup") { }

        public override WzImageProperty GetInfoPropertyById(Dictionary<string, WzFile> wzFiles, string id)
        {
            // Ins -> Install
            string paddedId = "0" + id;
            WzDirectory dir = wzFiles["Item.wz"].WzDirectory["Install"] as WzDirectory;
            WzImage itemImg = dir.GetImageByName(paddedId.Substring(0, 4) + ".img");
            return itemImg[paddedId]["info"];
        }
    }
}
