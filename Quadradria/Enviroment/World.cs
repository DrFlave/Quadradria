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
        public Chunk[,] AllChunks = new Chunk[100, 100];

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

        public void Render(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    AllChunks[i, j].Render(spriteBatch);
                }
            }
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    AllChunks[i, j].Draw(spriteBatch);
                }
            }
        }
    }
}
