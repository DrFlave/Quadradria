using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria
{


    public static class Textures
    {
        public static Texture2D Error;

        public static class Blocks
        {
            public static Texture2D Dirt;

            public static void Load(ContentManager content)
            {
                Dirt = content.Load<Texture2D>("Dirt");
            }
        }

        public static void Load(ContentManager content)
        {
            Blocks.Load(content);
            Error = content.Load<Texture2D>("Error");
        }
    }
}
