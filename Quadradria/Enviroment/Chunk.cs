using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quadradria.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Enviroment
{
    class Chunk
    {
        public const int SIZE = 16;
        public const int BLOCK_SIZE = 16;

        private bool isLoaded;

        public Point pos;
        public Vector2 drawPos;

        public Block[,] Blocks = new Block[SIZE, SIZE];
        public List<BaseEntity> entities = new List<BaseEntity>();

        RenderTarget2D renderTarget;
        GraphicsDevice graphicsDevice;

        public bool shouldRender = true;

        public Chunk(int x, int y, GraphicsDevice graphicsDevice)
        {
            this.pos = new Point(x, y);
            this.drawPos = new Vector2(x * SIZE, y * SIZE);
            this.graphicsDevice = graphicsDevice;

            renderTarget = new RenderTarget2D(graphicsDevice, SIZE * BLOCK_SIZE, SIZE * BLOCK_SIZE);
        }

        public void Load()
        {
            if (isLoaded) return;
            isLoaded = true;
        }

        public void Unload()
        {
            if (!isLoaded) return;
            isLoaded = false;

            renderTarget.Dispose();
        }

        public void Render(SpriteBatch spriteBatch)
        {
            if (!shouldRender || renderTarget == null || !isLoaded) return;

            graphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    Blocks[i, j].Draw(spriteBatch, i * BLOCK_SIZE, j * BLOCK_SIZE);
                }
            }

            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);

            shouldRender = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isLoaded) return;

            float scale = 1.0f / BLOCK_SIZE;
            spriteBatch.Draw(renderTarget, drawPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);

            entities.ForEach((entity) =>
            {
                entity.Draw(spriteBatch);
            });
        }

        public Block? GetBlockAtLocalPosition(int x, int y)
        {
            if (!isLoaded) return null;
            if (x < 0 || x >= SIZE || y < 0 || y >= SIZE) return null;
            return Blocks[x, y];
        }

        public void AddEntity(BaseEntity entity)
        {
            if (!isLoaded) return;
            entities.Add(entity);
        }

        public void RemoveEntiy(BaseEntity entity)
        {
            if (!isLoaded) return;
            entities.Remove(entity);
        }

        public byte[] Export()
        {
            if (!isLoaded) return null;
            byte[] array = new byte[1024];
            int index, i, j;

            for (i = 0; i < SIZE; i++)
            {
                for (j = 0; j < SIZE; j++)
                {
                    index = 4 * (i * SIZE + j);

                    Block block = Blocks[j, i];
                    array[index + 0] = ((byte)(block.BlockID));
                    array[index + 1] = ((byte)((ushort)block.BlockID >> 8));
                    array[index + 2] = ((byte)(block.SubID));
                    array[index + 3] = ((byte)(block.SubID >> 8));
                }
            }

            return array;
        }

        public void Import(byte[] data)
        {
            if (data.Length != SIZE*SIZE*4) return; //!= 1024

            int index, i, j;
            for (i = 0; i < SIZE; i++)
            {
                for (j = 0; j < SIZE; j++)
                {
                    index = 4 * (i * SIZE + j);

                    Blocks[j, i].damage = 0;
                    Blocks[j, i].BlockID = (BlockType)BitConverter.ToUInt16(data, index);
                    Blocks[j, i].SubID = BitConverter.ToUInt16(data, index + 2);
                }
            }
            Load();
        }
    }
}
