﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Utilities;
using MiniEngine.Systems.Components;
using MiniEngine.Units;
using System;
using System.Collections.Generic;

namespace MiniEngine.Pipeline.Particles.Components
{
    public abstract class AEmitter : IComponent
    {
        private static readonly Random Random = new Random();

        private Seconds timeToSpawn;
        private Vector4 tint;

        public AEmitter(Vector3 position, Texture2D texture, int rows, int columns, float scale)
        {
            this.Position = position;
            this.Texture = texture;
            this.Rows = rows;
            this.Columns = columns;

            this.Particles = new List<Particle>();

            this.SpawnInterval = 0.05f;
            this.timeToSpawn = 0.0f;

            this.Scale = scale;
            this.Direction = Vector3.Up;
            this.Speed = 3.0f;
            this.Spread = 0.5f;
            this.TimeToLive = 2.0f;
            this.TimePerFrame = 0.125f;
            this.Tint = Color.White;
        }

        public Vector3 Position { get; set; }
        public Texture2D Texture { get; }
        public int Rows { get; }
        public int Columns { get; }
        public List<Particle> Particles { get; }

        public Seconds SpawnInterval { get; set; }

        public float Scale { get; set; }
        public Vector3 Direction { get; set; }
        public float Speed { get; set; }
        public float Spread { get; set; }
        public Seconds TimeToLive { get; set; }
        public Seconds TimePerFrame { get; set; }

        public Color Tint
        {
            get => new Color(this.tint);
            set => this.tint = value.ToVector4();
        }

        public void Update(Seconds elapsed)
        {
            for (var i = this.Particles.Count - 1; i >= 0; i--)
            {
                var particle = this.Particles[i];
                particle.LifeTime += elapsed;
                if (particle.LifeTime >= this.TimeToLive)
                {
                    this.Particles.RemoveAt(i);
                }
                else
                {
                    particle.Position += particle.LinearVelocity * elapsed.Value;
                    particle.Frame = (int)(particle.LifeTime.Value / particle.TimePerFrame.Value);

                    var progress = particle.LifeTime / this.TimeToLive;
                    var lifeTimeRatio = 1.0f - Easings.ExponentialEaseIn(progress);
                    particle.Tint = new Color(this.tint.X * lifeTimeRatio, this.tint.Y * lifeTimeRatio, this.tint.Z * lifeTimeRatio, this.tint.W * lifeTimeRatio);
                }
            }

            this.timeToSpawn -= elapsed;
            if (this.timeToSpawn <= 0.0f)
            {
                var velocity = Vector3.Normalize(this.Direction + this.GetSpreadVector()) * this.Speed;

                this.timeToSpawn += this.SpawnInterval;
                this.Particles.Add(
                    new Particle(
                        this.Position,
                        this.Scale,
                        velocity,
                        this.TimePerFrame));
            }
        }

        public abstract ComponentDescription Describe();

        protected void Describe(ComponentDescription description)
        {
            description.AddProperty("Position", this.Position, x => this.Position = x, MinMaxDescription.MinusInfinityToInfinity);
            description.AddProperty("Spawn interval", this.SpawnInterval, x => this.SpawnInterval = x, MinMaxDescription.ZeroToInfinity);
            description.AddProperty("Scale", this.Scale, x => this.Scale = x, MinMaxDescription.ZeroToInfinity);
            description.AddProperty("Direction", this.Direction, x => this.Direction = x, MinMaxDescription.MinusOneToOne);
            description.AddProperty("Speed", this.Speed, x => this.Speed = x, MinMaxDescription.ZeroToInfinity);
            description.AddProperty("Spread", this.Spread, x => this.Spread = x, MinMaxDescription.ZeroToOne);
            description.AddProperty("Time to live", this.TimeToLive, x => this.TimeToLive = x, MinMaxDescription.ZeroToInfinity);
            description.AddProperty("Time per frame", this.TimePerFrame, x => this.TimePerFrame = x, MinMaxDescription.ZeroToInfinity);
            description.AddProperty("Tint", this.Tint, t => this.Tint = t, MinMaxDescription.ZeroToOne);
            description.AddLabel("Particles", this.Particles.Count);
        }

        private Vector3 GetSpreadVector()
        {
            var x = GetRandomOffset() * this.Spread;
            var y = GetRandomOffset() * this.Spread;
            var z = GetRandomOffset() * this.Spread;

            return new Vector3(x, y, z);
        }

        private static float GetRandomOffset() => ((float)Random.NextDouble() * 2.0f) - 1.0f;

        public override string ToString() => $"emitter, position: {this.Position}, particles: {this.Particles.Count}";
    }
}