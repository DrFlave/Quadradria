﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quadradria.Utils;
using Microsoft.Xna.Framework.Input;

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

        //public float x;
        //public float y;

        public Camera(Viewport viewport)
        {
            this.viewport = viewport;
        }

        public void Update(Vector2 position)
        {
            center = new Vector2(position.X, position.Y);

            transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * 
                        Matrix.CreateRotationZ(rotation) *
                        //Matrix.CreateScale(new Vector3(zoom, zoom, 0) * zoomScale) *
                        Matrix.CreateScale(zoomScale * zoom, zoomScale * zoom, 1) *
                        Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
        }

        public RectF GetRect()
        {
            return new RectF((center.X - viewport.Width / zoomScale / zoom / 2), (center.Y - viewport.Height / zoomScale / zoom / 2), (viewport.Width / zoomScale / zoom), (viewport.Height / zoomScale / zoom));
        }

        public void Resize(Viewport viewport)
        {
            this.viewport = viewport;

            center = new Vector2(this.viewport.Width / 2, this.viewport.Height / 2);
        }

        public Vector2 GetMousePositionInWorld()
        {
            MouseState mouseState = Mouse.GetState();
            return Vector2.Transform(mouseState.Position.ToVector2(), Matrix.Invert(transform));
        }

    }
}
