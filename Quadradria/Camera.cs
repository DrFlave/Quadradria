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
        public Matrix transform;
        public Vector2 center;
        private Viewport viewport;

        public float zoom = 1;
        private float zoomScale = 16;
        public float rotation = 0;

        public float x;
        public float y;

        public Camera(Viewport viewport)
        {
            this.viewport = viewport;
        }

        public void Update(Vector2 position)
        {
            center = new Vector2(position.X, position.Y);

            transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * 
                        Matrix.CreateRotationZ(rotation) * 
                        Matrix.CreateScale(new Vector3(zoom, zoom, 0) * zoomScale) *
                        Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
        }

        public void Resize(Viewport viewport)
        {
            this.viewport = viewport;

            center = new Vector2(this.viewport.Width / 2, this.viewport.Height / 2);
        }

    }
}
