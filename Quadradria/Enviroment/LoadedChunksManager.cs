﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Quadradria.Utils;

namespace Quadradria.Enviroment
{
    class LoadedChunksManager
    {

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

                Chunks.Add(new ChunkWrapper(y, chunk));
                return true;
            }

            public bool RemoveChunk(int y)
            {
                for (int i = 0; i < Chunks.Count; i++)
                {
                    if (Chunks[i].y == y)
                    {
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

        public Rect lastRect = new Rect(0, 0, 0, 0);


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
                        if (!ChunkColoumns[i].IsEmpty())
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
            if (newrect == lastRect) return;

            //remove old chunks
            for (int i = lastRect.Y; i <= lastRect.Y + lastRect.Height; i++)
            {
                for (int j = lastRect.X; j <= lastRect.X + lastRect.Width; j++)
                {
                    //if (j >= newrect.x && j <= (newrect.x + newrect.width) && i >= newrect.y && i <= (newrect.y + newrect.height)) continue;
                    RemoveChunk(j, i);
                }
            }

            //add new chunks
            for (int i = newrect.Y; i <= newrect.Y + newrect.Height; i++)
            {
                for (int j = newrect.X; j <= newrect.X + newrect.Width; j++)
                {
                    //if ((j >= lastRect.x && j <= (lastRect.x + lastRect.width)) && (i >= lastRect.y && i <= (lastRect.y + lastRect.height))) continue;
                    Chunk c = chunkLoader.loadChunk(j, i);
                    if (c != null) AddChunk(j, i, c);
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

    }

}