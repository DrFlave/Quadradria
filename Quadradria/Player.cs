using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Quadradria.Enviroment;

namespace Quadradria
{
    class Player
    {
        public Texture2D texture;
        public Vector2 Position { get => pawn.Position; }
        private World world;

        private Keys[] keys = Keyboard.GetState().GetPressedKeys();

        private Entity.Mob pawn;

        public Player(Entity.Mob pawn)
        {
            this.pawn = pawn;
        }

        public void Load(ContentManager content)
        {
            texture = content.Load<Texture2D>("Blocks/Dirt");
        }

        public void Update(float dt)
        {
            Keys[] newKeys = Keyboard.GetState().GetPressedKeys();

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                pawn.Impulse(-0.02f, 0f);
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                pawn.Impulse(0.02f, 0f);

            if (Array.IndexOf(newKeys, Keys.Space) > -1 && Array.IndexOf(keys, Keys.Space) == -1)
                pawn.Impulse(0, -0.8f);

            keys = newKeys;


        }
    }
}
