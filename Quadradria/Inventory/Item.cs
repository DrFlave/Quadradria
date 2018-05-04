using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Inventory
{
    class Item
    {
        private int maxStackSize = 64;
        private int stackSize = 1;

        private Texture2D texture = Textures.Items.Sword;
        private SpriteFont font = Textures.Fonts.Inventory;

        public Slot slot = null;
        public string Name = "Item.Generic";

        public int StackSize
        {
            get { return stackSize; }
            set
            {
                if (StackSize > 0 && StackSize < maxStackSize)
                {
                    stackSize = value;
                }
                if (StackSize == 0) {
                    stackSize = 0;
                    ClearSlot();
                }
            }
        }

        public Item Split(int amount)
        {

            amount = Math.Min(stackSize, amount);

            Item item = new Item();
            item.stackSize = amount;
            stackSize -= amount;

            return item;
        }

        public void Combine(Item item)
        {
            if (item.GetType() == GetType())
            {
                int transfer = Math.Min(maxStackSize - stackSize, item.stackSize);
                stackSize += transfer;
                item.stackSize -= transfer;
            }
        }

        public void ClearSlot()
        {
            if (slot != null)
            {
                slot.Clear();
            }
        }

        public void Draw(SpriteBatch spriteBatch, int x, int y, float scale = 1)
        {
            spriteBatch.Draw(texture, new Vector2(x, y), color: Color.White, scale: new Vector2(scale, scale));
            spriteBatch.DrawString(font, "" + stackSize, new Vector2(x + 8 * scale, y + 8 * scale), Color.White, 0, Vector2.Zero, scale / 2, SpriteEffects.None, 0);
        }
    }
}
