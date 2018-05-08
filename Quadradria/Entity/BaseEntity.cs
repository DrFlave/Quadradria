using Microsoft.Xna.Framework;
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

        public BaseEntity(Vector2 position)
        {
            Position = position;
        }

        public void Initialize(int id)
        {
            ID = id;
            Console.WriteLine("Initialized an entity at position " + Position.X + ", " + Position.Y + " with the ID: " + ID);
        }

        public void Update()
        {

        }

        public void Tick()
        {

        }

        public void Draw()
        {

        }
    }
}
