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
            texture = content.Load<Texture2D>("terrain_atlas");
        }

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                position.Y -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                position.Y += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                position.X -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                position.X += 1;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
