using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Entity
{
    abstract class Physical : BaseEntity
    {
        Vector2 momentum;

        public Physical(Vector2 position) : base(position)
        {
            momentum = new Vector2(0, 0);
        }
    }
}
