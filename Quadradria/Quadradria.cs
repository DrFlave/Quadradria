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
using Quadradria.Enviroment.Generators;

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

        BlendState blendStateMultiply;
        BlendState blendStateLighten;

        RenderTarget2D rtLight;
        RenderTarget2D rtLightBlur;
        RenderTarget2D rtLightSource;
        Color[] lightSourceColor;

        Effect efGauss;

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

            BlockManager.Init();
            EntityManager.Init();

            world = new World(@"E:\", GraphicsDevice);

            camera = new Camera(GraphicsDevice.Viewport);

            RectF rect = camera.GetRect();
            world.Update(rect.X, rect.Y, rect.Width, rect.Height);


            BaseEntity human = EntityManager.Spawn(EntityType.Human);
            world.AddEntity(human);

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


            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;

            blendStateMultiply = new BlendState();
            blendStateMultiply.ColorBlendFunction = BlendFunction.Add;
            blendStateMultiply.ColorSourceBlend = Blend.DestinationColor;
            blendStateMultiply.ColorDestinationBlend = Blend.Zero;

            blendStateLighten = new BlendState();
            blendStateLighten.ColorBlendFunction = BlendFunction.Max;
            blendStateLighten.ColorSourceBlend = Blend.SourceColor;
            blendStateLighten.ColorDestinationBlend = Blend.DestinationColor;
            
            RecalcLightRT();

            base.Initialize();

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player.Load(Content);
            efGauss = Content.Load<Effect>("Shader/Blur");
        }

        protected override void UnloadContent()
        {
            Console.WriteLine("Unload Content");

            world.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            debugInformation.Visible = showDebugInformation;

            Vector2 mpos = camera.GetMousePositionInWorld();

            Block? underMouse = world.GetBlockAtPosition((int)Math.Floor(mpos.X), (int)Math.Floor(mpos.Y));

            debugInformation.Show();
            debugInformation.Text
            = "Loaded chunks: " + world.LoadedChunks.GetLoadedChunkNumber()
            + "\nVisible chunks: " + world.LoadedChunks.GetVisibleChunkNumber()
            + "\nLoaded Megachunks: " + world.GetNumberOfLoadedMegachunks()
            + "\nMemory usage: " + Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024 + "MB"
            + "\nMouse position (World space): " + mpos.X + ", " + mpos.Y
            + "\nMouse position (Block): " + Math.Floor(mpos.X) + ", " + Math.Floor(mpos.Y)
            + "\nMouse position (Chunk): " + Math.Floor(mpos.X / Chunk.SIZE) + ", " + Math.Floor(mpos.Y / Chunk.SIZE)
            + "\nMouse position (Megachunk): " + Math.Floor(mpos.X / Chunk.SIZE / Megachunk.SIZE) + ", " + Math.Floor(mpos.Y / Chunk.SIZE / Megachunk.SIZE)
            + "\nCamera zoom: " + camera.zoom
            + "\nCamera position (Center): " + camera.center.X + ", " + camera.center.Y
            + "\nBlock under mouse: " + underMouse;

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                world.SetBlockAtPosition((int)Math.Floor(mpos.X), (int)Math.Floor(mpos.Y), BlockType.Stone, 0);
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.S)) camera.zoom -= 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.W)) camera.zoom += 0.01f;

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

            //Lighting
            GraphicsDevice.SetRenderTarget(rtLight);
            RectF rect = camera.GetRect();
            int chunksX = (int)Math.Floor(rect.Width / Chunk.SIZE) + 2;
            int chunksY = (int)Math.Floor(rect.Height / Chunk.SIZE) + 2;
            int chunkX = (int)Math.Floor(rect.X / Chunk.SIZE);
            int chunkY = (int)Math.Floor(rect.Y / Chunk.SIZE);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp);
            for (int i = 0; i < chunksX; i++)
            {
                for (int j = 0; j < chunksY; j++)
                {
                    Texture2D tex = world.GetLightTexture(chunkX + i, chunkY + j);
                    if (tex != null)
                        spriteBatch.Draw(tex, new Vector2(i * Chunk.SIZE, j * Chunk.SIZE), Color.White);
                    else
                        spriteBatch.Draw(Textures.Error, new Vector2(i * Chunk.SIZE, j * Chunk.SIZE), Color.White);
                }
            }
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            rtLight.GetData<Color>(lightSourceColor);
            rtLightSource.SetData<Color>(lightSourceColor);

            GraphicsDevice.SetRenderTarget(rtLightBlur);
            spriteBatch.Begin(SpriteSortMode.BackToFront, blendStateLighten, SamplerState.PointClamp);
            spriteBatch.Draw(rtLight, Vector2.Zero, Color.White);
            spriteBatch.End();

            efGauss.Parameters["Size"]?.SetValue(chunksX * Chunk.SIZE);
            efGauss.Parameters["Direction"]?.SetValue(new Vector2(1, 0));
            spriteBatch.Begin(SpriteSortMode.BackToFront, blendStateLighten, SamplerState.PointClamp, null, null, efGauss, null);
            spriteBatch.Draw(rtLight, Vector2.Zero, Color.White);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.SetRenderTarget(rtLight);
            spriteBatch.Begin(SpriteSortMode.BackToFront, blendStateLighten, SamplerState.PointClamp);
            spriteBatch.Draw(rtLightBlur, Vector2.Zero, Color.White);
            spriteBatch.End();
            efGauss.Parameters["Size"]?.SetValue(chunksY * Chunk.SIZE);
            efGauss.Parameters["Direction"]?.SetValue(new Vector2(0, 1));
            spriteBatch.Begin(SpriteSortMode.BackToFront, blendStateLighten, SamplerState.PointClamp, null, null, efGauss, null);
            spriteBatch.Draw(rtLightBlur, Vector2.Zero, Color.White);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            //World

            world.Render(spriteBatch);
            graphics.GraphicsDevice.Clear(Color.DeepSkyBlue);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.transform);
            world.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, blendStateMultiply, SamplerState.PointClamp, null, null, null, camera.transform);
            spriteBatch.Draw(rtLight, new Vector2(chunkX * Chunk.SIZE, chunkY * Chunk.SIZE), null, Color.White, 0, Vector2.Zero, 1 ,SpriteEffects.None, 0);
            spriteBatch.End();

            //UI
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            UIMaster.Draw(spriteBatch);
            spriteBatch.Draw(rtLightSource, new Vector2(4, GraphicsDevice.Viewport.Height - rtLight.Height - 4), Color.White);
            spriteBatch.Draw(rtLightBlur, new Vector2(8 + rtLight.Width, GraphicsDevice.Viewport.Height - rtLight.Height - 4), Color.White);
            spriteBatch.Draw(rtLight, new Vector2(12 + rtLight.Width * 2, GraphicsDevice.Viewport.Height - rtLight.Height - 4), Color.White);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        public void OnResize(Object sender, EventArgs e)
        {
            camera.Resize(GraphicsDevice.Viewport);
            UIMaster.Resize(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            RecalcLightRT();
        }

        private void RecalcLightRT()
        {
            RectF rect = camera.GetRect();
            int w = (int)(Math.Floor(rect.Width / Chunk.SIZE) + 2) * Chunk.SIZE;
            int h = (int)(Math.Floor(rect.Height / Chunk.SIZE) + 2) * Chunk.SIZE;
            rtLight = new RenderTarget2D(GraphicsDevice, w, h);
            rtLightBlur = new RenderTarget2D(GraphicsDevice, w, h);
            rtLightSource = new RenderTarget2D(GraphicsDevice, w, h);
            lightSourceColor = new Color[w * h];
            Console.WriteLine("New Light RT Dimensions: {0}, {1}", w, h);
        }
    }
}
