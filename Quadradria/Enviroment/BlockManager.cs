using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Enviroment
{
    class BlockManager
    {
        public static BlockTypeDefault[] BlockTypeList = new BlockTypeDefault[Enum.GetValues(typeof(BlockType)).Cast<ushort>().Max()+1];

        public static void Init()
        {
            BlockTypeList[(int)BlockType.Air] = (new BlockTypeAir(BlockType.Air, "air"));
            BlockTypeList[(int)BlockType.Dirt] = (new BlockTypeGrass(BlockType.Dirt, "dirt", Textures.Blocks.Dirt, Textures.Blocks.Grass));
            BlockTypeList[(int)BlockType.Stone] = (new BlockTypeDefault(BlockType.Stone, "stone", Textures.Blocks.Stone));
            BlockTypeList[(int)BlockType.DoorWood] = (new BlockTypeDoor(BlockType.DoorWood, "doorWood", Textures.Blocks.DoorWood));
        }
    }
}
