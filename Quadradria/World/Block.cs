using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.World
{
    class Block
    {
        private Texture2D texture;

        public Block()
        {
            texture = Textures.Blocks.Dirt;
        }

        public void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(texture, new Vector2(x, y), Color.White);
        }
    }
}
