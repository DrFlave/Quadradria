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
        public Vector2 position;
        public int id;

        public BaseEntity(Vector2 position)
        {
            this.position = position;
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
