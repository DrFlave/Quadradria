using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.UI
{
    public enum UISizeMethod { UV, Pixel }

    class UIContainer
    {

        private UISizeMethod sizing;
        private float x;
        private float y;
        private float width;
        private float height;
        private UIContainer focused;

        public bool Visible
        {
            get => visible;
            set => visible = value;
        }

        protected UIContainer parent;
        protected bool visible = true;
        protected List<UIContainer> children;
        protected Rectangle globalRect;

        public bool HasFocus()
        {
            return (GetFocusedElement() == this);
        }

        public UIContainer GetFocusedElement()
        {
            if (parent == null) return focused;
            return parent.GetFocusedElement();
        }

        public void Focus(UIContainer element)
        {
            if (element == null) element = this;

            if (parent == null) {
                focused = element;
                return;
            }
            parent.Focus(element);
        }

        public UIContainer(float x, float y, float width, float height, UIContainer parent = null, UISizeMethod sizing = UISizeMethod.UV)
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

        public void Resize(float width, float height)
        {
            if (width < 0 || height < 0) return;
            this.width = width;
            this.height = height;

            RecalcGlobals();
        }

        public void SetPosition(float x, float y)
        {
            this.x = x;
            this.y = y;
            RecalcGlobals();
        }

        protected void RecalcGlobals()
        {

            if (parent == null)
            {
                globalRect.X = (int)x;
                globalRect.Y = (int)y;
                globalRect.Width = (int)width;
                globalRect.Height = (int)height;
            }
            else
            {
                switch (sizing)
                {
                    case UISizeMethod.Pixel:
                        globalRect.X = (int)(parent.globalRect.X + x);
                        globalRect.Y = (int)(parent.globalRect.Y + y);
                        globalRect.Width = (int)width;
                        globalRect.Height = (int)height;
                        break;

                    case UISizeMethod.UV:
                        globalRect.X = (int)(parent.globalRect.X + parent.globalRect.Width * x);
                        globalRect.Y = (int)(parent.globalRect.Y + parent.globalRect.Height * y);
                        globalRect.Width = (int)(parent.globalRect.Width * width);
                        globalRect.Height = (int)(parent.globalRect.Height * height);
                        break;
                }
            }

            foreach (UIContainer element in children)
            {
                element.RecalcGlobals();
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

        public virtual void CheckHover(int x, int y, ref int currentTop)
        {
            if (!visible) return;

            foreach (UIContainer element in children)
            {
                element.CheckHover(x, y, ref currentTop);
            }
        }

        public void Show()
        {
            visible = true;
        }

        public void Hide()
        {
            visible = false;
        }

        public virtual void Draw(SpriteBatch spriteBatch, int currentTop = 0)
        {
            if (!visible) return;

            if (parent == null)
            {
                MouseState mouseState = Mouse.GetState();
                CheckHover(mouseState.X, mouseState.Y, ref currentTop);
            }

            foreach (UIContainer element in children)
            {
                element.Draw(spriteBatch, currentTop);
            }
        }
    }
}
