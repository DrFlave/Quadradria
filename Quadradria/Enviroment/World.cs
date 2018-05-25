using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quadradria.Entity;
using Quadradria.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Enviroment
{
    class WorldInfo
    {
        public uint width, timeOfDay, lengthOfDay;
        private string name;
        public ulong creationTime, playTime;
        public long seed;
        public Generator generator;
        public WorldSize Size;
        public Difficulty difficulty;

        public string Name {
            get => name;
            set {
                name = value;
                if (value.Length > 128)
                {
                    name = value.Substring(0, 128);
                }
            }
        }

        public WorldInfo()
        {
            seed = 0x1;
            width = 0;
            timeOfDay = 0xabcdef99;
            lengthOfDay = 0x12345678;
            Name = "unnamed world";
            difficulty = Difficulty.Ultra;
            generator = Generator.Skyblock;
            Size = WorldSize.Small;
            creationTime = 0x1212121212121212;
            playTime = 0x5454545454545454;
        }
    }

    enum WorldSize : byte { Small = 0, Medium = 1, Large = 2 }
    enum Difficulty : byte { Easy = 0, Medium = 1, Hard = 2, Ultra = 0xFF }
    enum Generator : byte { Default = 0, Hell = 1, Island = 2, Skyblock = 3 }

    class World
    {

        private WorldLoader worldLoader;
        public LoadedChunksManager LoadedChunks;

        public WorldInfo Info { get; set; }

        private string path;

        private uint nextEntId = 0;

        public World(string path, GraphicsDevice graphicsDevice)
        {
            Info = new WorldInfo();

            try
            {
                worldLoader = new WorldLoader(path, graphicsDevice, Info);
            } catch (Exception e)
            {
                throw new Exception("Can't create World", e);
            }
            LoadedChunks = new LoadedChunksManager(worldLoader);

            this.path = path;
        }

        public void Update(float x, float y, float width, float height)
        {
            int cX = (int)Math.Floor(x / 16);
            int cY = (int)Math.Floor(y / 16);
            int cW = (int)Math.Ceiling((x + width) / 16) - cX;
            int cH = (int)Math.Ceiling((y + height) / 16) - cY;

            LoadedChunks.UpdateLoadedArea(cX, cY, cW, cH, worldLoader);
            LoadedChunks.ForEachLoaded((chunk) =>
            {
                chunk.Update(this);
            });
        }

        public void Render(SpriteBatch spriteBatch)
        {
            LoadedChunks.ForEachVisible((chunk) => {
                chunk.Render(spriteBatch);
            });
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            LoadedChunks.ForEachVisible((chunk) => {
                chunk.Draw(spriteBatch);
            });
        }

        public void AddEntity(BaseEntity entity)
        {
            Chunk chunk = GetChunkAtTilePosition((int)Math.Floor(entity.Position.X), (int)Math.Floor(entity.Position.Y));
            if (chunk == null) {
                //Something went wrong :(
                Console.WriteLine("Can't create entity because there is no chunk! Entity position: ({0}, {1}), Chunk position: ({2}, {3})");
                return;
            };

            entity.Initialize(RequestEntityId());

            chunk.AddEntity(entity);
        }

        public uint RequestEntityId()
        {
            return nextEntId++;
        }
        
        public Chunk GetChunkAtTilePosition(int x, int y)
        {
            int cX = (int)Math.Floor((float)x / Chunk.SIZE);
            int cY = (int)Math.Floor((float)y / Chunk.SIZE);
            return LoadedChunks.GetChunk(cX, cY);
        }
        
        public Block? GetBlockAtPosition(int x, int y)
        {
            Chunk c = GetChunkAtTilePosition(x, y);
            if (c == null) return null;

            return c.GetBlockAtLocalPosition(Tools.Mod(x, Chunk.SIZE), Tools.Mod(y, Chunk.SIZE));
        }

        public void SetBlockAtPosition(int x, int y, BlockType type, ushort subid)
        {
            Chunk c = GetChunkAtTilePosition(x, y);
            if (c == null) return;

            c.SetBlockAtLocalPosition(Tools.Mod(x, Chunk.SIZE), Tools.Mod(y, Chunk.SIZE), type, subid);
        }

        public void Save()
        {
            worldLoader.WriteWorld(true);
            LoadedChunks.Unload();
        }

        public int GetNumberOfLoadedMegachunks()
        {
            return worldLoader.GetNumberOfLoadedMegachunks();
        }

        public void Unload()
        {
            Save();
            //worldLoader.Unload();
        }

    }
}
