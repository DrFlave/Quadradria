using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Quadradria
{

    public class Quadradria : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player;

        Camera camera;

        Vector2 pos = new Vector2(0, 0);
        
        public Quadradria()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        
        protected override void Initialize()
        {
            player = new Player();
            
            base.Initialize();

        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            player.Load(Content);
            camera = new Camera(GraphicsDevice.Viewport);
        }
        
        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                camera.zoom += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                camera.zoom -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                camera.rotation -= 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                camera.rotation += 0.01f;



            player.Update();
            camera.Update(player.position);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.transform);

            spriteBatch.Draw(player.texture, pos, Color.White);

            player.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
