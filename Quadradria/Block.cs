using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria
{
    class Block
    {
        private Texture2D _texture;
        public Vector2 _postion;

        public void Update()
        {

        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _postion, Color.White);
        }
    }
}
