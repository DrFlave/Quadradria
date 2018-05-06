using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Quadradria.Utils;

namespace Quadradria.Enviroment
{
    class LoadedChunksManager
    {

        public Rect lastRect;

        private List<ChunkColoumn> ChunkColoumns = new List<ChunkColoumn>();
        
        private ChunkColoumn GetChunkColoumn(int x)
        {
            for (int i = 0; i < ChunkColoumns.Count; i++)
            {
                if (ChunkColoumns[i].x == x)
                {
                    return ChunkColoumns[i];
                }
            }
            return null;
        }



        public Chunk GetChunk(int x, int y)
        {
            ChunkColoumn cc = GetChunkColoumn(x);
            if (cc == null) return null;

            return cc.GetChunk(y);
        }

        public bool AddChunk(int x, int y, Chunk chunk)
        {
            if (GetChunk(x, y) != null) return false;

            ChunkColoumn cc = GetChunkColoumn(x);
            if (cc == null)
            {
                cc = new ChunkColoumn(x);
                ChunkColoumns.Add(cc);
            }

            cc.AddChunk(y, chunk);
            return true;
        }

        public bool RemoveChunk(int x, int y)
        {
            for (int i = 0; i < ChunkColoumns.Count; i++)
            {
                if (ChunkColoumns[i].x == x)
                {
                    if (ChunkColoumns[i].RemoveChunk(y))
                    {
                        if (ChunkColoumns[i].IsEmpty())
                        {
                            ChunkColoumns.RemoveAt(i);
                        }
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }


        public void UpdateLoadedArea(int x, int y, int width, int height, ChunkLoader chunkLoader)
        {
            Rect newrect = new Rect(x, y, width, height);
            if (newrect.Equals(lastRect))  return;

            //remove old chunks
            if (lastRect != null)
            {
                for (int i = lastRect.Y; i < lastRect.Y + lastRect.Height; i++)
                {
                    for (int j = lastRect.X; j < lastRect.X + lastRect.Width; j++)
                    {
                        if (!newrect.Contains(j, i))
                            RemoveChunk(j, i);
                    }
                }
            }

            //add new chunks
            
            for (int i = newrect.Y; i < newrect.Y + newrect.Height; i++)
            {
                for (int j = newrect.X; j < newrect.X + newrect.Width; j++)
                {
                    if (lastRect == null || !lastRect.Contains(j, i))
                    {
                        Chunk c = chunkLoader.loadChunk(j, i);
                        if (c != null)
                        {
                            AddChunk(j, i, c);
                        }
                    }
                }
            }

            lastRect = newrect;
        }

        public void ForEach(Action<Chunk> func)
        {
            for (int i = 0; i < ChunkColoumns.Count; i++)
            {
                ChunkColoumns[i].ForEach(func);
            }

        }


        private class ChunkColoumn
        {
            public int x;

            private List<ChunkWrapper> Chunks = new List<ChunkWrapper>();

            public ChunkColoumn(int x)
            {
                this.x = x;
            }

            public Chunk GetChunk(int y)
            {
                for (int i = 0; i < Chunks.Count; i++)
                {
                    if (Chunks[i].y == y)
                    {
                        return Chunks[i].chunk;
                    }
                }
                return null;
            }

            public bool AddChunk(int y, Chunk chunk)
            {
                if (GetChunk(y) != null) return false;

                //chunk.Load();
                Chunks.Add(new ChunkWrapper(y, chunk));
                return true;
            }

            public bool RemoveChunk(int y)
            {
                for (int i = 0; i < Chunks.Count; i++)
                {
                    if (Chunks[i].y == y)
                    {
                        //Chunks[i].chunk.Unload();
                        Chunks.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }

            public bool IsEmpty()
            {
                return !Chunks.Any();
            }

            public void ForEach(Action<Chunk> func)
            {
                for (int i = 0; i < Chunks.Count; i++)
                {
                    func(Chunks[i].chunk);
                }
            }

            private struct ChunkWrapper
            {
                public int y;
                public Chunk chunk;

                public ChunkWrapper(int y, Chunk chunk)
                {
                    this.y = y;
                    this.chunk = chunk;
                }
            }
        }


    }

}
