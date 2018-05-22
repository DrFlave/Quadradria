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

        public void GenerateChunk(Chunk chunk)
        {
            BlockType[] types = { BlockType.Air, BlockType.Dirt, BlockType.DoorWood, BlockType.Stone };



            int cx = chunk.pos.X * Chunk.SIZE;
            int cy = chunk.pos.Y * Chunk.SIZE;

            for(int x = 0; x < Chunk.SIZE; x++)
            {
                int height = (int)Math.Floor(Math.Sin((x + cx)/20.0)*10);
                for(int y = 0; y < Chunk.SIZE; y++)
                {
                    chunk.Blocks[x, y] = new Block(types[Math.Abs(chunk.pos.X % 4)], 0);

                    /*int worldX = cx + x;
                    int worldY = cy + y;

                    if (worldY < height) chunk.Blocks[x, y] = new Block(BlockType.Air, 0);
                    else if (worldY == height) chunk.Blocks[x, y] = new Block(BlockType.Grass, 2);
                    else if (worldY > height && worldY < height + 10) chunk.Blocks[x, y] = new Block(BlockType.Dirt, 0);
                    else chunk.Blocks[x, y] = new Block(BlockType.Stone, 0);*/
                }
            }
        }
    }
}
