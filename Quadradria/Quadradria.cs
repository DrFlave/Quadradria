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
            Textures.Load(Content, GraphicsDevice);

            chunk = new Chunk(0, 0, GraphicsDevice);
            chunk2 = new Chunk(1, 1, GraphicsDevice);
            testItem = new Item(new ItemType("item.sword", 1, Textures.Items.Sword));

            UIMaster = new UIContainer(20, 20, 300, 300);
            new UILabel(0, 0, 1, 1, "Center", UIMaster, UISizeMethod.UV, UIAlignment.Center);
            new UILabel(0, 0, 1, 1, "Top", UIMaster, UISizeMethod.UV, UIAlignment.Top);
            new UILabel(0, 0, 1, 1, "Bottom", UIMaster, UISizeMethod.UV, UIAlignment.Bottom);
            new UILabel(0, 0, 1, 1, "Left", UIMaster, UISizeMethod.UV, UIAlignment.Left);
            new UILabel(0, 0, 1, 1, "Right", UIMaster, UISizeMethod.UV, UIAlignment.Right);
            new UILabel(0, 0, 1, 1, "Right Top", UIMaster, UISizeMethod.UV, UIAlignment.Right | UIAlignment.Top);
            new UILabel(0, 0, 1, 1, "Right Bottom", UIMaster, UISizeMethod.UV, UIAlignment.Right | UIAlignment.Bottom);
            new UILabel(0, 0, 1, 1, "Left Top", UIMaster, UISizeMethod.UV, UIAlignment.Left | UIAlignment.Top);
            new UILabel(0, 0, 1, 1, "Left Bottom", UIMaster, UISizeMethod.UV, UIAlignment.Left | UIAlignment.Bottom);

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
