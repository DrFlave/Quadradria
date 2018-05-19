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
        private Rect lastDrawRect;
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
            if (lastRect != null)
            {
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
                            RemoveChunk(j, i);

                            if (!newDrawRect.Contains(j, i) && lastDrawRect.Contains(j, i))
                            {
                                ChunksVisible.Remove(j, i);
                            }
                        }
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
                        worldLoader.LoadChunk(j, i, (chunk) => {
                            if (chunk == null) return;
                            AddChunk(j, i, chunk);

                            if (lastDrawRect == null || !lastDrawRect.Contains(j, i))
                            {
                                ChunksVisible.Add(j, i, chunk);
                            }
                        });
                    }
                }
            }

            lastRect = newrect;
            lastDrawRect = newDrawRect;
        }
        
        private List2D<Chunk> ChunksLoaded = new List2D<Chunk>();
        private List2D<Chunk> ChunksVisible = new List2D<Chunk>();

        public void GetChunk(int x, int y, Action<Chunk> callback)
        {
            Chunk c = ChunksLoaded.Get(x, y);
            if (c != null) {
                callback(c);
                return;
            }

            worldLoader.LoadChunk(x, y, callback);
        }

        public bool AddChunk(int x, int y, Chunk chunk)
        {
            return ChunksLoaded.Add(x, y, chunk);
        }

        public bool RemoveChunk(int x, int y)
        {
            return ChunksLoaded.Remove(x, y);
        }

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
