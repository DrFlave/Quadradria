using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Inventory
{
    class Storage
    {
        Slot[] slots;

        public Storage(int size) {
            if (size < 0) size = 1;
            slots = new Slot[size];

            Console.WriteLine("Created Storage with {0} slots!", size);
        }

        public void Give(Item item)
        {
            foreach(Slot slot in slots)
            {
                if (slot.IsEmpty())
                {
                    slot.Item = item;
                    break;
                }
            }
        }

        public void Clear()
        {
            foreach(Slot slot in slots)
            {
                slot.Clear();
            }
        }
    }
}
