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
            public static Texture2D Stone;
            public static Texture2D Wool;
            public static Texture2D DoorWood;
            public static Texture2D Grass;
            public static Texture2D OreCopper;
            public static Texture2D OreTin;
            public static Texture2D Water;
            public static Texture2D GrassBush;

            public static void Load(ContentManager content)
            {
                Dirt = content.Load<Texture2D>("Blocks/Dirt");
                Stone = content.Load<Texture2D>("Blocks/Stone");
                Wool = content.Load<Texture2D>("Blocks/Wool");
                DoorWood = content.Load<Texture2D>("Blocks/Doors/Wood");
                Grass = content.Load<Texture2D>("Blocks/Grass");
                OreCopper = content.Load<Texture2D>("Blocks/OreCopper");
                OreTin = content.Load<Texture2D>("Blocks/OreTin");
                Water = content.Load<Texture2D>("Blocks/Water");
                GrassBush = content.Load<Texture2D>("Blocks/GrassBush");
            }
        }

        public static class Backgrounds
        {
            public static Texture2D Stone;
            public static Texture2D Dirt;

            public static void Load(ContentManager content)
            {
                Dirt = content.Load<Texture2D>("Backgrounds/Dirt");
                Stone = content.Load<Texture2D>("Backgrounds/Stone");
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

        public static class UI
        {
            public static Texture2D DropDownArrow;

            public static void Load(ContentManager content)
            {
                DropDownArrow = content.Load<Texture2D>("UI/DropDown_arrow");
            }
        }

        public static Texture2D Solid; //A 1x1 pixel white texture

        public static void Load(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Error = content.Load<Texture2D>("Error");

            Textures.Solid = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Textures.Solid.SetData<Color>(new Color[1] { Color.White });

            Blocks.Load(content);
            Backgrounds.Load(content);
            Fonts.Load(content);
            Items.Load(content);
            UI.Load(content);
        }
    }
}
