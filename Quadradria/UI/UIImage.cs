using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.UI
{
    class UIImage : UIContainer
    {
        private Texture2D texture;

        public UIImage(float x, float y, float width, float height, Texture2D texture, UIContainer parent = null, UISizeMethod sizing = UISizeMethod.UV) : base(x, y, width, height, parent, sizing)
        {
            this.texture = texture;
        }

        public override void Draw(SpriteBatch spriteBatch, int currentTop)
        {
            if (!visible) return;

            float wscale = (float)globalRect.Width / (float)texture.Width;
            float hscale = (float)globalRect.Height / (float)texture.Height;
            float scale = Math.Min(wscale, hscale);

            spriteBatch.Draw(texture, globalRect.Center.ToVector2(), null, Color.White, 0, new Vector2(globalRect.Width / 2, globalRect.Height / 2), new Vector2(scale, scale) , SpriteEffects.None, 0);

            base.Draw(spriteBatch, currentTop);
        }
    }
}
