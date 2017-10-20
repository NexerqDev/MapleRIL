using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Windows.Structure
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
            WzImage itemImg = dir.GetImageByName(paddedId + ".img");
            return itemImg["info"];
        }

        public override WzImageProperty GetStringPropertyById(Dictionary<string, WzFile> wzFiles, string id)
        {
            return wzFiles["String.wz"].WzDirectory.GetImageByName("Eqp.img")["Eqp"][Name][id];
        }

        public override string GetDescription(WzImageProperty stringProp, WzImageProperty infoProp)
        {
            string desc = "WARNING: Equip stats are still WIP - a lot of stats may be missing!\n\n";
            desc += "REQ LEV: " + (infoProp["reqLevel"] == null ? "0" : infoProp["reqLevel"].GetInt().ToString());
            desc += "\n";
            desc += "REQ STR: " + (infoProp["reqSTR"] == null ? "0" : infoProp["reqSTR"].GetInt().ToString());
            desc += "   ";
            desc += "REQ LUK: " + (infoProp["reqLUK"] == null ? "0" : infoProp["reqLUK"].GetInt().ToString());
            desc += "\n";
            desc += "REQ DEX: " + (infoProp["reqDEX"] == null ? "0" : infoProp["reqDEX"].GetInt().ToString());
            desc += "   ";
            desc += "REQ INT: " + (infoProp["reqINT"] == null ? "0" : infoProp["reqINT"].GetInt().ToString());
            desc += "\n";
            desc += (Name == "Weapon") ? (getFriendlyWeaponAttackSpeed(infoProp) + "\n") : "";
            desc += ifIntExistsOutputFormat(infoProp, "incSTR", "STR: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incDEX", "DEX: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incINT", "INT: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incLUK", "LUK: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incPAD", "WEAPON ATTACK: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incMAD", "MAGIC ATTACK: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incACC", "ACCURACY: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "bdR", "BOSS DAMAGE: +{0}%\n");
            desc += ifIntExistsOutputFormat(infoProp, "imdR", "IED: +{0}%\n");
            desc += ifIntExistsOutputFormat(infoProp, "charmEXP", "CHARM EXP ON FIRST EQUIP: +{0}%\n");
            desc = desc.Trim();

            return desc;
        }

        // eg pass in (info prop, "incLUK", "LUK: +{0}\n") -> formatted as LUK: +20\n if the value was 20
        // if no prop then returns empty string
        private string ifIntExistsOutputFormat(WzImageProperty infoProp, string propToCheck, string formatString)
        {
            WzImageProperty prop = infoProp[propToCheck];
            if (prop == null)
                return "";

            return String.Format(formatString, prop.GetInt().ToString());
        }

        private string getFriendlyWeaponAttackSpeed(WzImageProperty infoProp)
        {
            string friendlyAtkSpd;
            if (infoProp["attackSpeed"] != null)
            {
                switch (infoProp["attackSpeed"].GetInt())
                {
                    case 2:
                        friendlyAtkSpd = "Faster (2)"; break;
                    case 3:
                        friendlyAtkSpd = "Faster (3)"; break;
                    case 4:
                        friendlyAtkSpd = "Fast (4)"; break;
                    case 5:
                        friendlyAtkSpd = "Fast (5)"; break;
                    case 6:
                        friendlyAtkSpd = "Normal (6)"; break;
                    default:
                        friendlyAtkSpd = infoProp["attackSpeed"].GetInt().ToString(); break;
                }
            }
            else
            {
                return "";
            }
            return friendlyAtkSpd;
        }
    }
}
