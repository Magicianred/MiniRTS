﻿using Microsoft.Xna.Framework;
using MiniEngine.Primitives;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace MiniEngine.UI
{
    public sealed class UIState
    {
        private const string UIStateFile = "ui_state.xml";

        public UIState()
        {
            this.EditorState = new EditorState();
            this.EntityState = new EntityState();
            this.DebugState = new DebugState();
        }

        public EditorState EditorState { get; set; }
        public EntityState EntityState { get; set; }
        public DebugState DebugState { get; set; }                
          
        public void Serialize(Vector3 cameraPosition, Vector3 cameraLookAt)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            this.EditorState.CameraPosition = cameraPosition;
            this.EditorState.CameraLookAt = cameraLookAt;
          
            var serializer = new XmlSerializer(typeof(UIState));
            using (var stream = File.CreateText(UIStateFile))
            {
                serializer.Serialize(stream, this);
            }

            Thread.CurrentThread.CurrentCulture = culture;
        }

        public static UIState Deserialize(GBuffer gBuffer)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            try
            {
                var serializer = new XmlSerializer(typeof(UIState));
                using (var stream = File.OpenRead(UIStateFile))
                {
                    return (UIState)serializer.Deserialize(stream);                   
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error: could not deserialize {UIStateFile}, {e.ToString()}");
                return new UIState();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = culture;
            }            
        }       
    }
}
