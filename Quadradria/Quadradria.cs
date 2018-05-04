using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Quadradria.Entity;
using Quadradria.Inventory;
using Quadradria.World;

namespace Quadradria
{

    public class Quadradria : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Item testItem;

        Player player;

        Camera camera;

        Vector2 pos = new Vector2(0, 0);

        Chunk chunk;
        Chunk chunk2;

        public Quadradria()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        
        protected override void Initialize()
        {
            player = new Player();
            Textures.Load(Content);

            chunk = new Chunk(0, 0, GraphicsDevice);
            chunk2 = new Chunk(1, 1, GraphicsDevice);
            testItem = new Item();

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
                camera.rotation -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                camera.rotation += 0.02f;

            player.Update();
            camera.Update(player.position);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            chunk.Render(spriteBatch);
            chunk2.Render(spriteBatch);

            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.transform);
            
            chunk.Draw(spriteBatch);
            chunk2.Draw(spriteBatch);

            
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);

            testItem.Draw(spriteBatch, 32, 32, 4);
       
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
