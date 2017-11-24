using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Common.RILItemType
{
    public class RILEtcItemItemType : RILItemItemType
    {
        public RILEtcItemItemType() : base("Etc") { }

        // for some reason etc is weird
        public override List<WzImageProperty> GetAllStringIdProperties(RILFileManager files)
        {
            // String.wz/Etc.img/Etc/id
            return files["String.wz"].WzDirectory.GetImageByName(Name + ".img")["Etc"].WzProperties;
        }
    }
}
