﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Lighting;
using MiniEngine.Units;
using DirectionalLight = MiniEngine.Rendering.Lighting.DirectionalLight;

namespace MiniEngine.Scenes
{
    public interface IScene
    {        
        List<DirectionalLight> DirectionalLights { get; }
        List<ShadowCastingLight> ShadowCastingLights { get; }

        Color AmbientLight { get; }

        void Draw(IViewPoint viewPoint);
        void Draw(Effect effectOverride, IViewPoint viewPoint);
        void LoadContent(ContentManager content);
        void Set(SystemCollection systems);
        void Update(Seconds elapsed);


    }
}