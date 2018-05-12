using Microsoft.Xna.Framework.Graphics;
using Quadradria.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quadradria.Enviroment
{
    class WorldLoader
    {

        /*
         * Files per world:
         * world.qwld           //World Data
         * 4    magic number    //ever 0x42171701
         * 4    file version
         * 4    seed
         * 128  world name  
         * 4    width           // 0 => infinity
         * 1    worldSize
         * 4    creationTime    //Timestamp. Seconds since 01.01.1970
         * 4    playtime        //seconds
         * 1    difficulty
         * 1    generator
         * 4    timeOfDay
         * 4    lengthOfDay
         * 4    chunkIndexSize
         * repeat(chunkIndexSize) { //Chunk specific values
         *      4    x          
         *      4    y
         *      8    address        //Address in chunks.cdat                                
         * }
         * 
         * 
         * 
         * 
         * chunks.cdat       //Block Data
         * chunk(1024bytes), chunk...
         *
         *
         * 
         * entData.edat         //Entity Data
         * 
         * */

        private Chunk[,] AllChunks = new Chunk[100, 100];


        public void Init(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    AllChunks[j, i] = new Chunk(j, i, graphicsDevice);
                }
            }
        }

        public Chunk LoadChunk(int x, int y)
        {
            if (x < 0 || y < 0 || x > 99 || y > 99) return null;
            return (AllChunks[x, y]);
        }

        private FileStream fsChunk;
        private FileStream fsWorld;
        private string worldPath;
        private GraphicsDevice graphicsDevice;

        private List2D<long> chunkIndex = new List2D<long>();

        public WorldLoader()
        {

        }

        public WorldLoader(string fileName)
        {
            this.worldPath = fileName;

            fsChunk = new FileStream(worldPath + @"\chunks.cdat", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            fsWorld = new FileStream(worldPath + @"\world.qwld", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        }

        public Chunk GetChunk(int x, int y)
        {
            if (chunkIndex.Includes(x, y))
            {
                Chunk chunk = new Chunk(x, y, graphicsDevice);
                ReadChunk(chunk);
                return chunk;
            } else {
                Chunk chunk = new Chunk(x, y, graphicsDevice);
                //ToDo: Woldgenerator here!
                return chunk;
            }
        }

        public void ReadChunk(Chunk chunk)
        {
            long? address = chunkIndex.Get(chunk.pos.X, chunk.pos.Y);
            if (address == null) return;

            fsChunk.Seek((long)address, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(fsChunk);

            byte[] bytes = reader.ReadBytes(1024);
            chunk.Import(bytes);
        }

        public void WriteChunk(Chunk chunk)
        {
            long address;
            BinaryWriter writerChunk = new BinaryWriter(fsChunk);
            BinaryWriter writerWorld = new BinaryWriter(fsWorld);

            bool write = false;
            if (!chunkIndex.Includes(chunk.pos.X, chunk.pos.Y))
            {
                address = (uint)fsChunk.Length;
                chunkIndex.Add(chunk.pos.X, chunk.pos.Y, address);
                write = true;
            }
            else
            {
                address = chunkIndex.Get(chunk.pos.X, chunk.pos.Y);
            }

            Task task = new Task(() =>
            {
                if(write)
                {
                    writerWorld.Write(chunk.pos.X);
                    writerWorld.Write(chunk.pos.Y);
                    writerWorld.Write(address);
                }
                fsChunk.Seek(address, SeekOrigin.Begin);
                writerChunk.Write(chunk.Export());
            });
            task.Start();
        }

        public void WriteWorld(WorldInfo worldInfo)
        {
            Task task = new Task(() =>
            {
                fsWorld.Seek(0, SeekOrigin.Begin);
                BinaryWriter writer = new BinaryWriter(fsWorld);
                writer.Write((uint)0x42171701);    //Magic Number
                writer.Write((uint)0x1);           //Version
                writer.Write((uint)worldInfo.seed);
                writer.Write(Encoding.UTF8.GetBytes(worldInfo.Name.PadRight(128, '\0')));
                writer.Write((uint)worldInfo.width);
                writer.Write((byte)worldInfo.Size);
                writer.Write((ulong)worldInfo.creationTime);
                writer.Write((ulong)worldInfo.playTime);
                writer.Write((byte)worldInfo.difficulty);
                writer.Write((byte)worldInfo.generator);
                writer.Write((uint)worldInfo.timeOfDay);
                writer.Write((uint)worldInfo.lengthOfDay);
                writer.Write((uint)chunkIndex.Length);

                uint chunkIndexSize = (uint)chunkIndex.Length;

                chunkIndex.ForEachWrapper((chunk) => {
                    writer.Write(chunk.x);
                    writer.Write(chunk.y);
                    writer.Write(chunk.item);
                });
            });
            task.Start();
        }

        public void LoadWorld()
        {
            fsWorld.Seek(0, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(fsWorld);

            uint magicNumber = reader.ReadUInt32();
            uint version = reader.ReadUInt32();
            uint seed = reader.ReadUInt32();

            char[] worldChars = reader.ReadChars(128);

            string worldName = "";
            for (int i = 0; i < 128; i++)
            {
                if (worldChars[i] == 0) break;
                worldName += worldChars[i];
            }

            uint width = reader.ReadUInt32();
            byte worldSize = reader.ReadByte();
            ulong creationTime = reader.ReadUInt32();
            ulong playTime = reader.ReadUInt32();
            byte difficulty = reader.ReadByte();
            byte generator = reader.ReadByte();
            uint timeOfDay = reader.ReadUInt32();
            uint lengthOfDay = reader.ReadUInt32();

            Console.WriteLine("magic number: {0}, {1}", magicNumber, magicNumber == 0x42171701 ? "Matches." : "No Match!");
            Console.WriteLine("version: {0}", version);
            Console.WriteLine("seed: {0}", seed);
            Console.WriteLine("worldName: {0}", worldName);
            Console.WriteLine("width: {0}", width);
            Console.WriteLine("worldSize: {0}", worldSize);
            Console.WriteLine("creationTime: {0}", creationTime);
            Console.WriteLine("playTime: {0}", playTime);
            Console.WriteLine("difficulty: {0}", difficulty);
            Console.WriteLine("generator: {0}", generator);
            Console.WriteLine("timeOfDay: {0}", timeOfDay);
            Console.WriteLine("lengthOfDay: {0}", lengthOfDay);

            uint chunkIndexSize = reader.ReadUInt32();

            for(int i = 0; i < chunkIndexSize; i++)
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                uint pointer = reader.ReadUInt32();
                chunkIndex.Add(x, y, pointer);
            }
        }

        public void Close()
        {
            fsWorld.Close();
            fsChunk.Close();
        }
    }
}
