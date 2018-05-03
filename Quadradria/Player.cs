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
            texture = content.Load<Texture2D>("Block");
        }

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                position.Y -= 0.2f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                position.Y += 0.2f;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                position.X -= 0.2f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                position.X += 0.2f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float scale = 1.0f / texture.Width;
            spriteBatch.Draw(Textures.Blocks.Dirt, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
