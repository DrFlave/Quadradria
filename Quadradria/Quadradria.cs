using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Quadradria.Entity;
using Quadradria.Inventory;
using Quadradria.UI;
using Quadradria.Enviroment;
using System;

namespace Quadradria
{

    public class Quadradria : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player;

        Camera camera;

        //Chunk chunk;
        //Chunk chunk2;
        World world;

        UIContainer UIMaster;
        UILabel frameCounter;

        public Quadradria()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            player = new Player();
            Textures.Load(Content, GraphicsDevice);

            //chunk = new Chunk(0, 0, GraphicsDevice);
            //chunk2 = new Chunk(1, 1, GraphicsDevice);
            world = new World(GraphicsDevice);

            UIMaster = new UIContainer(0, 0, 300, 300);
            frameCounter = new UILabel(0, 0, 200, 50, "FPS: -", UIMaster, UISizeMethod.Pixel, UIAlignment.Top | UIAlignment.Left);

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.ApplyChanges();

            camera = new Camera(GraphicsDevice.Viewport);

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;


            base.Initialize();

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player.Load(Content);

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {



            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            
            player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            camera.Update(player.position);

            Rectangle rect = camera.GetRect();

            world.Update(rect.X, rect.Y, rect.Width, rect.Height);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            if (gameTime.ElapsedGameTime.Milliseconds != 0)
                frameCounter.Text = "FPS: " + Math.Round(1000 / gameTime.ElapsedGameTime.TotalMilliseconds);

            //chunk.Render(spriteBatch);
            //chunk2.Render(spriteBatch);

            world.Render(spriteBatch);

            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.transform);

            //chunk.Draw(spriteBatch);
            //chunk2.Draw(spriteBatch);

            world.Draw(spriteBatch);


            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            UIMaster.Draw(spriteBatch);

            spriteBatch.End();


            base.Draw(gameTime);
        }

        public void OnResize(Object sender, EventArgs e)
        {
            camera.Resize(GraphicsDevice.Viewport);
            UIMaster.Resize(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        }
    }
}
