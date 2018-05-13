using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Enviroment
{

    enum BlockType : ushort
    {
        Air = 0,
        Dirt = 1,
        Stone = 2,
        Grass = 3,
        DoorWood = 4
    }

    struct Block
    {
        public BlockType BlockID;
        public ushort SubID;
        public byte damage;

        public Block(BlockType blockID, ushort subID) {
            BlockID = blockID;
            SubID = subID;
            damage = 0;
        }

        public void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            BlockTypeDefault typeInst = BlockTypeDefault.BlockTypeList[(uint)BlockID];

            if (typeInst != null)
                typeInst.Draw(spriteBatch, x, y, this);

        }
    }

    class BlockTypeDefault
    {
        public static BlockTypeDefault[] BlockTypeList = new BlockTypeDefault[Enum.GetNames(typeof(BlockType)).Length];

        public static void InitBlockTypes()
        {
            BlockTypeList[(int)BlockType.Air] = (new BlockTypeDefault(BlockType.Air, "air", Textures.Solid));
            BlockTypeList[(int)BlockType.Dirt] = (new BlockTypeDefault(BlockType.Dirt, "dirt", Textures.Blocks.Dirt));
            BlockTypeList[(int)BlockType.Stone] = (new BlockTypeDefault(BlockType.Stone, "stone", Textures.Blocks.Stone));
            BlockTypeList[(int)BlockType.Grass] = new BlockTypeGrass(BlockType.Grass, "grass", Textures.Blocks.Dirt);
            BlockTypeList[(int)BlockType.DoorWood] = (new BlockTypeDoor(BlockType.DoorWood, "doorWood", Textures.Blocks.DoorWood));
        }

        protected string name;
        protected Texture2D texture;
        protected BlockType type;

        public BlockTypeDefault(BlockType type, string name, Texture2D texture)
        {
            this.type = type;
            this.name = name;
            this.texture = texture;
        }

        public virtual void Draw(SpriteBatch spriteBatch, int x, int y, Block block)
        {
            spriteBatch.Draw(texture, new Vector2(x, y), Color.White);
        }

    }

    class BlockTypeDoor : BlockTypeDefault
    {
        private Rectangle rectClosed = new Rectangle(0, 0, 32, 32);
        private Rectangle rectOpen = new Rectangle(32, 0, 32, 32);

        public BlockTypeDoor(BlockType type, string name, Texture2D texture) : base(type, name, texture) { }
        public override void Draw(SpriteBatch spriteBatch, int x, int y, Block block)
        {
            bool open = block.SubID > 0;
            spriteBatch.Draw(texture, new Vector2(x, y), open ? rectOpen : rectClosed, Color.White);
        }
    }

    class BlockTypeGrass : BlockTypeDefault
    {
        public BlockTypeGrass(BlockType type, string name, Texture2D texture) : base(type, name, texture) { }
        public override void Draw(SpriteBatch spriteBatch, int x, int y, Block block)
        {
            base.Draw(spriteBatch, x, y, block);

            bool right =  ((block.SubID & 0b0001) > 0);
            bool top =    ((block.SubID & 0b0010) > 0);
            bool left =   ((block.SubID & 0b0100) > 0);
            bool bottom = ((block.SubID & 0b1000) > 0);

        }
    }
}
