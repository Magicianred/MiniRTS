﻿using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.UI
{
    public sealed class EntityWindow
    {
        private readonly Editors Editors;
        private readonly EntityState EntityState;
        private readonly EntityManager EntityManager;

        private readonly Dictionary<Type, int> ComponentCounter;
        
        
        public EntityWindow(Editors editors, UIState ui, EntityManager entityManager)
        {
            this.Editors = editors;
            this.EntityState = ui.EntityState;            
            this.EntityManager = entityManager;

            this.ComponentCounter = new Dictionary<Type, int>();
        }
        
        public void Render()
        {
            if (ImGui.Begin("Entity Details", ref this.EntityState.ShowEntityWindow, ImGuiWindowFlags.NoCollapse))
            {
                ImGui.Text($"{this.EntityState.SelectedEntity}");

                this.ComponentCounter.Clear();

                var components = new List<IComponent>();
                this.EntityManager.Linker.GetComponents(this.EntityState.SelectedEntity, components);

                foreach (var component in components)
                {
                    // ImGui requires a unique name for every node, so for each component we add
                    // check how many of that component we've already added and use that in the name
                    var count = this.Count(component);

                    var name = GetLabel(component);
                    if (ImGui.TreeNode(name + " #" + count.ToString("00")))
                    {
                        this.CreateEditors(component);

                        if (ImGui.Button("Remove Component"))
                        {
                            this.EntityManager.Linker.RemoveComponent(this.EntityState.SelectedEntity, component);
                        }
                        ImGui.TreePop();
                    }
                }

                if (this.EntityManager.Creator.GetAllEntities().Contains(this.EntityState.SelectedEntity))
                {
                    if (ImGui.Button("Destroy Entity"))
                    {
                        this.EntityManager.Controller.DestroyEntity(this.EntityState.SelectedEntity);
                        this.EntityState.ShowEntityWindow = false;
                        var entities = this.EntityManager.Creator.GetAllEntities();
                        this.EntityState.SelectedEntity = entities.Any() ? entities.First() : new Entity(-1);
                    }
                }

                ImGui.End();
            }
        }

        private void CreateEditors(IComponent component)
        {
            var componentType = component.GetType();

            var attributes = componentType.GetProperties()
                .SelectMany(p => p.GetCustomAttributes(typeof(EditorAttribute), false))
                .Cast<EditorAttribute>();

            foreach (var attribute in attributes)
            {
                var getter = GetGetter(attribute.Getter, component, componentType);
                var setter = GetSetter(attribute.Setter, component, componentType);                

                this.Editors.Create(attribute.Name, getter(), attribute.MinMax, setter);
            }
        }

        private static string GetLabel(IComponent component)
        {
            return component.GetType().GetCustomAttributes(typeof(LabelAttribute), false).Cast<LabelAttribute>()
                .Select(l => l.Label)
                .FirstOrDefault();
        }

        private static Func<object> GetGetter(string name, IComponent component, Type componentType)
        {
            var property = componentType.GetProperty(name);
            if (property != null)
            {
                return () => property.GetGetMethod().Invoke(component, null);
            }

            return null;
        }

        private static Action<object> GetSetter(string name, IComponent component, Type componentType)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            var property = componentType.GetProperty(name);
            if (property != null && property.GetSetMethod() != null)
            {
                return o => property.GetSetMethod().Invoke(component, new object[] { o });
            }

            var method = componentType.GetMethod(name);
            if(method != null)
            {
                return o => method.Invoke(component, new object[] { o });
            }

            return null;
        }

        private int Count(IComponent component)
        {
            if (this.ComponentCounter.ContainsKey(component.GetType()))
            {
                this.ComponentCounter[component.GetType()] += 1;
            }
            else
            {
                this.ComponentCounter.Add(component.GetType(), 0);
            }

            var id = this.ComponentCounter[component.GetType()];
            return id;
        }
    }
}
