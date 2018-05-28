using Microsoft.Xna.Framework;
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
            AddBlockType(new BlockTypeDefault(BlockType.Error, "error", Textures.Error));
            AddBlockType(new BlockTypeAir(BlockType.Air, "air"));
            AddBlockType(new BlockTypeGrass(BlockType.Dirt, "dirt", Textures.Blocks.Dirt, Textures.Blocks.Grass));
            AddBlockType(new BlockTypeDefault(BlockType.Stone, "stone", Textures.Blocks.Stone));
            AddBlockType(new BlockTypeDoor(BlockType.DoorWood, "doorWood", Textures.Blocks.DoorWood));
            AddBlockType(new BlockTypeDefault(BlockType.Wool, "wool", Textures.Blocks.Wool));
            AddBlockType(new BlockTypeDefault(BlockType.OreCopper, "oreCopper", Textures.Blocks.OreCopper, true, Color.Orange));
            AddBlockType(new BlockTypeDefault(BlockType.OreTin, "oreTin", Textures.Blocks.OreTin, true, Color.Silver));
            AddBlockType(new BlockTypeFluid(BlockType.Water, "water", Textures.Blocks.Water));
            AddBlockType(new BlockTypeFlower(BlockType.GrassBush, "grassBush", Textures.Blocks.GrassBush));
            AddBlockType(new BlockTypeDefault(BlockType.Cloud, "cloud", Textures.Blocks.Cloud, false));
        }

        public static void AddBlockType(BlockTypeDefault blockType)
        {
            BlockTypeList[(ushort)blockType.GetBlockType()] = blockType;
        }
    }
}
