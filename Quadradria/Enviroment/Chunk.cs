using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quadradria.Entity;
using System;
using System.Collections.Generic;
using System.IO;
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
        public Background[,] Backgrounds = new Background[SIZE, SIZE];

        private Color[] lightMap = new Color[SIZE * SIZE];
        public Texture2D LightTexture;
        
        public List<BaseEntity> entities = new List<BaseEntity>();

        RenderTarget2D renderTarget;
        GraphicsDevice graphicsDevice;

        public bool shouldRender = true;
        public bool IsGenerated = false;

        public Chunk(int x, int y, GraphicsDevice graphicsDevice)
        {
            this.pos = new Point(x, y);
            this.drawPos = new Vector2(x * SIZE, y * SIZE);
            this.graphicsDevice = graphicsDevice;
        }

        public void Load()
        {
            if (isLoaded) return;

            renderTarget = new RenderTarget2D(graphicsDevice, SIZE * BLOCK_SIZE, SIZE * BLOCK_SIZE);
            LightTexture = new Texture2D(graphicsDevice, SIZE, SIZE);
            LightTexture.SetData<Color>(lightMap);

            isLoaded = true;
        }

        public void Unload()
        {
            if (!isLoaded) return;

            renderTarget.Dispose();
            LightTexture.Dispose();
            isLoaded = false;
        }

        public void Render(SpriteBatch spriteBatch)
        {
            if (!shouldRender || renderTarget == null || !isLoaded) return;

            graphicsDevice.SetRenderTarget(renderTarget);

            graphicsDevice.Clear(Color.Transparent);

            LightTexture.SetData<Color>(lightMap);

            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    Backgrounds[i, j].Draw(spriteBatch, i * BLOCK_SIZE, j * BLOCK_SIZE);
                    Blocks[i, j].Draw(spriteBatch, i * BLOCK_SIZE, j * BLOCK_SIZE);
                }
            }

            spriteBatch.End();
            
            graphicsDevice.SetRenderTarget(null);

            shouldRender = false;
        }

        public void Update(GameTime gameTime, World world)
        {
            Random randomTickGenerator = new Random();
            int rx = randomTickGenerator.Next(SIZE);
            int ry = randomTickGenerator.Next(SIZE);

            Blocks[rx, ry].RandomTick(rx + pos.X * SIZE, ry + pos.Y * SIZE, world);

            foreach (BaseEntity ent in entities)
            {
                ent.Update(gameTime, world);
            }
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

        public void DrawLight(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(LightTexture, drawPos, Color.White);
        }

        public Block? GetBlockAtLocalPosition(int x, int y)
        {
            if (!isLoaded) return null;
            if (x < 0 || x >= SIZE || y < 0 || y >= SIZE) return null;
            return Blocks[x, y];
        }

        public void SetBlockAtLocalPosition(int x, int y, BlockType type, ushort subid, byte damage = 0)
        {
            if (x < 0 || y < 0 || x >= SIZE || y >= SIZE) return;
            Blocks[x, y].BlockID = type;
            Blocks[x, y].SubID = subid;
            Blocks[x, y].Damage = damage;
            shouldRender = true;

            CalculateLight(x, y);

            LightTexture.SetData<Color>(lightMap);
        }

        public void CalculateLight(int x, int y)
        {
            Block block = Blocks[x, y];
            if (block.IsSolid())
            {
                lightMap[y * SIZE + x] = block.GetLight();
            } else
            {
                lightMap[y * SIZE + x] = (Backgrounds[x, y].BType == BackgroundType.None) ? Color.White : Color.Black;
            }
        }

        public void AddEntity(BaseEntity entity)
        {
            if (!isLoaded) return;
            entities.Add(entity);
        }

        public void RemoveEntity(BaseEntity entity)
        {
            if (!isLoaded) return;
            entities.Remove(entity);
        }

        public void Import(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                long entBlockSize;
                int entityNumber, index, i, j;

                try
                {
                    lock (this)
                    {
                        for (i = 0; i < SIZE; i++)
                        {
                            for (j = 0; j < SIZE; j++)
                            {
                                index = 4 * (i * SIZE + j);

                                Blocks[j, i].Damage = 0;
                                Blocks[j, i].BlockID = (BlockType)reader.ReadUInt16();
                                Blocks[j, i].SubID = reader.ReadUInt16();
                                Backgrounds[j, i].BType = (BackgroundType)reader.ReadUInt16();
                            }
                        }

                        for (i = 0; i < SIZE; i++)
                        {
                            for (j = 0; j < SIZE; j++)
                            {
                                CalculateLight(i, j);
                            }
                        }

                        entBlockSize = reader.ReadInt64();
                        entityNumber = reader.ReadInt32();

                        for (i = 0; i < entityNumber; i++)
                        {
                            EntityType type = (EntityType)reader.ReadUInt16();
                            uint ID = reader.ReadUInt32();
                            float x = reader.ReadSingle();
                            float y = reader.ReadSingle();
                            int len = reader.ReadInt32();
                            byte[] entData = reader.ReadBytes(len);


                            BaseEntity entity = EntityManager.Spawn(type);
                            if (entity != null)
                            {
                                entity.Position.X = x;
                                entity.Position.Y = y;
                                entity.Import(entData);
                                entity.Initialize(ID);

                                AddEntity(entity);
                            }
                        }

                    }
                } catch (Exception e)
                {
                    throw new Exception("The Chunk data was corrupted.", e);
                }
            }

            shouldRender = true;
            IsGenerated = true;
        }

        public byte[] Export()
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                lock (this)
                {
                    for (int i = 0; i < SIZE; i++)
                    {
                        for (int j = 0; j < SIZE; j++)
                        {
                            int index = 4 * (i * SIZE + j);

                            Block block = Blocks[j, i];
                            writer.Write((ushort)block.BlockID);
                            writer.Write((ushort)block.SubID);
                            writer.Write((ushort)Backgrounds[j, i].BType);
                        }
                    }

                    long entsLengthPos = stream.Position;
                    writer.Write((long)0);
                    writer.Write((int)entities.Count);

                    foreach (BaseEntity ent in entities)
                    {
                        byte[] entData = ent.Export();
                        writer.Write((ushort)ent.EntType);
                        writer.Write((uint)ent.ID);
                        writer.Write((float)ent.Position.X);
                        writer.Write((float)ent.Position.Y);
                        writer.Write(entData.Length);
                        writer.Write(entData);
                    }

                    long len = stream.Position - entsLengthPos;
                    stream.Seek(entsLengthPos, SeekOrigin.Begin);
                    writer.Write((long)len);

                    return stream.ToArray();//stream.GetBuffer();
                }
            }

        }
    }
}
