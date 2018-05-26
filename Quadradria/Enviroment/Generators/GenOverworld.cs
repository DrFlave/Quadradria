using Microsoft.Xna.Framework.Graphics;
using Quadradria.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quadradria.Enviroment.Generators
{
    class GenOverworld : IGenerator
    {
        public int Seed { get; set; }
        private OpenSimplexNoise noise;
        private OpenSimplexNoise noiseCave1;
        private OpenSimplexNoise noiseCave2;
        private OpenSimplexNoise noiseOreCopper;
        private OpenSimplexNoise noiseOreTin;
        private OpenSimplexNoise dirtDepth;
        private OpenSimplexNoise noiseWater;
        private OpenSimplexNoise noiseWater2;
        private WorldInfo info;

        public GenOverworld(WorldInfo info)
        {
            this.info = info;

            noise = new OpenSimplexNoise(info.seed);
            noiseCave1 = new OpenSimplexNoise(info.seed);
            noiseCave2 = new OpenSimplexNoise((info.seed == 0) ? 1 : info.seed - Math.Sign(info.seed));
            noiseOreCopper = new OpenSimplexNoise((info.seed == 0) ? 3 : info.seed - Math.Sign(info.seed) * 3);
            noiseOreTin = new OpenSimplexNoise((info.seed == 0) ? 4 : info.seed - Math.Sign(info.seed) * 4);
            dirtDepth = new OpenSimplexNoise((info.seed == 0) ? 2 : info.seed - Math.Sign(info.seed) * 2);
            noiseWater = new OpenSimplexNoise((info.seed == 0) ? 5 : info.seed - Math.Sign(info.seed) * 5);
            noiseWater2 = new OpenSimplexNoise((info.seed == 0) ? 6 : info.seed - Math.Sign(info.seed) * 6);
        }

        public Task GenerateMegachunk(Megachunk mc, GraphicsDevice graphicsDevice, Action callback)
        {
            return Task.Run(() =>
            {
                Task[] tasks = new Task[Megachunk.SIZE * Megachunk.SIZE];

                for (int y = 0; y < Megachunk.SIZE; y++)
                {
                    for (int x = 0; x < Megachunk.SIZE; x++)
                    {
                        Chunk c = new Chunk(mc.WorldX * Megachunk.SIZE + x, mc.WorldY * Megachunk.SIZE + y, graphicsDevice);
                        GenerateChunk(c);
                        tasks[x + y * Megachunk.SIZE] = mc.SaveChunk(x, y, c.Export());
                    }
                }

                Task.WaitAll(tasks);

                callback();
            });

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
                    BlockType blockType = BlockType.Air;
                    ushort subid = 0;

                    int worldX = cx + x;
                    int worldY = cy + y;

                    if (worldY < height) chunk.Backgrounds[x, y] = new Background(BackgroundType.None);
                    else if (worldY >= height && worldY <= height + dirth) chunk.Backgrounds[x, y] = new Background(BackgroundType.Dirt);
                    else chunk.Backgrounds[x, y] = new Background(BackgroundType.Stone);

                    if (worldY < height) { blockType = BlockType.Air; subid = 0; }
                    else if (worldY == height) { blockType = BlockType.Dirt; subid = 0b10; }
                    else if (worldY > height && worldY < height + dirth) { blockType = BlockType.Dirt; subid = 0; }
                    else { blockType = BlockType.Stone; subid = 0; };

                    if (worldY == height - 1)
                    {
                        if (Math.Abs(noise.Generate((float)worldX)) > 0.2) { blockType = BlockType.GrassBush; subid = 0; } 
                    }

                    if (worldY > height + dirth)
                    {
                        bool cop, tin;
                        cop = (noiseOreCopper.Generate((worldX) * 0.15f, (worldY) * 0.15f) > 0.65);
                        tin = (noiseOreTin.Generate((worldX) * 0.15f, (worldY) * 0.15f) > 0.65);

                        if (cop && tin)
                        {
                            if ((x + y) % 2 == 0)
                                cop = false;
                            else
                                tin = false;
                        } 

                        if (cop) { blockType = BlockType.OreCopper; subid = 0; }
                        if (tin) { blockType = BlockType.OreTin; subid = 0; }

                        if (noiseWater.Generate((worldX) * 0.15f, (worldY) * 0.15f) > 0.75) { blockType = BlockType.Water; subid = 0; }
                        if (worldY > height + dirth + 50)
                        if (noiseWater.Generate((worldX) * 0.03f, (worldY) * 0.03f) > 0.6) { blockType = BlockType.Water; subid = 0; }
                    }

                    if (worldY < -100)
                    {
                        if (noiseOreCopper.Generate((worldX) * 0.02f, (worldY) * 0.08f) > 0.65) { blockType = BlockType.Cloud; subid = 0; }
                    }

                    chunk.Blocks[x, y] = new Block(blockType, subid);
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

            chunk.IsGenerated = true;
        }
    }
}
