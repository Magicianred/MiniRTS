﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering.Cameras
{
    public sealed class PerspectiveCamera : IMovableViewPoint
    {
        public PerspectiveCamera(Viewport viewport)
        {
            this.NearPlane = 0.1f;
            this.FarPlane = 250.0f;
            this.AspectRatio = viewport.AspectRatio;
            this.FieldOfView = MathHelper.PiOver2;

            Move(Vector3.Backward * 10, Vector3.Zero);            
        }

        public float NearPlane { get; }
        public float FarPlane { get; }
        public float AspectRatio { get; }
        public float FieldOfView { get; }

        public Vector3 Position { get; private set; }
        public Vector3 LookAt { get; private set; }

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }
        public Matrix ViewProjection { get; private set; }
        
        public Matrix InverseView { get; private set; }
        public Matrix InverseProjection { get; private set; }
        public Matrix InverseViewProjection { get; private set; }

        public void Move(Vector3 position, Vector3 lookAt)
        {
            this.Position = position;
            this.LookAt = lookAt;

            this.View = Matrix.CreateLookAt(this.Position, this.LookAt, Vector3.Up);
            this.Projection = Matrix.CreatePerspectiveFieldOfView(
                this.FieldOfView,
                this.AspectRatio,
                this.NearPlane,
                this.FarPlane);
            this.ViewProjection = this.View * this.Projection;

            this.InverseView = Matrix.Invert(this.View);
            this.InverseProjection = Matrix.Invert(this.Projection);
            this.InverseViewProjection = Matrix.Invert(this.ViewProjection);
        }
    }
}