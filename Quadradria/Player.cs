using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Quadradria
{
    class Player
    {
        public Texture2D texture;
        public Vector2 position;

        public void Load(ContentManager content)
        {
            texture = content.Load<Texture2D>("Blocks/Dirt");
        }

        public void Update(float dt)
        {
            float speed = 80f;
            if (Keyboard.GetState().IsKeyDown(Keys.RightShift))
            {
                speed *= 2;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                position.Y -= speed * dt;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                position.Y += speed * dt;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                position.X -= speed * dt;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                position.X += speed * dt;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float scale = 1.0f / texture.Width;
            spriteBatch.Draw(Textures.Blocks.Dirt, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

    }
}
