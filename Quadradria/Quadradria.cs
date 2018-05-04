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
        Item testItem;

        Player player;

        Camera camera;

        Chunk chunk;
        Chunk chunk2;

        UIContainer UIMaster;

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
            Textures.Solid = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Textures.Solid.SetData<Color>(new Color[1] { Color.White });

            chunk = new Chunk(0, 0, GraphicsDevice);
            chunk2 = new Chunk(1, 1, GraphicsDevice);
            testItem = new Item(new ItemType("item.sword", 1, Textures.Items.Sword));

            UIMaster = new UIContainer(0, 0, 200, 200);
            UIMaster.color = Color.Orange;
            UIContainer UIInner = new UIContainer(0.1f, 0.1f, 0.8f, 0.8f, UIMaster);
            UIInner.color = Color.Blue;
            UIContainer UIAbsolute = new UIContainer(10, 10, 100, 50, UIInner, UIContainer.SizeMethod.Pixel);

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
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

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                camera.zoom += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                camera.zoom -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                camera.rotation -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                camera.rotation += 0.02f;

            
            player.Update(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
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

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            UIMaster.Draw(spriteBatch);
            testItem.Draw(spriteBatch, 32, 32, 4);
       
            spriteBatch.End();


            base.Draw(gameTime);
        }

        public void OnResize(Object sender, EventArgs e)
        {
            camera.Resize(GraphicsDevice.Viewport);
        }
    }
}
