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
        DoorWood = 3
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

        public bool RandomTick()
        {
            BlockTypeDefault typeInst = BlockTypeDefault.BlockTypeList[(uint)BlockID];

            if (typeInst != null)
                return typeInst.RandomTick(this);
            return false;
        }
    }
    
    class BlockTypeDefault
    {
        public static BlockTypeDefault[] BlockTypeList = new BlockTypeDefault[Enum.GetNames(typeof(BlockType)).Length];

        public static void InitBlockTypes()
        {
            BlockTypeList[(int)BlockType.Air] = (new BlockTypeAir(BlockType.Air, "air"));
            BlockTypeList[(int)BlockType.Dirt] = (new BlockTypeGrass(BlockType.Dirt, "dirt", Textures.Blocks.Dirt, Textures.Blocks.Grass));
            BlockTypeList[(int)BlockType.Stone] = (new BlockTypeDefault(BlockType.Stone, "stone", Textures.Blocks.Stone));
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

        public virtual bool RandomTick(Block block) //will be triggered randomly. Used for plant grow and stuff.
        {
            return false;
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
        private Rectangle sourceRight = new Rectangle(0, 0, 16, 16);
        private Rectangle sourceTop = new Rectangle(16, 0, 16, 16);
        private Rectangle sourceLeft = new Rectangle(32, 0, 16, 16);
        private Rectangle sourceBottom = new Rectangle(48, 0, 16, 16);

        private Texture2D textureFoliage;

        public BlockTypeGrass(BlockType type, string name, Texture2D textureBase, Texture2D textureFoliage) : base(type, name, textureBase) {
            this.textureFoliage = textureFoliage;
        }
        public override void Draw(SpriteBatch spriteBatch, int x, int y, Block block)
        {
            base.Draw(spriteBatch, x, y, block);

            bool right =  ((block.SubID & 0b0001) > 0);
            bool top =    ((block.SubID & 0b0010) > 0);
            bool left =   ((block.SubID & 0b0100) > 0);
            bool bottom = ((block.SubID & 0b1000) > 0);

            if (right) spriteBatch.Draw(textureFoliage, new Vector2(x, y), sourceRight, Color.White);
            if (top) spriteBatch.Draw(textureFoliage, new Vector2(x, y), sourceTop, Color.White);
            if (left) spriteBatch.Draw(textureFoliage, new Vector2(x, y), sourceLeft, Color.White);
            if (bottom) spriteBatch.Draw(textureFoliage, new Vector2(x, y), sourceBottom, Color.White);
        }

        public override bool RandomTick(Block block)
        {
            base.RandomTick(block);

            block.SubID = 0b1111;
            return true;
        }
    }

    class BlockTypeAir : BlockTypeDefault
    {
        public BlockTypeAir(BlockType type, string name) : base(type, name, null) { }
        public override void Draw(SpriteBatch spriteBatch, int x, int y, Block block)
        {

        }

        public override bool RandomTick(Block block)
        {
            base.RandomTick(block);
            block.BlockID = BlockType.DoorWood;
            return true;
        }
    }
}
