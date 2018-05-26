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

            typeInst?.Draw(spriteBatch, x, y, this);

        }

        public void RandomTick(int x, int y, World world) //will be triggered randomly. Used for plant grow and stuff.
        {
            BlockTypeDefault typeInst = BlockManager.BlockTypeList[(uint)BlockID];

            typeInst?.RandomTick(x, y, this, world);
        }

        public void Update(int x, int y, World world) //triggered when a surrounding block changes
        {
            BlockTypeDefault typeInst = BlockManager.BlockTypeList[(uint)BlockID];

            typeInst?.Update(x, y, this, world);
        }

        public override string ToString()
        {
            return "" + BlockID + ":" + SubID;
        }

        public bool IsSolid()
        {
            BlockTypeDefault typeInst = BlockManager.BlockTypeList[(uint)BlockID];

            return (bool)typeInst?.IsSolid(this);
        }
    }
    
    //Default Block
    class BlockTypeDefault
    {
        protected string name;
        protected Texture2D texture;
        protected BlockType type;
        protected bool isSolid = true;

        public byte LightStrength = 0;
        public byte LightRed = 0;
        public byte LightGreen = 0;
        public byte LightBlue = 0;

        public virtual bool IsSolid(Block block)
        {
            return isSolid;
        }

        public BlockTypeDefault(BlockType type, string name, Texture2D texture, bool solid = true)
        {
            this.type = type;
            this.name = name;
            this.texture = texture;
            this.isSolid = solid;
        }

        public virtual void RandomTick(int x, int y, Block block, World world)
        {
            
        }

        public virtual void Update(int x, int y, Block block, World world)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch, int x, int y, Block block)
        {
            spriteBatch.Draw(texture, new Vector2(x, y), new Rectangle(x % texture.Width, y % texture.Height, 16, 16), Color.White);
        }

    }

    //Can be open or closed
    class BlockTypeDoor : BlockTypeDefault
    {
        private Rectangle rectClosed = new Rectangle(0, 0, 32, 32);
        private Rectangle rectOpen = new Rectangle(32, 0, 32, 32);

        public BlockTypeDoor(BlockType type, string name, Texture2D texture) : base(type, name, texture) { }

        public override bool IsSolid(Block block)
        {
            return block.SubID > 0;
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y, Block block)
        {
            bool open = block.SubID > 0;
            spriteBatch.Draw(texture, new Vector2(x, y), open ? rectOpen : rectClosed, Color.White);
        }
    }

    //Grows to other blocks
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
                else
                    return;
            }

            if (subid > 0) subid = 0b1111;

            /*
            bool gRight = ((subid & 0b0001) > 0);
            bool gTop = ((subid & 0b0010) > 0);
            bool gLeft = ((subid & 0b0100) > 0);
            bool gBottom = ((subid & 0b1000) > 0);
            */

            bool bRight = (bool)nexts[4]?.IsSolid();
            bool bTop = (bool)nexts[1]?.IsSolid();
            bool bLeft = (bool)nexts[3]?.IsSolid();
            bool bBottom = (bool)nexts[6]?.IsSolid();

            if (bRight) subid &= 0b1110;
            if (bTop) subid &= 0b1101;
            if (bLeft) subid &= 0b1011;
            if (bBottom) subid &= 0b0111;

            if (block.SubID != subid)
                world.SetBlockAtPosition(x, y, type, subid);
        }
    }

    //Does nothing
    class BlockTypeAir : BlockTypeDefault
    {
        public BlockTypeAir(BlockType type, string name) : base(type, name, null) {
            isSolid = false;
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y, Block block)
        {
        }
    }

    //Flows
    class BlockTypeFluid : BlockTypeDefault
    {
        public BlockTypeFluid(BlockType type, string name, Texture2D texture) : base(type, name, texture) { }
        public override void Draw(SpriteBatch spriteBatch, int x, int y, Block block)
        {
            base.Draw(spriteBatch, x, y, block);
        }
    }

    //Looks Nice
    class BlockTypeFlower : BlockTypeDefault
    {
        public BlockTypeFlower(BlockType type, string name, Texture2D texture) : base(type, name, texture) { }
        public override void Draw(SpriteBatch spriteBatch, int x, int y, Block block)
        {
            isSolid = false;
            base.Draw(spriteBatch, x, y, block);
        }

        public override void Update(int x, int y, Block block, World world)
        {
            Block? under = world.GetBlockAtPosition(x, y + 1);

            if (under == null || under?.BlockID != BlockType.Dirt)
            {
                world.SetBlockAtPosition(x, y, BlockType.Air, 0);
            }

            base.Update(x, y, block, world);
        }
    }

    //Grows
    class BlockTypePlant : BlockTypeDefault
    {
        public BlockTypePlant(BlockType type, string name, Texture2D texture) : base(type, name, texture) { }
        public override void Draw(SpriteBatch spriteBatch, int x, int y, Block block)
        {
            isSolid = false;
            base.Draw(spriteBatch, x, y, block);
        }

        public override void RandomTick(int x, int y, Block block, World world)
        {
            block.SubID++;
            base.RandomTick(x, y, block, world);
        }
    }
}
