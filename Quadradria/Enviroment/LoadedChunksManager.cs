using Microsoft.Xna.Framework.Graphics;
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

        private List2D<Chunk> ChunksLoaded = new List2D<Chunk>();
        private List2D<Chunk> ChunksVisible = new List2D<Chunk>();

        public LoadedChunksManager(WorldLoader worldLoader)
        {
            this.worldLoader = worldLoader;
        }

        public void UnloadChunk(Chunk c)
        {
            worldLoader.UnloadChunk(c);
            c.Unload();
            ChunksLoaded.Remove(c.pos.X, c.pos.Y);
        }

        public void UpdateLoadedArea(int x, int y, int width, int height, WorldLoader worldLoader)
        {
            Rect newrect = new Rect(x-3, y-3, width+6, height+6);
            if (newrect.Equals(lastRect)) return;
            Rect newDrawRect = new Rect(x, y, width, height);

            ChunksLoaded.ForEachWrapper((cw) => {
                int j = cw.x;
                int i = cw.y;
                
                if (!newrect.Contains(j, i))
                {
                    Chunk chunk = ChunksLoaded.Get(j, i);
                    if (chunk == null) return;
                    UnloadChunk(chunk);
                }
                
            });

            ChunksVisible.ForEachWrapper((cw) => {
                int j = cw.x;
                int i = cw.y;

                if (!newDrawRect.Contains(j, i))
                {
                    ChunksVisible.Remove(j, i);
                }
            });
            

            for (int i = newrect.Y; i < newrect.Y + newrect.Height; i++)
            {
                for (int j = newrect.X; j < newrect.X + newrect.Width; j++)
                {       
                    
                    if(!ChunksLoaded.Includes(j, i))
                    {
                        Chunk chunk = worldLoader.LoadChunk(j, i);
                        if (chunk == null) continue;
                        chunk.Load();
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
        
        public void Unload()
        {
            ForEachLoaded((chunk) =>
            {
                UnloadChunk(chunk);
            });
        }

        public void ForEachLoaded(Action<Chunk> func)
        {
            ChunksLoaded.ForEach(func);
        }

        internal Texture2D GetLightTexture(int x, int y)
        {
            return ChunksVisible.Get(x, y)?.LightTexture;
        }

        public void ForEachVisible(Action<Chunk> func)
        {
            ChunksVisible.ForEach(func);
        }

    }

}
