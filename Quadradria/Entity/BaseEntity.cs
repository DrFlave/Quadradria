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
        private Chunk chunk;

        public virtual void Initialize(uint id, Chunk chunk)
        {
            ID = id;
            Console.WriteLine("Initialized an entity at position " + Position.X + ", " + Position.Y + " with the ID: " + ID);

            this.chunk = chunk;
            chunk.AddEntity(this);
        }

        public BaseEntity()
        {
            hitbox = new RectF(0);
        }

        public virtual void Update(GameTime gameTime, World world) {
            hitbox.X = Position.X - hitbox.Width / 2;
            hitbox.Y = Position.Y - hitbox.Height / 2;

            Chunk c = world.GetChunkAtTilePosition((int)Math.Floor(Position.X), (int)Math.Floor(Position.Y));
            if (c != chunk && c != null)
            {
                UpdateChunk(c);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        public virtual byte[] Export() { return new byte[0];  }

        public virtual void Import(byte[] data) { }

        public abstract BaseEntity Create();

        public bool PlaceMeeting()
        {
            return false;
        }

        protected void UpdateChunk(Chunk chunk)
        {
            this.chunk.RemoveEntity(this);
            chunk.AddEntity(this);
            this.chunk = chunk;
        }
    }
}
