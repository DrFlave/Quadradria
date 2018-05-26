using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Enviroment
{
    public enum BackgroundType : ushort {
        None = 0,
        Dirt = 1,
        Stone = 2
    }

    public struct Background
    {
        public BackgroundType BType;

        public Background(BackgroundType type)
        {
            BType = type;
        }

        public void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            if (BType == BackgroundType.None) return;
            Texture2D texture = BackgroundManager.Looks[(ushort)BType];
            spriteBatch.Draw(texture, new Vector2(x, y), new Rectangle((int)(x % texture.Width), (int)(y % texture.Height), 16, 16), Color.White);
        }

        public override string ToString()
        {
            return "Background: " + BType;
        }
    }

    public static class BackgroundManager
    {
        public static Texture2D[] Looks = new Texture2D[Enum.GetValues(typeof(BackgroundType)).Cast<ushort>().Max() + 1];

        static BackgroundManager()
        {
            AddBackground(BackgroundType.Dirt, Textures.Backgrounds.Dirt);
            AddBackground(BackgroundType.Stone, Textures.Backgrounds.Stone);
        }

        public static void AddBackground(BackgroundType btype, Texture2D texture)
        {
            Looks[(ushort)btype] = texture;
        }
    }
}
