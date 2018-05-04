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
            public static Texture2D Stone;
            public static Texture2D WoolRed;
            public static Texture2D WoolYellow;
            public static Texture2D WoolGreen;
            public static Texture2D WoolCyan;
            public static Texture2D WoolPurple;

            public static void Load(ContentManager content)
            {
                Dirt = content.Load<Texture2D>("Dirt");
                Stone = content.Load<Texture2D>("Stone");
                WoolRed = content.Load<Texture2D>("WoolRed");
                WoolYellow = content.Load<Texture2D>("WoolYellow");
                WoolGreen = content.Load<Texture2D>("WoolGreen");
                WoolCyan = content.Load<Texture2D>("WoolCyan");
                WoolPurple = content.Load<Texture2D>("WoolPurple");
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

        public static Texture2D Solid; //A 1x1 pixel white texture

        public static void Load(ContentManager content)
        {
            Blocks.Load(content);
            Fonts.Load(content);
            Items.Load(content);
            Error = content.Load<Texture2D>("Error");
        }
    }
}
