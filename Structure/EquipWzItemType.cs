using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Structure
{
    public class EquipWzItemType : WzItemType
    {
        public EquipWzItemType(string name) : base(name) { }
        public EquipWzItemType(string name, string friendlyName) : base(name, friendlyName) { }

        public override List<WzImageProperty> GetAllStringIdProperties(Dictionary<string, WzFile> wzFiles)
        {
            // Eqp.img/Eqp/Accessory/ID
            return wzFiles["String.wz"].WzDirectory.GetImageByName("Eqp.img")["Eqp"][Name].WzProperties;
        }

        public override WzImageProperty GetInfoPropertyById(Dictionary<string, WzFile> wzFiles, string id)
        {
            // Character.wz/Accessory/01010000.img
            string paddedId = "0" + id;
            WzDirectory dir = wzFiles["Character.wz"].WzDirectory[Name] as WzDirectory;
            WzImage itemImg = dir.GetImageByName(id + ".img");
            return itemImg["info"];
        }

        public override WzImageProperty GetStringPropertyById(Dictionary<string, WzFile> wzFiles, string id)
        {
            return wzFiles["String.wz"].WzDirectory.GetImageByName("Eqp.img")["Eqp"][Name][id];
        }
    }
}
