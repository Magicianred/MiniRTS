﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Models.Components
{
    public abstract class AModel : IComponent
    {
        protected AModel(Entity entity, Model model)
        {
            this.Entity = entity;
            this.Model = model;
            this.TextureScale = Vector2.One;

            if (model.Tag is SkinningData)
            {
                this.SkinTransforms = new Matrix[SkinningData.MaxBones];
                this.HasAnimations = true;
            }
        }

        public Entity Entity { get; }

        public Model Model { get; }

        [Editor(nameof(TextureScale))]
        public Vector2 TextureScale { get; set; }

        public IconType Icon => IconType.Model;

        public Matrix[] SkinTransforms { get; set; }

        public bool HasAnimations { get; }
    }
}
