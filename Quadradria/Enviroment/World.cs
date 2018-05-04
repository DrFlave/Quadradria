using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Enviroment
{
    class World
    {

        struct Rect
        {
            public int x1, y1, x2, y2;

            public Rect(int x1, int y1, int x2, int y2)
            {
                this.x1 = x1;
                this.y1 = y1;
                this.x2 = x2;
                this.y2 = y2;
            }

            public static bool operator ==(Rect x, Rect y)
            {
                return x.x1 == y.x1 && x.y1 == y.y1 && x.x2 == y.x2 && x.y2 == y.y2;
            }

            public static bool operator !=(Rect x, Rect y)
            {
                return !(x == y);
            }
        }


        public Chunk[,] AllChunks = new Chunk[100, 100];

        public List<Chunk> activeChunks = new List<Chunk>();

        private Rect lastRect = new Rect(0, 0, 0, 0);
        

        public World(GraphicsDevice graphicsDevice)
        {

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    AllChunks[i, j] = new Chunk(i, j, graphicsDevice);
                }
            }
        }

        public void Update(int x, int y,  int width, int height)
        {
            int minX = x / 16;
            int minY = y / 16;
            int maxX = (x + width) / 16 +1;
            int maxY = (y + height) / 16 +1;

            Rect rect = new Rect(minX, minY, maxX, maxY);
            if (rect == lastRect) return;

            activeChunks.Clear();

            for (int yy = minY; yy < maxY; yy++)
            {
                for (int xx = minX; xx < maxX; xx++)
                {
                    activeChunks.Add(getInactiveChunk(xx, yy));
                }
            }
        }

        private Chunk getInactiveChunk(int x, int y)
        {
            if (x < 0 || y < 0) return null;
            return (AllChunks[x, y]);
        }

        public void Render(SpriteBatch spriteBatch)
        {
            for (var i = 0; i < activeChunks.Count; i++)
            {
                if(activeChunks[i] != null) activeChunks[i].Render(spriteBatch);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (var i = 0; i < activeChunks.Count; i++)
            {
                if (activeChunks[i] != null) activeChunks[i].Draw(spriteBatch);
            }
        }
    }
}
