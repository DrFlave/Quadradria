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

        //DEBUG!!!!
        private Chunk[,] AllChunks = new Chunk[100, 100];

        //DEBUG!!!!
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

        //DEBUG!!!!
        public void LoadChunk(int x, int y, Action<Chunk> callback)
        {
            if (x < 0 || y < 0 || x > 99 || y > 99) { 
                callback(null);
                return;
            }
            callback(AllChunks[x, y]);
        }

        private FileStream fsChunk;
        private FileStream fsWorld;
        private BinaryReader ChunkReader;
        private BinaryWriter ChunkWriter;
        private BinaryReader WorldReader;
        private BinaryWriter WorldWriter;
        private string worldPath;
        private GraphicsDevice graphicsDevice;

        private List2D<long?> chunkIndex = new List2D<long?>();

        //ToDo: try catch when instanziate
        public WorldLoader(string fileName)
        {
            this.worldPath = fileName;

            try
            {
                fsChunk = new FileStream(worldPath + @"\chunks.cdat", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                fsWorld = new FileStream(worldPath + @"\world.qwld", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                ChunkReader = new BinaryReader(fsChunk);
                ChunkWriter = new BinaryWriter(fsChunk);
                WorldReader = new BinaryReader(fsWorld);
                WorldWriter = new BinaryWriter(fsWorld);
            } catch (Exception e)
            {
                throw new Exception("Error, creating world loader", e);
            }
        }

        /*
        public void LoadChunk(int x, int y, Action<Chunk> callback)
        {
            if (chunkIndex.Includes(x, y))
            {
                ReadChunk(x, y, (bytes)=> {
                    Chunk c = new Chunk(x, y, graphicsDevice);
                    c.Import(bytes);
                    callback(c);
                });
            } else {
                callback(new Chunk(x, y, graphicsDevice));
                //ToDo: Woldgenerator here!
                //ToDo: (Maybe) save chunk after generating
            }
        }
        */

        private void ReadChunk(int x, int y, Action<byte[]> callback)
        {
            long? address = chunkIndex.Get(x, y);
            if (address == null) return;

            Task.Run(() => {
                byte[] bytes;
                lock (fsChunk)
                {
                    ChunkReader.BaseStream.Seek((long)address, SeekOrigin.Begin);
                    bytes = ChunkReader.ReadBytes(1024);
                }
                callback(bytes);
            });
        }

        public void WriteChunk(Chunk chunk)
        {
            long? address;
            bool write = false;

            if (!chunkIndex.Includes(chunk.pos.X, chunk.pos.Y))
            {
                address = fsChunk.Length;
                chunkIndex.Add(chunk.pos.X, chunk.pos.Y, address);
                write = true;
            }
            else
            {
                address = chunkIndex.Get(chunk.pos.X, chunk.pos.Y);
            }

            int x = chunk.pos.X;
            int y = chunk.pos.Y;
            byte[] export = chunk.Export();

            Task.Run(() => {
                lock (fsWorld) lock (fsChunk)
                {
                    if (write)
                    {
                        WorldWriter.BaseStream.Seek(WorldWriter.BaseStream.Length, SeekOrigin.Begin);
                        //WorldWriter.BaseStream.Seek(0, SeekOrigin.End);
                        WorldWriter.Write(x);
                        WorldWriter.Write(y);
                        WorldWriter.Write((long)address);
                    }

                    ChunkWriter.BaseStream.Seek((long)address, SeekOrigin.Begin);
                    ChunkWriter.Write(export);
                    WorldWriter.Seek(0xAB, SeekOrigin.Begin);
                    WorldWriter.Write((uint)chunkIndex.Length);
                }
            });
        }

        public void WriteWorld(WorldInfo worldInfo)
        {
            uint indexLength = (uint)chunkIndex.Length;

            Task.Run(() => {
                lock (fsWorld)
                {
                    //potential issue with using worldInfo without locking
                    WorldWriter.Seek(0, SeekOrigin.Begin);
                    WorldWriter.Write((uint)0x42171701);    //Magic Number
                    WorldWriter.Write((uint)0x1);           //Version
                    WorldWriter.Write((uint)worldInfo.seed);
                    WorldWriter.Write(Encoding.UTF8.GetBytes(worldInfo.Name.PadRight(128, '\0')));
                    WorldWriter.Write((uint)worldInfo.width);
                    WorldWriter.Write((byte)worldInfo.Size);
                    WorldWriter.Write((ulong)worldInfo.creationTime);
                    WorldWriter.Write((ulong)worldInfo.playTime);
                    WorldWriter.Write((byte)worldInfo.difficulty);
                    WorldWriter.Write((byte)worldInfo.generator);
                    WorldWriter.Write((uint)worldInfo.timeOfDay);
                    WorldWriter.Write((uint)worldInfo.lengthOfDay);
                    WorldWriter.Write((uint)indexLength);

                    uint chunkIndexSize = (uint)indexLength;

                    chunkIndex.ForEachWrapper((chunkW) => {
                        WorldWriter.Write((int)chunkW.x);
                        WorldWriter.Write((int)chunkW.y);
                        WorldWriter.Write((long)chunkW.item);
                    });
                }
            });
        }

        public void LoadWorld()
        {
            Task.Run(() =>
            {
                lock (fsWorld)
                {

                    fsWorld.Seek(0, SeekOrigin.Begin);

                    uint magicNumber = WorldReader.ReadUInt32();
                    uint version = WorldReader.ReadUInt32();
                    uint seed = WorldReader.ReadUInt32();

                    char[] worldChars = WorldReader.ReadChars(128);

                    string worldName = "";
                    for (int i = 0; i < 128; i++)
                    {
                        if (worldChars[i] == 0) break;
                        worldName += worldChars[i];
                    }

                    uint width = WorldReader.ReadUInt32();
                    byte worldSize = WorldReader.ReadByte();
                    ulong creationTime = WorldReader.ReadUInt32();
                    ulong playTime = WorldReader.ReadUInt32();
                    byte difficulty = WorldReader.ReadByte();
                    byte generator = WorldReader.ReadByte();
                    uint timeOfDay = WorldReader.ReadUInt32();
                    uint lengthOfDay = WorldReader.ReadUInt32();

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

                    uint chunkIndexSize = WorldReader.ReadUInt32();

                    WorldReader.ReadUInt32();
                    /*
                    for (int i = 0; i < chunkIndexSize; i++)
                    {
                        int x = WorldReader.ReadInt32();
                        int y = WorldReader.ReadInt32();
                        long pointer = WorldReader.ReadInt64();
                        WorldReader.ReadInt32();
                        chunkIndex.Add(x, y, pointer);
                    }
                    */
                }
            });

        }

        public void Close()
        {
            lock (fsWorld)
            {
                fsWorld.Close();
            }
            lock (fsChunk) {
                fsChunk.Close(); 
            }
        }
        

    }
}
