﻿using Microsoft.Xna.Framework;
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


        private float scale = 1;
        private SpriteFont font = Textures.Fonts.Inventory;
        private Vector2 textDimension;
        private string text;
        private UIAlignment alignment = UIAlignment.Center;

        public string Text {
            set {
                text = value;
                textDimension = font.MeasureString(value);
            }
            get{ return text; }
        }

        public UILabel(float x, float y, float width, float height, string text, UIContainer parent = null, UISizeMethod sizing = UISizeMethod.UV, UIAlignment alignment = UIAlignment.Center) : base(x, y, width, height, parent, sizing)
        {
            Text = text;
            this.alignment = alignment;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = textDimension * scale * 0.5f;
            Vector2 center = new Vector2(globalRect.Left + globalRect.Width / 2, globalRect.Top + globalRect.Height / 2);

            if (alignment.HasFlag(UIAlignment.Left)) origin.X += globalRect.Width / 2 - textDimension.X / 2;
            if (alignment.HasFlag(UIAlignment.Right)) origin.X -= globalRect.Width / 2 - textDimension.X / 2;
            if (alignment.HasFlag(UIAlignment.Top)) origin.Y += globalRect.Height / 2 - textDimension.Y / 2;
            if (alignment.HasFlag(UIAlignment.Bottom)) origin.Y -= globalRect.Height / 2 - textDimension.Y / 2;

            spriteBatch.DrawString(font, text, center, Color.White, 0, origin, new Vector2(scale, scale), SpriteEffects.None, 0);
            base.Draw(spriteBatch);
        }
    }
}
