using Microsoft.Xna.Framework.Graphics;
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
        public const int SIZE = 64;

        private Chunk[,] chunks = new Chunk[SIZE, SIZE];

        private long[,] index = new long[SIZE, SIZE];

        public int WorldX, WorldY;
        private GraphicsDevice graphicsDevice;

        private int numberOfLoadedChunks = 0;
        private bool isGenerated = false;

        private FileStream fs;
        private BinaryReader reader;
        private BinaryWriter writer;

        private int sortedCount = 0;

        private List<Task> tasksToDo = new List<Task>();

        public Megachunk(int worldX, int worldY, GraphicsDevice graphicsDevice, IGenerator generator)
        {

            Directory.CreateDirectory(@"E:\c");

            this.WorldX = worldX;
            this.WorldY = worldY;
            this.graphicsDevice = graphicsDevice;
            
            Log("Opened");
            string path = @"E:\c\q" + worldX + "," + worldY + ".meg";

            fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            reader = new BinaryReader(fs);
            writer = new BinaryWriter(fs);

            if (fs.Length != 0)
            {
                Log("Megachunk is already saved => Importing...");
                for (int y = 0; y < SIZE; y++)
                {
                    for (int x = 0; x < SIZE; x++)
                    {
                        index[x, y] = reader.ReadInt64();
                        if (index[x, y] == 0)
                        {
                            Log("There is a null pointer in the index!");
                        }
                    }
                }
                isGenerated = true;
            } else
            {
                Log("Megachunk is new => Generating...");
                writer.Write(new Byte[8 * SIZE * SIZE]);
                writer.BaseStream.Position = 8 * SIZE * SIZE;
                Generate(generator);
            }
        }

        private void LoadChunk(int x, int y, Chunk c = null)
        {
            if (c == null)
            {
                c = new Chunk(WorldX * SIZE + x, WorldY * SIZE + y, graphicsDevice);
                /*for (int i = 0; i < Chunk.SIZE; i++)
                {
                    for (int j = 0; j < Chunk.SIZE; j++)
                    {
                        c.SetBlockAtLocalPosition(i, j, BlockType.Wool, 0);
                    }
                }*/
            }

            chunks[x, y] = c;
            numberOfLoadedChunks++;

            if (!isGenerated) return;

            Task.Run(() => {

                lock (fs)
                {
                    long address, dataLength;
                    byte[] data;

                    try
                    {
                        address = index[x, y];
                        if (address < (SIZE * SIZE * 8)) {
                            if (address == 0)
                            {
                                //Log("Adress is zero for chunk " + (x+WorldX*SIZE) + ", " + (y+WorldY*SIZE));
                            } else
                            {
                                Log("File Corrupted: Chunk address is in the index!");
                            }
                            return;
                        };
                        
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
        }

        public void AddChunk(Chunk chunk)
        {

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
                chunks[x, y] = null;
                numberOfLoadedChunks--;


                SaveChunk(x, y, data);

                if (numberOfLoadedChunks <= 0)
                {
                    Log("All Chunks are unloaded. Sorting file and close...");

                    SortFile(true);

                    return UnloadState.MegachunkEmpty;
                }
            } else
            {
                Log("Chunk does not exists! " + x + (WorldX * SIZE) + ", " + (y + WorldY * SIZE));
            }
            return UnloadState.MegachunkNotEmpty;
        }

        public Task SaveChunk(int x, int y, byte[] data)
        {
            //Log("Saving a chunk");
            Task task = null;
            task = Task.Run(() =>
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
                lock(tasksToDo)
                    tasksToDo.Remove(task);
            });
            lock(tasksToDo)
                tasksToDo.Add(task);

            return task;
        } 

        public void SortFile(bool doCloseAfterFinish = false)
        {
            //Log("Sorting that file");
            sortedCount++;
            Task.Run(() =>
            {
                Task[] tasks;
                lock (tasksToDo)
                {
                    if (tasksToDo.Contains(null))
                    {
                        Log("Tasks to do contains null");
                    }

                    tasks = tasksToDo.ToArray();
                }
                Task.WaitAll(tasks);
                //Log("Waited for saving tasks to complete");

                lock (fs)
                {
                    using (MemoryStream outStream = new MemoryStream())
                    using (BinaryWriter outWriter = new BinaryWriter(outStream))
                    {
                        //Make space for the index
                        outWriter.Write(new Byte[8 * SIZE * SIZE]);
                        outStream.Seek(8 * SIZE * SIZE, SeekOrigin.Begin);
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

                        if (doCloseAfterFinish)
                        {
                            CloseEverything();
                        }
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
            Task generatorTask = null;
            generatorTask = generator.GenerateMegachunk(this, graphicsDevice, () => {
                Log("Generation complete!");
                isGenerated = true;
                for (int y = 0; y < SIZE; y++)
                {
                    for (int x = 0; x < SIZE; x++)
                    {
                        if(chunks[x, y] != null)
                        {
                            LoadChunk(x, y, chunks[x, y]);
                        }
                    }
                }
                tasksToDo.Remove(generatorTask);
            });

            tasksToDo.Add(generatorTask);
        }

        private void CloseEverything()
        {
            lock (fs)
            {
                reader.Dispose();
                writer.Dispose();

                fs.Close();
                fs.Dispose();
                Log("Closed");
            }
        }

        public override string ToString()
        {
            return "Megachunk:" + WorldX + "," + WorldY;
        }

        private void Log(string message)
        {
            Console.WriteLine("[WorldLoaded][" + ToString() + "]" + message);
        }
    }
}
