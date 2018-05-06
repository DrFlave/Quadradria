using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Enviroment
{
    class World
    {
        
        private LoadedChunksManager LoadedChunks = new LoadedChunksManager();
        private ChunkLoader chunkLoader = new ChunkLoader();
        

        public World(GraphicsDevice graphicsDevice)
        {
            chunkLoader.init(graphicsDevice);
        }

        public void Update(float x, float y, float width, float height)
        {
            int cX = (int)Math.Floor(x / 16);
            int cY = (int)Math.Floor(y / 16);
            int cW = (int)Math.Floor((x + width) / 16) - cX;
            int cH = (int)Math.Floor((y + height) / 16) - cY;

            LoadedChunks.UpdateLoadedArea(cX, cY, cW, cH, chunkLoader);
        }

        public void Render(SpriteBatch spriteBatch)
        {
            LoadedChunks.ForEach((chunk) => {
                chunk.Render(spriteBatch);
            });
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            LoadedChunks.ForEach((chunk) => {
                chunk.Draw(spriteBatch);
            });
        }
    }
}
