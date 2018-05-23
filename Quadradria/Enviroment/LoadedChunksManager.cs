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

        private Rect lastRect = new Rect(0, 0, 0, 0);
        private Rect lastDrawRect = new Rect(0, 0, 0, 0);
        private WorldLoader worldLoader;

        public LoadedChunksManager(WorldLoader worldLoader)
        {
            this.worldLoader = worldLoader;
        }

        public void UpdateLoadedArea(int x, int y, int width, int height, WorldLoader worldLoader)
        {
            Rect newrect = new Rect(x-3, y-3, width+6, height+6);
            if (newrect.Equals(lastRect)) return;
            Rect newDrawRect = new Rect(x, y, width, height);

            //remove old chunks
            for (int i = lastRect.Y; i < lastRect.Y + lastRect.Height; i++)
            {
                for (int j = lastRect.X; j < lastRect.X + lastRect.Width; j++)
                {
                    if (!newrect.Contains(j, i))
                    {
                        Chunk chunk = ChunksLoaded.Get(j, i);
                        if (chunk == null) continue;

                        worldLoader.WriteChunk(chunk);
                        chunk.Unload();
                        ChunksLoaded.Remove(j, i);
                    }
                    if (!newDrawRect.Contains(j, i) && lastDrawRect.Contains(j, i))
                    {
                        ChunksVisible.Remove(j, i);
                    }
                }
            }


            //add new chunks
            for (int i = newrect.Y; i < newrect.Y + newrect.Height; i++)
            {
                for (int j = newrect.X; j < newrect.X + newrect.Width; j++)
                {
                    if (!lastRect.Contains(j, i))
                    {
                        Chunk chunk = worldLoader.LoadChunk(j, i);
                        if (chunk == null) continue;
                        ChunksLoaded.Add(j, i, chunk);
                    }

                    if (newDrawRect.Contains(j, i))
                    {
                        Chunk chunk = ChunksLoaded.Get(j, i);
                        if (chunk == null) continue;
                        ChunksVisible.Add(j, i, chunk);
                    }

                }
            }

            lastRect = newrect;
            lastDrawRect = newDrawRect;
        }
        
        private List2D<Chunk> ChunksLoaded = new List2D<Chunk>();
        private List2D<Chunk> ChunksVisible = new List2D<Chunk>();

        public int GetLoadedChunkNumber()
        {
            return ChunksLoaded.Length;
        }

        public int GetVisibleChunkNumber()
        {
            return ChunksVisible.Length;
        }

        
        public Chunk GetChunk(int x, int y)
        {
            return ChunksLoaded.Get(x, y);
        }
        

        /*
        public bool AddChunk(int x, int y, Chunk chunk)
        {
            return ChunksLoaded.Add(x, y, chunk);
        }

        public bool RemoveChunk(int x, int y)
        {
            return ChunksLoaded.Remove(x, y);
        }
        */

        public void ForEachLoaded(Action<Chunk> func)
        {
            ChunksLoaded.ForEach(func);
        }

        public void ForEachVisible(Action<Chunk> func)
        {
            ChunksVisible.ForEach(func);
        }

    }

}
