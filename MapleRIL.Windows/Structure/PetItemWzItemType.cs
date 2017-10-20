using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Windows.Structure
{
    public class PetItemWzItemType : ItemWzItemType
    {
        public PetItemWzItemType() : base("Pet") { }

        public override WzImageProperty GetInfoPropertyById(Dictionary<string, WzFile> wzFiles, string id)
        {
            // pets are special for some reason
            // Item.wz/Pet/02000000.img
            WzDirectory dir = wzFiles["Item.wz"].WzDirectory[Name] as WzDirectory;
            WzImage petImg = dir.GetImageByName(id + ".img");
            return petImg["info"];
        }
    }
}
