using Quadradria.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Enviroment.Generators
{
    class GenOverworld : IGenerator
    {
        public int Seed { get; set; }
        private OpenSimplexNoise noise;
        private OpenSimplexNoise noiseCave1;
        private OpenSimplexNoise noiseCave2;
        private OpenSimplexNoise dirtDepth;
        private WorldInfo info;

        public GenOverworld(WorldInfo info)
        {
            this.info = info;

            noise = new OpenSimplexNoise(info.seed);
            noiseCave1 = new OpenSimplexNoise(info.seed);
            noiseCave2 = new OpenSimplexNoise((info.seed == 0) ? 1 : info.seed - Math.Sign(info.seed));
            dirtDepth = new OpenSimplexNoise((info.seed == 0) ? 2 : info.seed - Math.Sign(info.seed) * 2);
        }

        public void GenerateChunk(Chunk chunk)
        {
            int cx = chunk.pos.X * Chunk.SIZE;
            int cy = chunk.pos.Y * Chunk.SIZE;
            
            for (int x = 0; x < Chunk.SIZE; x++)
            {

                float h1 = noise.Generate((x + cx) * 0.0005f) * 50;
                float h2 = noise.Generate((x + cx) * 0.005f) * 25;
                float h3 = noise.Generate((x + cx) * 0.05f) * 3;
                int height = (int)(h1 + h2 + h3);
                int dirth = 18 + (int)Math.Abs(noise.Generate((x + cx) * 0.05f) * 10);

                for (int y = 0; y < Chunk.SIZE; y++)
                {
                    int worldX = cx + x;
                    int worldY = cy + y;

                    if (worldY < height) chunk.Blocks[x, y] = new Block(BlockType.Air, 0);
                    else if (worldY == height) chunk.Blocks[x, y] = new Block(BlockType.Dirt, 0b10);
                    else if (worldY > height && worldY < height + dirth) chunk.Blocks[x, y] = new Block(BlockType.Dirt, 0);
                    else chunk.Blocks[x, y] = new Block(BlockType.Stone, 0);
                }
            }

            bool[,] cave = new bool[Chunk.SIZE + 2, Chunk.SIZE + 2];

            for (int y = -1; y < Chunk.SIZE + 1; y++)
            {
                for (int x = -1; x < Chunk.SIZE + 1; x++)
                {
                    int worldX = cx + x;
                    int worldY = cy + y;

                    float t = 0.2f;
                    if (worldY < 150)
                        t = 0.175f + (worldY / 150f) * 0.025f;

                    float c1 = Math.Abs(noiseCave1.Generate((worldX) / 100f, (worldY) / 100f)); //wolken
                    float c2 = Math.Abs(noiseCave2.Generate((worldX) / 80f, (worldY) / 50f)); //schlauch
                    float c3 = noiseCave2.Generate((worldX) * 0.05f, (worldY) * 0.05f) * 0.15f; //besser aussehen

                    if ((c1 * 0.35 + (c2 + c3) * 0.65) < t) cave[x+1, y+1] = true;
                }
            }
            
            int num, xx, yy;
            for (int i = 0; i<Chunk.SIZE; i++)
            {
                for (int j = 0; j<Chunk.SIZE; j++)
                {
                    xx = i + 1;
                    yy = j + 1;

                    num = (cave[xx, yy + 1] ? 1 : 0)
                        + (cave[xx, yy - 1] ? 1 : 0)
                        + (cave[xx + 1, yy] ? 1 : 0)
                        + (cave[xx - 1, yy] ? 1 : 0);

                    if (num >= 3)
                    {
                        chunk.Blocks[i, j] = new Block(BlockType.Air, 0);
                    }
                }
            }
        }
    }
}
