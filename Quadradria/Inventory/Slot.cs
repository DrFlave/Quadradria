using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Inventory
{
    class Slot
    {
        private Item item;

        public Item Item {
            get { return item; }
            set
            {
                if (item == null)
                {
                    item = value;
                    item.slot = this;
                }
            }
        }

        public void Clear()
        {
            if (item != null)
            {
                item = null;
                item.slot = null;
            }
        }

        public bool IsEmpty()
        {
            return (item == null);
        }
    }
}
