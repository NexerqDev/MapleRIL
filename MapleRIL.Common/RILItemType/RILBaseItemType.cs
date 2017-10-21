using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Common.RILItemType
{
    public abstract class RILBaseItemType
    {
        public string FriendlyName;
        public string Name;

        public RILBaseItemType(string name)
        {
            FriendlyName = name;
            Name = name;
        }
        public RILBaseItemType(string name, string friendlyName)
        {
            FriendlyName = friendlyName;
            Name = name;
        }

        public override string ToString()
        {
            return FriendlyName;
        }

        public abstract List<WzImageProperty> GetAllStringIdProperties(RILFileManager files);
        public abstract WzImageProperty GetStringPropertyById(RILFileManager files, string id);
        public abstract WzImageProperty GetInfoPropertyById(RILFileManager files, string id);

        public virtual string GetDescription(WzImageProperty stringProp, WzImageProperty infoProp)
        {
            var d = stringProp["desc"];
            if (d != null)
                return d.GetString();
            return null;
        }

        public virtual Bitmap GetIcon(WzImageProperty infoProp)
        {
            try
            {
               return infoProp["icon"].GetBitmap();
            }
            catch
            {
                return null;
            }
        }
    }
}
