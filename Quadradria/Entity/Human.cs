using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quadradria.Enviroment;
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
            //Position.X = 2;
            //Position.Y = 8;

            EntType = EntityType.Human;
            Inventory = new Storage(10);
            hitbox.Width = 1.5f;
            hitbox.Height = 2.5f;
        }

        public override void Update(GameTime time, World world)
        {
            base.Update(time, world);

            //Position.Y--;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures.Player, new Vector2(hitbox.X, hitbox.Y), color: Color.White, scale: new Vector2(1f/16), rotation: 0);
            
            base.Draw(spriteBatch);
        }

        public override BaseEntity Create()
        {
            return new Human();
        }

    }
}
