using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Quadradria.Entity;
using Quadradria.Inventory;
using Quadradria.UI;
using Quadradria.Enviroment;
using System;
using Quadradria.Utils;
using System.Diagnostics;

namespace Quadradria
{

    public class Quadradria : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player;

        Camera camera;
        
        World world;

        UIContainer UIMaster;
        UILabel frameCounter;
        UILabel debugInformation;

        bool showDebugInformation = true;

        public Quadradria()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;

        }

        protected override void Initialize()
        {
            player = new Player();
            Textures.Load(Content, GraphicsDevice);

            BlockTypeDefault.InitBlockTypes();
            world = new World(@"E:\", GraphicsDevice);


            world.AddEntity(new Human() { Position = new Vector2(4, 4) });
            world.AddEntity(new Human() { Position = new Vector2(2, 2) });

            UIMaster = new UIContainer(0, 0, 300, 300);
            new UIInteractable(0, 0, 1, 1, UIMaster); //makes things defocusable
            frameCounter = new UILabel(10, 10, 200, 50, "FPS: -", UIMaster, UISizeMethod.Pixel, UIAlignment.Top | UIAlignment.Left);
            debugInformation = new UILabel(10, 30, 200, 20, "Loaded Chunks: -", UIMaster, UISizeMethod.Pixel, UIAlignment.Top | UIAlignment.Left);

            /*UIDropDown dd = new UIDropDown(200, 20, 300, 35, UIMaster, UISizeMethod.Pixel);
            dd.AddOption("Option A", 1);
            dd.AddOption("Option B", 2);
            dd.AddOption("Option C", 3);*/

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.SynchronizeWithVerticalRetrace = true; //V-Sync
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
            debugInformation.Visible = showDebugInformation;

            Vector2 mpos = camera.GetMousePositionInWorld();

            debugInformation.Show();
            debugInformation.Text
            = "Loaded chunks: " + world.LoadedChunks.GetLoadedChunkNumber()
            + "\nVisible chunks: " + world.LoadedChunks.GetVisibleChunkNumber()
            + "\nMemory usage (Process): " + Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024 + "MB"
            + "\nMouse position (World space): "+ mpos.X + ", " + mpos.Y
            + "\nMouse position (Block): " + Math.Floor(mpos.X) + ", " + Math.Floor(mpos.Y)
            + "\nCamera zoom: " + camera.zoom
            + "\nCamera position (Center): " + camera.center;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.S)) camera.zoom -= 0.02f;
            
            player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            camera.Update(player.position);

            RectF rect = camera.GetRect();

            world.Update(rect.X, rect.Y, rect.Width, rect.Height);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            if (gameTime.ElapsedGameTime.Milliseconds != 0)
                frameCounter.Text = "FPS: " + Math.Round(1000 / gameTime.ElapsedGameTime.TotalMilliseconds);

            world.Render(spriteBatch);

            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.transform);

            world.Draw(spriteBatch);
            spriteBatch.Draw(Textures.Error, camera.GetMousePositionInWorld(), null, Color.White, 0, Vector2.Zero, 1/16, SpriteEffects.None, 0);

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
