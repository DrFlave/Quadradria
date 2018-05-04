using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.UI
{
    class UIContainer
    {
        public enum SizeMethod { UV, Pixel }

        private SizeMethod sizing;
        private float x;
        private float y;
        private float width;
        private float height;
        private UIContainer parent;
        private List<UIContainer> children;

        public Color color = Color.White;

        private Rectangle globalRect;

        public UIContainer(float x, float y, float width, float height, UIContainer parent = null, SizeMethod sizing = SizeMethod.UV)
        {
            children = new List<UIContainer>();

            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.sizing = sizing;
            if (parent == null)
            {
                RecalcGlobals();
            } else
            {
                parent.AddChild(this);
            }
        }

        protected void RecalcGlobals()
        {
            if (parent == null)
            {

                globalRect.X = (int)x;
                globalRect.Y = (int)y;
                globalRect.Width = (int)width;
                globalRect.Height = (int)height;

                /*
                if (sizing == SizeMethod.UV)
                {
                    globalX = x;
                    globalY = y;
                    globalWidth = width;
                    globalHeight = height;
                }*/

                return;
            }

            switch (sizing)
            {
                case SizeMethod.Pixel:
                    globalRect.X = (int)(parent.globalRect.X + x);
                    globalRect.Y = (int)(parent.globalRect.Y + y);
                    globalRect.Width = (int)width;
                    globalRect.Height = (int)height;
                    break;

                case SizeMethod.UV:
                    globalRect.X = (int)(parent.globalRect.X + parent.globalRect.Width * x);
                    globalRect.Y = (int)(parent.globalRect.Y + parent.globalRect.Height * y);
                    globalRect.Width = (int)(parent.globalRect.Width * width);
                    globalRect.Height = (int)(parent.globalRect.Height * height);
                    break;
            }
        }

        protected void SetParent(UIContainer parent)
        {
            if (this.parent != null)
            {
                this.parent.RemoveChild(this);
            }
            this.parent = parent;
            RecalcGlobals();
        }

        public void AddChild(UIContainer child)
        {
            children.Add(child);
            child.SetParent(this);
        }

        public void RemoveChild(UIContainer child)
        {
            children.Remove(child);
            child.parent = null;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(Textures.Solid, globalRect, color);

            foreach (UIContainer element in children)
            {
                element.Draw(spriteBatch);
            }
        }
    }
}
