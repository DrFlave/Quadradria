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

    struct Block
    {
        public BlockType BlockID;
        public ushort SubID;
        public byte Damage;

        public Block(BlockType blockID, ushort subID) {
            BlockID = blockID;
            SubID = subID;
            Damage = 0;
        }

        public void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            BlockTypeDefault typeInst = BlockManager.BlockTypeList[(uint)BlockID];

            if (typeInst != null)
                typeInst.Draw(spriteBatch, x, y, this);

        }

        public void RandomTick(int x, int y, World world)
        {
            BlockTypeDefault typeInst = BlockManager.BlockTypeList[(uint)BlockID];

            if (typeInst != null)
                typeInst.RandomTick(x, y, this, world);
        }
    }
    
    class BlockTypeDefault
    {
        protected string name;
        protected Texture2D texture;
        protected BlockType type;

        public BlockTypeDefault(BlockType type, string name, Texture2D texture)
        {
            this.type = type;
            this.name = name;
            this.texture = texture;
        }

        public virtual void RandomTick(int x, int y, Block block, World world) //will be triggered randomly. Used for plant grow and stuff.
        {
            
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

        public override void RandomTick(int x, int y, Block block, World world)
        {
            base.RandomTick(x, y, block, world);

            ushort subid = block.SubID;

            Block?[] nexts = new Block?[] {
                world.GetBlockAtPosition(x - 1, y - 1), world.GetBlockAtPosition(x , y - 1), world.GetBlockAtPosition(x + 1, y - 1),
                world.GetBlockAtPosition(x - 1, y    ),                                      world.GetBlockAtPosition(x + 1, y    ),
                world.GetBlockAtPosition(x - 1, y + 1), world.GetBlockAtPosition(x , y + 1), world.GetBlockAtPosition(x + 1, y + 1)
            };

            foreach(Block? b in nexts)
            {
                if (b != null)
                subid |= (ushort)(b?.SubID);
            }

            if (subid > 0) subid = 0b1111;

            /*
            bool gRight = ((subid & 0b0001) > 0);
            bool gTop = ((subid & 0b0010) > 0);
            bool gLeft = ((subid & 0b0100) > 0);
            bool gBottom = ((subid & 0b1000) > 0);
            */

            bool bRight = nexts[4]?.BlockID != BlockType.Air;
            bool bTop = nexts[1]?.BlockID != BlockType.Air;
            bool bLeft = nexts[3]?.BlockID != BlockType.Air;
            bool bBottom = nexts[6]?.BlockID != BlockType.Air;

            if (bRight) subid &= 0b1110;
            if (bTop) subid &= 0b1101;
            if (bLeft) subid &= 0b1011;
            if (bBottom) subid &= 0b0111;

            world.SetBlockAtPosition(x, y, type, subid);
        }
    }

    class BlockTypeAir : BlockTypeDefault
    {
        public BlockTypeAir(BlockType type, string name) : base(type, name, null) { }
        public override void Draw(SpriteBatch spriteBatch, int x, int y, Block block)
        {

        }
    }
}
