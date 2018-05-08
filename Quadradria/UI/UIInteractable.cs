using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.UI
{
    class UIInteractable : UIContainer
    {
        protected bool hover;
        protected bool pressed;

        //For Gamepad Controls
        protected UIInteractable elementDown;
        protected UIInteractable elementUp;
        protected UIInteractable elementRight;
        protected UIInteractable elementLeft;

        protected bool hasFocus;

        private bool lastLeftDown = false;

        public delegate void ClickEventHandler(object source, EventArgs args);
        public event ClickEventHandler Click;

        public UIInteractable(float x, float y, float width, float height, UIContainer parent = null, UISizeMethod sizing = UISizeMethod.UV) : base(x, y, width, height, parent, sizing)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, int currentHover)
        {
            var mouseState = Mouse.GetState();

            if (currentHover == GetHashCode())
            {
                hover = true;
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (!lastLeftDown)
                        pressed = true;
                } else {
                    if (pressed)
                    {
                        OnClick();
                        pressed = false;
                    }
                }
            } else {
                hover = false;
                if (mouseState.LeftButton != ButtonState.Pressed)
                {
                    pressed = false;
                }
            }

            lastLeftDown = mouseState.LeftButton == ButtonState.Pressed;

            base.Draw(spriteBatch);
        }

        public override void CheckHover(int x, int y, ref int currentTop)
        {
            if (globalRect.Contains(x, y))
            {
                currentTop = GetHashCode();
            }

            base.CheckHover(x, y, ref currentTop);
        }

        protected virtual void OnClick()
        {
            if (Click != null)
                Click(this, EventArgs.Empty);
        }
    }

}
