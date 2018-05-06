using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria
{
    static class Utils
    {
        public struct Rect
        {
            public int X, Y, Width, Height;

            public Rect(int x, int y, int width, int height)
            {
                this.X = x;
                this.Y = y;
                this.Width = width;
                this.Height = height;
            }

            public static bool operator ==(Rect a, Rect b)
            {
                return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) return false;

                return this == (Rect)obj;
            }

            public static bool operator !=(Rect a, Rect b)
            {
                return !(a == b);
            }
        }

        public struct RectF
        {
            public float X, Y, Width, Height;

            public RectF(float x, float y, float width, float height)
            {
                this.X = x;
                this.Y = y;
                this.Width = width;
                this.Height = height;
            }

            public static bool operator ==(RectF a, RectF b)
            {
                return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (!(obj is RectF)) return false;

                return this == (RectF)obj;
            }

            public static bool operator !=(RectF a, RectF b)
            {
                return !(a == b);
            }
        }

    }
}
