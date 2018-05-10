using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Utils
{
    class List2D<T>
    {
        private List<Coloumn> Coloumns = new List<Coloumn>();
        private int length = 0;
        public int Length { get { return length; } }

        private Coloumn GetColoumn(int x)
        {
            for (int i = 0; i < Coloumns.Count; i++)
            {
                if (Coloumns[i].x == x)
                {
                    return Coloumns[i];
                }
            }
            return null;
        }
        
        public T Get(int x, int y)
        {
            Coloumn cc = GetColoumn(x);
            if (cc == null) return default(T);

            return cc.Get(y);
        }

        public bool Includes(int x, int y)
        {
            Coloumn cc = GetColoumn(x);
            if (cc == null) return false;

            return cc.Includes(y);
        }

        public bool Add(int x, int y, T item)
        {
            if (Includes(x, y))  return false;

            Coloumn cc = GetColoumn(x);
            if (cc == null)
            {
                cc = new Coloumn(x);
                Coloumns.Add(cc);
            }

            if(cc.Add(y, item))
            {
                length++;
                return true;
            }

            return false;
        }

        public bool Remove(int x, int y)
        {
            for (int i = 0; i < Coloumns.Count; i++)
            {
                if (Coloumns[i].x == x)
                {
                    if (Coloumns[i].Remove(y))
                    {
                        if (Coloumns[i].IsEmpty())
                        {
                            Coloumns.RemoveAt(i);
                        }
                        length--;
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        public void ForEach(Action<T> func)
        {
            for (int i = 0; i < Coloumns.Count; i++)
            {
                Coloumns[i].ForEach(func);
            }

        }

        public void ForEachWrapper(Action<Wrapper> func)
        {
            for (int i = 0; i < Coloumns.Count; i++)
            {
                Coloumns[i].ForEachWrapper(func);
            }
        }


        public class Coloumn
        {
            public int x;

            private List<Wrapper> Items = new List<Wrapper>();

            public Coloumn(int x)
            {
                this.x = x;
            }

            public T Get(int y)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].y == y)
                    {
                        return Items[i].item;
                    }
                }
                return default(T);
            }

            public bool Includes(int y)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].y == y)
                    {
                        return true;
                    }
                }
                return false;
            }

            public bool Add(int y, T item)
            {
                if (Includes(y)) return false;

                Items.Add(new Wrapper(x, y, item));
                return true;
            }

            public bool Remove(int y)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].y == y)
                    {
                        Items.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }

            public bool IsEmpty()
            {
                return !Items.Any();
            }

            public void ForEach(Action<T> func)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    func(Items[i].item);
                }
            }
            
            public void ForEachWrapper(Action<Wrapper> func)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    func(Items[i]);
                }
            }

        }

        public struct Wrapper
        {
            public int x;
            public int y;
            public T item;

            public Wrapper(int x, int y, T item)
            {
                this.x = x;
                this.y = y;
                this.item = item;
            }
        }

    }
}
