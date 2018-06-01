using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quadradria.Enviroment;
using Quadradria.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Entity
{

    abstract class BaseEntity
    {
        public Vector2 Position;
        public uint ID { get; private set; }
        public EntityType EntType;
        public RectF hitbox;

        public virtual void Initialize(uint id)
        {
            ID = id;
            Console.WriteLine("Initialized an entity at position " + Position.X + ", " + Position.Y + " with the ID: " + ID);

            /*
             *  Entities Chunk 50, 12 -> read file: ents.1.0.ents -> gehe alle ents durch und lese wenn sie in 50, 12 sind.
             * 
             * */

        }

        public BaseEntity()
        {
            hitbox = new RectF(0);
        }

        public virtual void Update(GameTime gameTime, World world) {
            hitbox.X = Position.X - hitbox.Width / 2;
            hitbox.Y = Position.Y - hitbox.Height / 2;
        }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        public virtual byte[] Export() { return new byte[0];  }

        public virtual void Import(byte[] data) { }

        public abstract BaseEntity Create();

        public bool PlaceMeeting()
        {
            return false;
        }
    }
}
