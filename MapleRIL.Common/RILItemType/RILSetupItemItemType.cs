using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Common.RILItemType
{
    public class RILSetupItemItemType : RILItemItemType
    {
        public RILSetupItemItemType() : base("Ins", "Setup") { }

        public override WzImageProperty GetInfoPropertyById(RILFileManager rfm, string id)
        {
            // Ins -> Install
            string paddedId = "0" + id;
            WzDirectory dir = rfm["Item.wz"].WzDirectory["Install"] as WzDirectory;
            WzImage itemImg = dir.GetImageByName(paddedId.Substring(0, 4) + ".img");
            return itemImg[paddedId]["info"];
        }
    }
}
