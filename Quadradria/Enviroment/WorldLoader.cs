using Microsoft.Xna.Framework.Graphics;
using Quadradria.Enviroment.Generators;
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
        

        private FileStream fsChunk;
        private FileStream fsWorld;
        private BinaryReader ChunkReader;
        private BinaryWriter ChunkWriter;
        private BinaryReader WorldReader;
        private BinaryWriter WorldWriter;
        private string worldPath;
        private GraphicsDevice graphicsDevice;        

        private List2D<long?> chunkIndex = new List2D<long?>();

        private WorldInfo worldInfo;
        private IGenerator generator;

        //ToDo: try catch when instanziate
        public WorldLoader(string fileName, GraphicsDevice graphicsDevice, WorldInfo info)
        {
            this.graphicsDevice = graphicsDevice;
            this.worldPath = fileName;
            this.worldInfo = info;

            try
            {
                fsChunk = new FileStream(worldPath + @"\chunks.cdat", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                fsWorld = new FileStream(worldPath + @"\world.qwld", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                ChunkReader = new BinaryReader(fsChunk);
                ChunkWriter = new BinaryWriter(fsChunk);
                WorldReader = new BinaryReader(fsWorld);
                WorldWriter = new BinaryWriter(fsWorld);

                if (fsChunk.Length == 0)
                {
                    WriteWorld();
                } else
                {
                    LoadWorld();
                }

            } catch (Exception e)
            {
                throw new Exception("Error, creating world loader", e);
            }

            generator = new GenOverworld(info);
        }

        
        public Chunk LoadChunk(int x, int y)
        {
            Chunk c = new Chunk(x, y, graphicsDevice);
            if (chunkIndex.Includes(x, y))
            {
                ReadChunk(x, y, (bytes) => {
                    c.Import(bytes);
                    c.Load();
                });
            } else {
                generator.GenerateChunk(c);
                c.Load();
            }
            return c;
        }


        private void ReadChunk(int x, int y, Action<byte[]> callback)
        {
            long? address = chunkIndex.Get(x, y);
            if (address == null) return;

            Task.Run(() => {
                byte[] bytes;

                lock (fsChunk)
                {
                    try
                    {
                        ChunkReader.BaseStream.Seek((long)address, SeekOrigin.Begin);
                        bytes = ChunkReader.ReadBytes(1024);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error, reading chunk", e);
                    }
                }
                callback(bytes);
            });
        }

        public void WriteChunk(Chunk chunk)
        {
            int x = chunk.pos.X;
            int y = chunk.pos.Y;

            byte[] export = chunk.Export();
            if (export == null) return;
            
            /*
            bool write = false;
            if (!chunkIndex.Includes(x, y))
            {
                address = fsChunk.Length;
                chunkIndex.Add(x, y, address);
                write = true;
            }
            else
            {
                address = chunkIndex.Get(chunk.pos.X, chunk.pos.Y);
            }
            */


            Task.Run(() => {
                lock (fsWorld) lock (fsChunk)
                {
                    try
                    {
                        long? address;

                        if (!chunkIndex.Includes(x, y))
                        {
                            address = fsChunk.Length;
                            chunkIndex.Add(x, y, address);

                            WorldWriter.BaseStream.Seek(0, SeekOrigin.End);
                            WorldWriter.Write(x);
                            WorldWriter.Write(y);
                            WorldWriter.Write((long)address);
                        }
                        else
                        {
                            address = chunkIndex.Get(chunk.pos.X, chunk.pos.Y);
                        }

                        ChunkWriter.BaseStream.Seek((long)address, SeekOrigin.Begin);
                        ChunkWriter.Write(export);
                        WorldWriter.Seek(0xAD, SeekOrigin.Begin);
                        WorldWriter.Write((uint)chunkIndex.Length);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error saving chunk", e);
                    }
                }
            });
        }

        public void WriteWorld()
        {
            uint indexLength = (uint)chunkIndex.Length;

            Task.Run(() => {
                lock (fsWorld)
                {
                    try
                    {
                        //potential issue with using worldInfo without locking
                        WorldWriter.Seek(0, SeekOrigin.Begin);
                        WorldWriter.Write((uint)0x42171701);    //Magic Number
                        WorldWriter.Write((uint)0x1);           //Version
                        WorldWriter.Write((long)worldInfo.seed);
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

                        chunkIndex.ForEachWrapper((chunkW) => {
                            WorldWriter.Write((int)chunkW.x);
                            WorldWriter.Write((int)chunkW.y);
                            WorldWriter.Write((long)chunkW.item);
                        });
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error saving world", e);
                    }
                }
            });
        }

        public void LoadWorld()
        {
            WorldInfo Info = worldInfo;

            Task.Run(() =>
            {
                lock (fsWorld)
                {
                    try
                    {

                        fsWorld.Seek(0, SeekOrigin.Begin);

                        uint magicNumber = WorldReader.ReadUInt32();
                        uint version = WorldReader.ReadUInt32();
                        long seed = WorldReader.ReadInt64();

                        char[] worldChars = WorldReader.ReadChars(128);

                        string worldName = "";
                        for (int i = 0; i < 128; i++)
                        {
                            if (worldChars[i] == 0) break;
                            worldName += worldChars[i];
                        }

                        uint width = WorldReader.ReadUInt32();
                        WorldSize worldSize = (WorldSize)WorldReader.ReadByte();
                        ulong creationTime = WorldReader.ReadUInt64();
                        ulong playTime = WorldReader.ReadUInt64();
                        Difficulty difficulty = (Difficulty)WorldReader.ReadByte();
                        Generator generator = (Generator)WorldReader.ReadByte();
                        uint timeOfDay = WorldReader.ReadUInt32();
                        uint lengthOfDay = WorldReader.ReadUInt32();

                        Console.WriteLine("magic number: {0}, {1}", magicNumber, magicNumber == 0x42171701 ? "Matches." : "No Match!");
                        Console.WriteLine("version: {0}", version.ToString("X"));
                        Console.WriteLine("seed: {0}", seed.ToString("X"));
                        Console.WriteLine("worldName: {0}", worldName);
                        Console.WriteLine("width: {0}", width.ToString("X"));
                        Console.WriteLine("worldSize: {0}", worldSize.ToString("X"));
                        Console.WriteLine("creationTime: {0}", creationTime.ToString("X"));
                        Console.WriteLine("playTime: {0}", playTime.ToString("X"));
                        Console.WriteLine("difficulty: {0}", difficulty.ToString("X"));
                        Console.WriteLine("generator: {0}", generator.ToString("X"));
                        Console.WriteLine("timeOfDay: {0}", timeOfDay.ToString("X"));
                        Console.WriteLine("lengthOfDay: {0}", lengthOfDay.ToString("X"));

                        Info.seed = seed;
                        Info.width = width;
                        Info.timeOfDay = timeOfDay;
                        Info.lengthOfDay = lengthOfDay;
                        Info.Name = worldName;
                        Info.difficulty = difficulty;
                        Info.generator = generator;
                        Info.Size = worldSize;
                        Info.creationTime = creationTime;
                        Info.playTime = playTime;

                        uint chunkIndexSize = WorldReader.ReadUInt32();
                        Console.WriteLine("chunkIndexSize: {0}", chunkIndexSize);
                        
                        
                        for (int i = 0; i < chunkIndexSize; i++)
                        {
                            int x = WorldReader.ReadInt32();
                            int y = WorldReader.ReadInt32();
                            long pointer = WorldReader.ReadInt64();
                            chunkIndex.Add(x, y, pointer);
                        }
                        
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error reading world", e);
                    }
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
