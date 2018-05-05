using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.UI
{
    class UIButton : UIInteractable
    {
        UILabel label;
        string Text {
            set
            {
                label.Text = value;
            }
            get
            {
                return label.Text;
            }
        }

        public UIButton(float x, float y, float width, float height, string text, UIContainer parent, UISizeMethod sizing = UISizeMethod.UV) : base(x, y, width, height, parent, sizing)
        {
            label = new UILabel(0, 0, 1, 1, text, this, UISizeMethod.UV, UIAlignment.Center)
            {
                color = Color.Black
            };
        }

        public override void Draw(SpriteBatch spriteBatch, int currentHover = 0)
        {

            spriteBatch.Draw(Textures.Solid, globalRect, hover || pressed ? Color.White : Color.LightGray);

            base.Draw(spriteBatch, currentHover);
        }
    }
}
