using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Common.RILItemType
{
    public class RILPetItemItemType : RILItemItemType
    {
        public RILPetItemItemType() : base("Pet") { }

        public override WzImageProperty GetInfoPropertyById(RILFileManager rfm, string id)
        {
            // pets are special for some reason
            // Item.wz/Pet/02000000.img
            WzDirectory dir = rfm["Item.wz"].WzDirectory[Name] as WzDirectory;
            WzImage petImg = dir.GetImageByName(id + ".img");
            return petImg["info"];
        }
    }
}
