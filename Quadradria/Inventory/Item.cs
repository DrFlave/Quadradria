using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Inventory
{
    struct ItemType
    {
        public string name;
        public int maxStackSize;
        public Texture2D texture;

        public ItemType(string name, int maxStackSize, Texture2D texture)
        {
            this.name = name;
            this.maxStackSize = maxStackSize;
            this.texture = texture;
        }
    }

    class Item
    {
        private ItemType itemType;
        private int stackSize = 1;
        private SpriteFont font = Textures.Fonts.Inventory;
        public Slot slot = null;

        public Item(ItemType type)
        {
            this.itemType = type;
        }

        public int StackSize
        {
            get { return stackSize; }
            set
            {
                if (StackSize > 0 && StackSize < itemType.maxStackSize)
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

            Item item = new Item(itemType);
            item.stackSize = amount;
            stackSize -= amount;

            return item;
        }

        public void Combine(Item item)
        {
            if (item.GetType() == GetType())
            {
                int transfer = Math.Min(itemType.maxStackSize - stackSize, item.stackSize);
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
            //spriteBatch.Draw(itemType.texture, new Vector2(x, y), color: Color.White, scale: new Vector2(scale, scale));
            spriteBatch.Draw(itemType.texture, new Vector2(x, y), null, Color.White, 0f, Vector2.Zero, new Vector2(scale, scale), SpriteEffects.None, 0f);

            spriteBatch.DrawString(font, "" + stackSize, new Vector2(x + 8 * scale, y + 8 * scale), Color.White, 0, Vector2.Zero, scale / 2, SpriteEffects.None, 0);
        }
    }
}
