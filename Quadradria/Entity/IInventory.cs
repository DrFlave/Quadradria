using Quadradria.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Entity
{
    interface IInventory
    {
        Storage Inventory { get; }
    }
}
