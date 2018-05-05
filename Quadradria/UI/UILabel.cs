using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.UI
{
    [Flags] public enum UIAlignment { Center = 0, Left = 1, Right = 2, Top = 4, Bottom = 8 }

    class UILabel : UIContainer
    {


        private Vector2 scale = new Vector2(1, 1);
        private SpriteFont font = Textures.Fonts.Inventory;
        private Vector2 textDimension;
        private string text;
        private UIAlignment alignment = UIAlignment.Center;

        public Color color = Color.White;

        public string Text {
            set {
                text = value;
                textDimension = font.MeasureString(value) * scale;
            }
            get{ return text; }
        }

        public float Scale
        {
            set
            {
                scale.X = value;
                scale.Y = value;
            }
            get { return scale.X; }
        }

        public UILabel(float x, float y, float width, float height, string text, UIContainer parent = null, UISizeMethod sizing = UISizeMethod.UV, UIAlignment alignment = UIAlignment.Center) : base(x, y, width, height, parent, sizing)
        {
            Text = text;
            this.alignment = alignment;
        }

        public override void Draw(SpriteBatch spriteBatch, int currentTop)
        {
            Vector2 origin = textDimension * scale * 0.5f;
            Vector2 center = globalRect.Center.ToVector2();

            if (alignment.HasFlag(UIAlignment.Left)) origin.X += globalRect.Width / 2 - textDimension.X / 2;
            if (alignment.HasFlag(UIAlignment.Right)) origin.X -= globalRect.Width / 2 - textDimension.X / 2;
            if (alignment.HasFlag(UIAlignment.Top)) origin.Y += globalRect.Height / 2 - textDimension.Y / 2;
            if (alignment.HasFlag(UIAlignment.Bottom)) origin.Y -= globalRect.Height / 2 - textDimension.Y / 2;

            spriteBatch.DrawString(font, text, center, color, 0, origin, scale, SpriteEffects.None, 0);
            base.Draw(spriteBatch, currentTop);
        }
    }
}
