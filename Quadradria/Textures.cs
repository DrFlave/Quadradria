using Microsoft.Xna.Framework;
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

        public static class Fonts
        {
            public static SpriteFont Inventory;

            public static void Load(ContentManager content)
            {
                Inventory = content.Load<SpriteFont>("fontInventory");
            }
        }

        public static class Items
        {
            public static Texture2D Sword;

            public static void Load(ContentManager content)
            {
                Sword = content.Load<Texture2D>("itemSword");
            }
        }

        public static Texture2D Solid; //A 1x1 pixel white texture (set in Quadradian.cs)

        public static void Load(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Error = content.Load<Texture2D>("Error");

            Textures.Solid = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Textures.Solid.SetData<Color>(new Color[1] { Color.White });

            Blocks.Load(content);
            Fonts.Load(content);
            Items.Load(content);
        }
    }
}
