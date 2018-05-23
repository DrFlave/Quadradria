using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Entity
{

    abstract class BaseEntity
    {
        public Vector2 Position { get; set; }
        public int ID { get; private set; }
        public EntityType EntType;

        public void Initialize(int id)
        {
            ID = id;
            Console.WriteLine("Initialized an entity at position " + Position.X + ", " + Position.Y + " with the ID: " + ID);

            /*
             *  Entities Chunk 50, 12 -> read file: ents.1.0.ents -> gehe alle ents durch und lese wenn sie in 50, 12 sind.
             * 
             * */

        }

        public virtual void Update() { }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        public virtual byte[] Export() { return new byte[0];  }

        public virtual void Import(byte[] data) { }

        public abstract BaseEntity Create();
    }
}
