using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.UI
{
    class UIDropDown : UIButton
    {
        private int active = -1;
        public object Value {
            get
            {
                if (active == -1) return null;
                return ((UIDropDownItem)children[active + 1]).Value;
            }
        }

        public UIDropDown(float x, float y, float width, float height, UIContainer parent, UISizeMethod sizing = UISizeMethod.UV) : base(x, y, width, height, "---", parent, sizing)
        {

        }

        public void SetActive(int index)
        {
            active = index;

            if (active == -1)
            {
                Text = "---";
                return;
            }

            UIDropDownItem item = (UIDropDownItem)children[index + 1];
            Text = item.Text;
        }

        public void AddOption(string text, object value)
        {
            AddChild(new UIDropDownItem(this, children.Count - 1, text, value));
        }

        public override void Draw(SpriteBatch spriteBatch, int currentHover) {

            for (int i = 1; i < children.Count; i++)
            {
                UIContainer element = children[i];
                if (HasFocus())
                {
                    element.Show();
                }
                else
                {
                    element.Hide();
                }
            }

            base.Draw(spriteBatch, currentHover);
        }

        class UIDropDownItem : UIButton
        {

            private object value;
            private int index;
            public object Value { get => value; set => this.value = value; }

            public void HandleClick(object sender, EventArgs e)
            {
                ((UIDropDown)parent).SetActive(index);
            }

            public UIDropDownItem(UIDropDown parent, int index, string text, object value) : base(0, index+1, 1, 1, text, parent, UISizeMethod.UV)
            {
                this.index = index;
                this.value = value;
                Click += HandleClick;
            }

        }
    }

}
