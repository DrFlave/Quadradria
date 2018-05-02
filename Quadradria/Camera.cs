using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria
{
    class Camera
    {
        public float zoom;
        public Matrix transform;
        public Vector2 pos;
        public Vector2 size;
        public float rotation;

        public Camera(float x, float y, float width, float height)
        {
            zoom = 1.0f;
            rotation = 0.0f;
            pos = new Vector2(x, y);
            size = new Vector2(width, height);
        }

        public Matrix getTransformation(GraphicsDevice graphicsDevice)
        {
            return Matrix.CreateTranslation(new Vector3(-pos.X, -pos.Y, 0)) *
                                            Matrix.CreateRotationZ(rotation) *
                                            Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                                            Matrix.CreateTranslation(new Vector3(size.Y * 0.5f, size.Y * 0.5f, 0));
        }

    }
}
