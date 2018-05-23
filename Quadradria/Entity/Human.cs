using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quadradria.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Entity
{
    class Human : Mob, IInventory
    {
        public Storage Inventory { get; }

        public Human() : base(100, 3)
        {
            EntType = EntityType.Human;
            Inventory = new Storage(10);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures.Error, Position, null, Color.White, 0, Vector2.Zero, 1/16f, SpriteEffects.None, 0);
            base.Draw(spriteBatch);
        }

        public override BaseEntity Create()
        {
            return new Human();
        }

    }
}
