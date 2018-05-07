using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Quadradria.Entity;
using Quadradria.Enviroment;

namespace Quadradria.Entity
{
    abstract class Mob : Physical
    {
        private float health;
        private float maxHealth;
        private float walkSpeed;

        public Mob(World world, Vector2 position, float maxHealth, float walkSpeed) : base(world, position) {
            this.maxHealth = maxHealth;
            this.health = maxHealth;
            this.walkSpeed = walkSpeed;
        }

        public void Walk(Vector2 direction)
        {

        }

        public void Damage(float amount)
        {
            health -= amount;
        }

        public void Heal(float amount)
        {
            health = Math.Min( health + amount, maxHealth);
        }
    }
}
