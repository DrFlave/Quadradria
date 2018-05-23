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
        protected float gravityMultiplier = 1;

        public Physical() : base()
        {
            momentum = new Vector2(0, 0);
        }

        public override void Update()
        {

        }
    }
}
