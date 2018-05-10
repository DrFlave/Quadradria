using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Enviroment
{
    class Block
    {
        private Texture2D texture;
        public short blockID = 0x1234;
        public short subID = 0x5678;

        public Block(Texture2D texture)
        {
            //texture = Textures.Blocks.Dirt;
            this.texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(texture, new Vector2(x, y), Color.White);
        }
    }
}
