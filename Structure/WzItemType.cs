using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Structure
{
    public abstract class WzItemType
    {
        public string FriendlyName;
        public string Name;

        public WzItemType(string name)
        {
            FriendlyName = name;
            Name = name;
        }
        public WzItemType(string name, string friendlyName)
        {
            FriendlyName = friendlyName;
            Name = name;
        }

        public override string ToString()
        {
            return FriendlyName;
        }

        public abstract List<WzImageProperty> GetAllStringIdProperties(Dictionary<string, WzFile> wzFiles);
        public abstract WzImageProperty GetStringPropertyById(Dictionary<string, WzFile> wzFiles, string id);
        public abstract WzImageProperty GetInfoPropertyById(Dictionary<string, WzFile> wzFiles, string id);

        public virtual string GetDescription(WzImageProperty stringProp, WzImageProperty infoProp)
        {
            var d = stringProp["desc"];
            if (d != null)
                return d.GetString();
            return null;
        }
    }
}
