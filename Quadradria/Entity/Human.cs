using Microsoft.Xna.Framework;
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

        public Human(World world, Vector2 position) : base(world, position, 100, 3)
        {
            Inventory = new Storage(10);
        }
    }
}
