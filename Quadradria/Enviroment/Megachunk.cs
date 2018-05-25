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
    public enum UnloadState { MegachunkEmpty, MegachunkNotEmpty, MegachunkStillGenerating }

    class Megachunk
    {
        public const int SIZE = 64;

        private Chunk[,] chunks = new Chunk[SIZE, SIZE];

        private long[,] index = new long[SIZE, SIZE];

        public int WorldX, WorldY;
        private GraphicsDevice graphicsDevice;

        private int numberOfLoadedChunks = 0;
        private bool isGenerated = false;
        public bool IsClosing = false;
        public bool PleaseDontClose = false;

        private FileStream fs;
        private BinaryReader reader;
        private BinaryWriter writer;

        private int sortedCount = 0;

        private Action onUnloaded;

        private List<Task> tasksToDo = new List<Task>();

        public Megachunk(int worldX, int worldY, GraphicsDevice graphicsDevice, IGenerator generator, Action onUnloaded)
        {

            Directory.CreateDirectory(@"E:\c");

            this.WorldX = worldX;
            this.WorldY = worldY;
            this.graphicsDevice = graphicsDevice;
            this.onUnloaded = onUnloaded;
            
            string path = @"E:\c\q" + worldX + "," + worldY + ".meg";

            fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
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
                isGenerated = true;
            } else
            {
                writer.Write(new Byte[8 * SIZE * SIZE]);
                writer.BaseStream.Position = 8 * SIZE * SIZE;
                Generate(generator);
            }
        }

        private void LoadChunk(int x, int y, Chunk c = null)
        {
            //if (chunks[x, y] != null) return;

            if (c == null)
            {
                c = new Chunk(WorldX * SIZE + x, WorldY * SIZE + y, graphicsDevice);
                for (int i = 0; i < Chunk.SIZE; i++)
                {
                    for (int j = 0; j < Chunk.SIZE; j++)
                    {
                        c.SetBlockAtLocalPosition(i, j, BlockType.Wool, 0);
                    }
                }
            }

            if (chunks[x, y] == null) numberOfLoadedChunks++;
            chunks[x, y] = c;

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
                            return;
                        }
                        
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
                numberOfLoadedChunks--;
                chunks[x, y] = null;
                if (!isGenerated) return UnloadState.MegachunkStillGenerating;


                byte[] data = c.Export();

                SaveChunk(x, y, data);                

                if (numberOfLoadedChunks <= 0)
                {
                    SortFile(true);

                    return UnloadState.MegachunkEmpty;
                }
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
            if (doCloseAfterFinish)
                IsClosing = true;
            
            sortedCount++;
            Task.Run(() =>
            {
                Task[] tasks;
                lock (tasksToDo)
                {
                    tasks = tasksToDo.ToArray();
                }
                Task.WaitAll(tasks);

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
                        } else
                        {
                            IsClosing = false;
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
            IsClosing = false;
            if (PleaseDontClose) {
                Log("We had the idea to close, but some idiots they we shoudnldndlndt");
                PleaseDontClose = false;
                return;
            }

            onUnloaded?.Invoke();

            lock (fs)
            {
                reader.Dispose();
                writer.Dispose();

                fs.Close();
                fs.Dispose();
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
