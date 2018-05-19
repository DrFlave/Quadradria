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
    class WorldInfo
    {
        public uint seed, width, timeOfDay, lengthOfDay;
        private string name;
        public byte difficulty, generator;
        public ulong creationTime, playTime;
        public WorldSize Size;

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
            timeOfDay = 0;
            lengthOfDay = 0xFFFF;
            Name = "unnamed world";
            difficulty = 1;
            generator = 0;
            Size = WorldSize.Small;
            creationTime = 0;
            playTime = 0;
        }
    }

    enum WorldSize : byte { Small = 0, Medium = 1, Large = 2 }

    class World
    {

        private WorldLoader worldLoader;
        private LoadedChunksManager LoadedChunks;

        public WorldInfo Info { get; set; }

        private string path;

        private int nextEntId = 0;

        public World(string path, GraphicsDevice graphicsDevice)
        {
            try
            {
                worldLoader = new WorldLoader(path);
            } catch (Exception e)
            {
                throw new Exception("Can't create World", e);
            }
            LoadedChunks = new LoadedChunksManager(worldLoader);

            this.path = path;
            Info = new WorldInfo();

            worldLoader.Init(graphicsDevice);
            worldLoader.WriteWorld(Info);
            worldLoader.LoadWorld();
        }

        public void Update(float x, float y, float width, float height)
        {
            int cX = (int)Math.Floor(x / 16);
            int cY = (int)Math.Floor(y / 16);
            int cW = (int)Math.Ceiling((x + width) / 16) - cX;
            int cH = (int)Math.Ceiling((y + height) / 16) - cY;

            LoadedChunks.UpdateLoadedArea(cX, cY, cW, cH, worldLoader);
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

        /*public void AddEntity(BaseEntity entity)
        {
            Chunk chunk = GetChunkAtTilePosition((int)Math.Floor(entity.Position.X), (int)Math.Floor(entity.Position.Y));
            /*if (chunk == null) {
                //Something went wrong :(
                Console.WriteLine("Can't create entity because there is no chunk! Entity position: ({0}, {1}), Chunk position: ({2}, {3})");
                return;
            };

            entity.Initialize(RequestEntityId());

            chunk.AddEntity(entity);
        }*/

        public int RequestEntityId()
        {
            return nextEntId++;
        }
        /*
        public Chunk GetChunkAtTilePosition(int x, int y)
        {
            int cX = (int)Math.Floor((float)x / Chunk.SIZE);
            int cY = (int)Math.Floor((float)y / Chunk.SIZE);
            return LoadedChunks.GetChunk(cX, cY);
        }
        */

        /*
        public Block? GetBlockAtPosition(int x, int y)
        {
            Chunk c = GetChunkAtTilePosition(x, y);
            if (c == null) return null;

            return c.GetBlockAtLocalPosition(x % Chunk.SIZE, y % Chunk.SIZE);
        }
        */

        public void Save()
        {
            worldLoader.WriteWorld(Info);
        }

    }
}
