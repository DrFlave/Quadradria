using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Utils
{
    static class Tools
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastFloor(float x)
        {
            int xi = (int)x;
            return x < xi ? xi - 1 : xi;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }
    }
}
