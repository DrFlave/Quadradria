using Microsoft.Xna.Framework;
using Quadradria.Enviroment;
using Quadradria.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Entity
{
    abstract class Physical : BaseEntity
    {
        private Vector2 momentum = new Vector2(0);
        protected float gravityMultiplier = 1;
        
        public Physical() : base()
        {
            momentum = new Vector2(0, 0);
        }

        public void Impulse(float x, float y)
        {
            momentum.X += x;
            momentum.Y += y;
        }

        public override void Update(GameTime delta, World world)
        {
            base.Update(delta, world);

            momentum.Y += world.Gravity * gravityMultiplier;

            RectF r = new RectF(hitbox.X, hitbox.Y, hitbox.Width, hitbox.Height);
            Vector2 dir = momentum;
            float mLength = momentum.Length();
            dir.Normalize();

            bool moveX = true;
            bool moveY = true;
            float newX = hitbox.X;
            float newY = hitbox.Y;

            Func<float, int> FloorEx = (float i) =>
            {
                int n = (int)Math.Floor(i);
                if (n == i) return n - 1;
                return n;
            };

            Action<float> a = (float i) => {
                if (moveX) r.X = newX + dir.X * i;
                
                for (int y = (int)Math.Floor(r.Y); y <= FloorEx(r.Bottom) && moveX; y++)
                {
                    if (dir.X >= 0)
                    {
                        Block? b = world.GetBlockAtPosition((int)Math.Floor(r.Right), y);
                        if (b?.IsSolid() == true)
                        {
                            moveX = false;
                            newX = (float)Math.Floor(r.Right) - r.Width;
                            r.X = newX;
                            momentum.X = 0;
                            break;
                        }
                        newX = r.X;
                    } else
                    {
                        Block? b = world.GetBlockAtPosition((int)Math.Floor(r.X), y);
                        if (b?.IsSolid() == true)
                        {
                            moveX = false;
                            newX = (float)Math.Floor(r.X) + 1;
                            r.X = newX;
                            momentum.X = 0;
                            break;
                        }
                        newX = r.X;
                    }
                }

                if(moveY) r.Y = newY + dir.Y * i;

                for (int x = (int)Math.Floor(r.X); x <= FloorEx(r.Right) && moveY; x++)
                {
                    if (dir.Y >= 0)
                    {
                        Block? b = world.GetBlockAtPosition(x, (int)Math.Floor(r.Bottom));
                        if (b?.IsSolid() == true)
                        {
                            moveY = false;
                            newY = (float)Math.Floor(r.Bottom) - r.Height;
                            r.Y = newY;
                            momentum.Y = 0;
                            break;
                        }
                        newY = r.Y;
                    } else
                    {
                        Block? b = world.GetBlockAtPosition(x, (int)Math.Floor(r.Y));
                        if (b?.IsSolid() == true)
                        {
                            moveY = false;
                            newY = (float)Math.Floor(r.Y)+1;
                            r.Y = newY;
                            momentum.Y = 0;
                            break;
                        }
                        newY = r.Y;
                    }
                }
            };
            
            for (float i = 0; i <= mLength-1; i++)
            {
                a(1);
            }

            a(mLength - (int)Math.Floor(mLength));

            hitbox.X = newX;
            hitbox.Y = newY;

            Position.X = hitbox.X + hitbox.Width / 2;
            Position.Y = hitbox.Y + hitbox.Height / 2;
        }

    }
}
