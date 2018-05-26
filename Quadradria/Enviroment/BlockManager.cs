using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Enviroment
{
    enum BlockType : ushort
    {
        Error = 0,
        Air = 1,
        Dirt = 2,
        Stone = 3,
        Wool = 4,
        Cloud = 5,
        Water = 10,
        OreCopper = 20,
        OreTin = 21,
        GrassBush = 40,
        DoorWood = 100
    }

    class BlockManager
    {
        public static BlockTypeDefault[] BlockTypeList = new BlockTypeDefault[Enum.GetValues(typeof(BlockType)).Cast<ushort>().Max()+1];

        public static void Init()
        {
            BlockTypeList[(int)BlockType.Error] = new BlockTypeDefault(BlockType.Error, "error", Textures.Error);
            BlockTypeList[(int)BlockType.Air] = new BlockTypeAir(BlockType.Air, "air");
            BlockTypeList[(int)BlockType.Dirt] = new BlockTypeGrass(BlockType.Dirt, "dirt", Textures.Blocks.Dirt, Textures.Blocks.Grass);
            BlockTypeList[(int)BlockType.Stone] = new BlockTypeDefault(BlockType.Stone, "stone", Textures.Blocks.Stone);
            BlockTypeList[(int)BlockType.DoorWood] = new BlockTypeDoor(BlockType.DoorWood, "doorWood", Textures.Blocks.DoorWood);
            BlockTypeList[(int)BlockType.Wool] = new BlockTypeDefault(BlockType.Wool, "wool", Textures.Blocks.Wool);
            BlockTypeList[(int)BlockType.OreCopper] = new BlockTypeDefault(BlockType.OreCopper, "oreCopper", Textures.Blocks.OreCopper);
            BlockTypeList[(int)BlockType.OreTin] = new BlockTypeDefault(BlockType.OreTin, "oreTin", Textures.Blocks.OreTin);
            BlockTypeList[(int)BlockType.Water] = new BlockTypeFluid(BlockType.Water, "water", Textures.Blocks.Water);
            BlockTypeList[(int)BlockType.GrassBush] = new BlockTypeFlower(BlockType.Water, "grassBush", Textures.Blocks.GrassBush);
            BlockTypeList[(int)BlockType.Cloud] = new BlockTypeDefault(BlockType.Cloud, "cloud", Textures.Blocks.Cloud, false);
        }
    }
}
