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
         *qx,y.meg
         * 
         * entData.edat         //Entity Data
         * 
         * */
        
        private FileStream fsWorld;
        private BinaryReader WorldReader;
        private BinaryWriter WorldWriter;
        private string worldPath;
        private GraphicsDevice graphicsDevice;        

        private List2D<long?> chunkIndex = new List2D<long?>();

        private List2D<Megachunk> megachunks = new List2D<Megachunk>();

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
                fsWorld = new FileStream(worldPath + @"\world.qwld", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                WorldReader = new BinaryReader(fsWorld);
                WorldWriter = new BinaryWriter(fsWorld);

                if (fsWorld.Length == 0)
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


        public Megachunk LoadMegachunk(int x, int y)
        {
            Megachunk mc = new Megachunk(x, y, graphicsDevice, generator);
            megachunks.Add(x, y, mc);
            return mc;
        }

        public Chunk LoadChunk(int x, int y)
        {
            //Get the megachunk coordinates
            int mx = (int)Math.Floor(x / (float)Megachunk.SIZE);
            int my = (int)Math.Floor(y / (float)Megachunk.SIZE);


            //Get the megachunk if in memory
            Megachunk mc = megachunks.Get(mx, my);
            if (mc == null)
            {
                mc = LoadMegachunk(mx, my);
            }

            //Get the chunk from within the megachunk
            Chunk c = mc.GetChunk(Tools.Mod(x, Megachunk.SIZE), Tools.Mod(y, Megachunk.SIZE));

            return c;
        }

        public void Unload(Chunk chunk)
        {
            int mx = (int)Math.Floor(chunk.pos.X / (float)Megachunk.SIZE);
            int my = (int)Math.Floor(chunk.pos.Y / (float)Megachunk.SIZE);

            Megachunk mc = megachunks.Get(mx, my);

            UnloadState state = mc.UnloadChunk(Tools.Mod(chunk.pos.X, Megachunk.SIZE), Tools.Mod(chunk.pos.Y, Megachunk.SIZE));
            if (state == UnloadState.MegachunkEmpty)
            {
                megachunks.Remove(mx, my);
            }

        }

        public void WriteWorld(bool lastsave = false)
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

                        if (lastsave) Unload();
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

        public int GetNumberOfLoadedMegachunks()
        {
            return megachunks.Length;
        }

        public void Unload()
        {
            megachunks.ForEach((mc) =>
            {
                mc.PutThatDamDataOutToTheDrive();
            });
            fsWorld.Close();
        }
    }
}
