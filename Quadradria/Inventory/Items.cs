using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Inventory
{
    public enum ItemID : ushort
    {
        blockDirt = 1
    }

    static class Items
    {
        public static ItemType[] ItemTypes = new ItemType[Enum.GetValues(typeof(ItemID)).Cast<ushort>().Max() + 1];


        static Items()
        {
            AddItemType(new ItemType(ItemID.blockDirt, "dirt", 96, Textures.Blocks.Dirt));
        }

        static void AddItemType(ItemType itemType)
        {
            ItemTypes[(ushort)itemType.ID] = itemType;
        }
    }
}
