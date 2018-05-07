using Microsoft.Xna.Framework;
using Quadradria.Enviroment;
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
        public int ID { get; set; }

        public BaseEntity(World world, Vector2 position)
        {
            Position = position;

            ID = world.RequestEntityId();
            Console.WriteLine("Created Entity with id " + ID);
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
