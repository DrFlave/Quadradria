using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.World
{
    class Chunk
    {
        public const int SIZE = 4;
        public const int BLOCK_SIZE = 32;

        public Block[,] blocks = new Block[SIZE, SIZE];
        public Vector2 pos;
        public Vector2 drawPos;

        RenderTarget2D renderTarget;
        GraphicsDevice graphicsDevice;

        public bool shouldRender = true;

        public Chunk(int x, int y, GraphicsDevice graphicsDevice) {
            this.pos = new Vector2(x, y);
            this.drawPos = new Vector2(x * SIZE, y * SIZE);

            this.graphicsDevice = graphicsDevice;

            renderTarget = new RenderTarget2D(graphicsDevice, SIZE* BLOCK_SIZE, SIZE * BLOCK_SIZE);

            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    blocks[i, j] = new Block();
                }
            }
        }

        public void Render(SpriteBatch spriteBatch)
        {
            if (!shouldRender) return;

            graphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    blocks[i, j].Draw(spriteBatch, i * BLOCK_SIZE, j * BLOCK_SIZE);
                }
            }
            
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);

            shouldRender = false;
        }

        public void Draw(SpriteBatch spriteBatch) {
            //spriteBatch.Draw(renderTarget, drawPos, Color.White);

            float scale = 1.0f / BLOCK_SIZE;
            spriteBatch.Draw(renderTarget, drawPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        }
    }
}
