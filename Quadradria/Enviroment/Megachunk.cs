﻿using Microsoft.Xna.Framework.Graphics;
using Quadradria.Entity;
using Quadradria.Enviroment.Generators;
using Quadradria.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Enviroment
{
    public enum UnloadState { MegachunkEmpty, MegachunkNotEmpty }

    class Megachunk
    {
        public const int SIZE = 32;

        private Chunk[,] chunks = new Chunk[SIZE, SIZE];

        private long[,] index = new long[SIZE, SIZE];

        private int worldX, worldY;
        private GraphicsDevice graphicsDevice;

        private int numberOfLoadedChunks = 0;

        private FileStream fs;
        private BinaryReader reader;
        private BinaryWriter writer;

        private bool isAlreadySaved = false;

        public Megachunk(int worldX, int worldY, GraphicsDevice graphicsDevice, IGenerator generator)
        {
            Console.WriteLine("Opening Megachunk {0}, {1}", worldX, worldY);

            Directory.CreateDirectory(@"E:\c");

            this.worldX = worldX;
            this.worldY = worldY;
            this.graphicsDevice = graphicsDevice;
            
            string path = @"E:\c\q" + worldX + "," + worldY + ".meg";

            fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            reader = new BinaryReader(fs);
            writer = new BinaryWriter(fs);

            if (fs.Length != 0)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    for (int x = 0; x < SIZE; x++)
                    {
                        index[x, y] = reader.ReadInt64();
                    }
                }
            } else
            {
                writer.Write(new Byte[8 * SIZE * SIZE]);
                Generate(generator);
            }
        }

        private void LoadChunk(int x, int y)
        {
            Chunk c = new Chunk(worldX * SIZE + x, worldY * SIZE + y, graphicsDevice);
            chunks[x, y] = c;
            Task.Run(() => {
                lock (fs)
                {
                    long address, dataLength;
                    byte[] data;

                    try
                    {
                        address = index[x, y];
                        fs.Seek(address, SeekOrigin.Begin);

                        dataLength = reader.ReadInt64();
                        data = reader.ReadBytes((int)dataLength);

                        c.Import(data);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Was anderes", e);
                    }
                }
            });

            numberOfLoadedChunks++;
        }

        public Chunk GetChunk(int x, int y)
        {
            if (chunks[x, y] == null) LoadChunk(x, y);
            return chunks[x, y];
        }

        public UnloadState UnloadChunk(int x, int y)
        {
            Chunk c = chunks[x, y];
            if (c != null)
            {
                byte[] data = c.Export();
                SaveChunk(x, y, data);
                chunks[x, y] = null;

                numberOfLoadedChunks--;
                if(numberOfLoadedChunks <= 0) 
                {
                    PutThatDamDataOutToTheDrive();
                    return UnloadState.MegachunkEmpty;
                }
            } else
            {
                Console.WriteLine("Chunk does not exists! {0}, {1}", x + worldX * SIZE, y + worldY * SIZE);
            }
            return UnloadState.MegachunkNotEmpty;
        }

        private void SaveChunk(int x, int y, byte[] data)
        {
            Task.Run(() =>
            {
                lock (fs)
                {
                    fs.Seek(0, SeekOrigin.End);
                    long address = fs.Position;
                    writer.Write((long)data.Length);
                    writer.Write(data);
                    index[x, y] = address;
                    writer.Seek((y * SIZE + x) * 8, SeekOrigin.Begin);
                    writer.Write(address);
                }
            });
        } 

        public void PutThatDamDataOutToTheDrive()
        {
            if (isAlreadySaved) return;
            isAlreadySaved = true;

            Task.Run(() =>
            {
                lock (fs)
                {
                    using (MemoryStream outStream = new MemoryStream())
                    using (BinaryWriter outWriter = new BinaryWriter(outStream))
                    {
                        //Make space for the index
                        outWriter.Write(new Byte[8 * SIZE * SIZE]);
                        outStream.Seek(8 * SIZE * SIZE, SeekOrigin.Begin);
                        Console.WriteLine("A {0}, {1}", worldX, worldY);
                        for (int y = 0; y < SIZE; y++)
                        {
                            for (int x = 0; x < SIZE; x++)
                            {
                                long address = index[x, y];
                                fs.Seek(address, SeekOrigin.Begin);

                                long dataLength = reader.ReadInt64();
                                byte[] data = reader.ReadBytes((int)dataLength);


                                outStream.Seek(0, SeekOrigin.End);
                                long addressNew = outStream.Position;
                                outWriter.Write((long)data.Length);
                                outWriter.Write(data);
                                outStream.Seek(y * 8 * SIZE + 8 * x, SeekOrigin.Begin);
                                outWriter.Write(addressNew);
                            }
                        }
                        
                        fs.Seek(0, SeekOrigin.Begin);
                        fs.SetLength(0);
                        writer.Write(outStream.ToArray());

                        CloseEverythingA();
                    }

                }
            });
        }

        private void ImportChunk(int x, int y, byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int length = reader.ReadInt32();
                byte[] chunkData = reader.ReadBytes(length);
                
                chunks[x, y].Import(chunkData);
            }
        }

        public byte[] Export(World world)
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {

                for (int y = 0; y < SIZE; y++)
                {
                    for (int x = 0; x < SIZE; x++)
                    {
                        Chunk c = chunks[x, y];
                        byte[] chunkData = c.Export();

                        writer.Write((int)chunkData.Length);
                        writer.Write(chunkData);
                    }
                }

                return stream.ToArray();
            }
        }

        public void Generate(IGenerator generator)
        {
            for (int y = 0; y < SIZE; y++)
            {
                for (int x = 0; x < SIZE; x++)
                {
                    chunks[x, y] = new Chunk(worldX * SIZE + x, worldY * SIZE + y, graphicsDevice);
                    generator.GenerateChunk(chunks[x, y]);
                    SaveChunk(x, y, chunks[x, y].Export());
                    chunks[x, y] = null;
                }
            }
        }

        private void CloseEverythingA()
        {
            lock (fs)
            {
                Console.WriteLine("Close Megachunk {0}, {1}", worldX, worldY);
                reader.Dispose();
                writer.Dispose();

                fs.Close();
                fs.Dispose();
                Console.WriteLine("B {0}, {1}", worldX, worldY);
            }
        }
    }
}
