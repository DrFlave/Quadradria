using Quadradria.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Enviroment
{
    class LoadedChunksManager
    {

        private Rect lastRect;

        public void UpdateLoadedArea(int x, int y, int width, int height, ChunkLoader chunkLoader)
        {
            Rect newrect = new Rect(x, y, width, height);
            if (newrect.Equals(lastRect)) return;

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
        
        private List2D<Chunk> Chunks = new List2D<Chunk>();

        public Chunk GetChunk(int x, int y)
        {
            return Chunks.Get(x, y);
        }

        public bool AddChunk(int x, int y, Chunk chunk)
        {
            return Chunks.Add(x, y, chunk);
        }

        public bool RemoveChunk(int x, int y)
        {
            return Chunks.Remove(x, y);
        }

        public void ForEach(Action<Chunk> func)
        {
            Chunks.ForEach(func);
        }
    }

}
