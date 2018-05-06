using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Enviroment
{
    class ChunkLoader
    {

        private Chunk[,] AllChunks = new Chunk[100, 100];

        public void init(GraphicsDevice graphicsDevice)
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    AllChunks[j, i] = new Chunk(j, i, graphicsDevice);
                }
            }
        }

        public Chunk loadChunk(int x, int y)
        {
            if (x < 0 || y < 0 || x > 99 || y > 99) return null;
            return (AllChunks[x, y]);
        }
    }

}
