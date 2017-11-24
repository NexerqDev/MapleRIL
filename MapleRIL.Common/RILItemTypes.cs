using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapleRIL.Common.RILItemType;

namespace MapleRIL.Common
{
    public static class RILItemTypes
    {
        public static List<RILBaseItemType> GetAllItemTypes(RILFileManager rfm)
        {
            var items = new List<RILBaseItemType> // default Item.wz types
            {
                new RILItemItemType("Consume"),
                new RILEtcItemItemType(),
                new RILPetItemItemType(),
                new RILItemItemType("Cash"),
                new RILSetupItemItemType()
            };

            IEnumerable<RILBaseItemType> equipProps = rfm["String.wz"].WzDirectory
                .GetImageByName("Eqp.img")["Eqp"].WzProperties
                .Select(w => w.Name)
                .Select(w => new RILEquipItemType(w));

            return items.Concat(equipProps).ToList();
        }
    }
}
