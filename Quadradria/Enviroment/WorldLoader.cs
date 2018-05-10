using Microsoft.Xna.Framework.Graphics;
using Quadradria.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Enviroment
{
    class WorldLoader
    {

        /*
         * Dateien je Welt:
         * world.qwld       -Welt Daten
         *
         * World Format
         * Header:
         * 4    magic number    //ever 0x42171701
         * 4    file version
         * 4    seed
         * 128  world name  
         * 4    width           //bei 0 = infinity
         * 1    worldSize
         * 4    creationTime    //Encoded in a binary format with the C# method DAtetime.ToBinary().
         * 4    playtime        //seconds
         * 1    difficulty
         * 1    generator
         * 4    timeOfDay
         * 4    lengthOfDay
         * 4    chunkIndexSize
         * Chunk Index:
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * chunkData.cdat   -Block Daten
         * entData.edat     -Entity Daten
         * 
         * */

        private Chunk[,] AllChunks = new Chunk[100, 100];

        public void Init(GraphicsDevice graphicsDevice)
        {
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

        private string worldPath;

        private List2D<uint> chunkIndex = new List2D<uint>();

        public WorldLoader()
        {

        }

        public WorldLoader(string fileName)
        {
            this.worldPath = fileName;

            //FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            //StreamWriter writer = new StreamWriter(fs);
            //StreamReader reader = new StreamReader(fs);
        }

        public void GetChunk(int x, int y)
        {

        }

        public void WriteChunk(Chunk chunk)
        {
            uint address;
            FileStream fsChunks = new FileStream(worldPath + @"\chunks.cdat", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            FileStream fsWorld = new FileStream(worldPath + @"\world.qwld", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            BinaryWriter writerChunk = new BinaryWriter(fsChunks);
            BinaryWriter writerWorld = new BinaryWriter(fsWorld);

            if (!chunkIndex.Includes(chunk.pos.X, chunk.pos.Y))
            {
                address = (uint)fsChunks.Length;
                chunkIndex.Add(chunk.pos.X, chunk.pos.Y, address);

                writerWorld.Write(chunk.pos.X);
                writerWorld.Write(chunk.pos.Y);
                writerWorld.Write(address);
            } else {
                address = chunkIndex.Get(chunk.pos.X, chunk.pos.Y);
            }

            fsChunks.Seek(address, SeekOrigin.Begin);
            writerChunk.Write(chunk.Export());

            fsChunks.Close();
            fsWorld.Close();
        }

        public void WriteWorld(WorldInfo worldInfo)
        {
            FileStream fsWorld = new FileStream(worldPath + @"\world.qwld", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

            BinaryWriter writer = new BinaryWriter(fsWorld);
            writer.Write((uint) 0x42171701);    //Magic Number
            writer.Write((uint) 0x1);           //Version
            writer.Write((uint)worldInfo.seed);
            writer.Write(Encoding.UTF8.GetBytes(worldInfo.name.PadRight(128, '\0')));
            writer.Write((uint)worldInfo.width);
            writer.Write((byte)worldInfo.worldSize);
            writer.Write((ulong)worldInfo.creationTime);
            writer.Write((ulong)worldInfo.playTime);
            writer.Write((byte)worldInfo.difficulty);
            writer.Write((byte)worldInfo.generator);
            writer.Write((uint)worldInfo.timeOfDay);
            writer.Write((uint)worldInfo.lengthOfDay);
            writer.Write((uint)0);

            uint chunkIndexSize = (uint)chunkIndex.Length;

            chunkIndex.ForEachWrapper((chunk) => {
                writer.Write(chunk.x);
                writer.Write(chunk.y);
                writer.Write(chunk.item);
            });

            fsWorld.Close();
        }

        public void LoadWorld()
        {
            FileStream fs = new FileStream(worldPath + @"\world.qwld", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

            BinaryReader reader = new BinaryReader(fs);

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

            Console.WriteLine("magic number: {0}, {1}", magicNumber, magicNumber == 0x42171701 ? "Ja" : "Nein");
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

            fs.Close();
        }

    }
}
