using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Utils
{
    public class RectF
    {
        public float X, Y, Width, Height;

        public RectF(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public bool Contains(float x, float y)
        {
            return (x >= X && y >= Y && x < X + Width && y < Y + Height);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rect)) return false;
            Rect b = (Rect)obj;

            return X == b.X && Y == b.Y && Width == b.Width && Height == b.Height;
        }

        public override string ToString()
        {
            return ("(X: " + X + ", Y: " + Y + ", W: " + Width + ", H: " + Height + ")");
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
