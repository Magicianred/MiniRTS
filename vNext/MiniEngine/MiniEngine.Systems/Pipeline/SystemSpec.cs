﻿using System;
using System.Collections.Generic;

namespace MiniEngine.Systems.Pipeline
{
    public sealed class SystemSpec
    {
        private readonly List<ResourceState> RequiresList;
        private readonly List<ResourceState> ProducesList;

        private SystemSpec(Type systemType)
        {
            this.SystemType = systemType;
            this.RequiresList = new List<ResourceState>();
            this.ProducesList = new List<ResourceState>();
            this.AllowParallelism = false;
        }

        public static SystemSpec Construct<T>()
            where T : ISystem
            => new SystemSpec(typeof(T));

        public Type SystemType { get; }

        public bool AllowParallelism { get; private set; }

        internal IReadOnlyList<ResourceState> RequiredResources => this.RequiresList;
        internal IReadOnlyList<ResourceState> ProducedResources => this.ProducesList;

        public SystemSpec Requires(string resource, string state)
        {
            this.RequiresList.Add(new ResourceState(resource, state));
            return this;
        }

        public SystemSpec Produces(string resource, string state)
        {
            this.ProducesList.Add(new ResourceState(resource, state));
            return this;
        }

        public SystemSpec Parallel()
        {
            this.AllowParallelism = true;
            return this;
        }


        public SystemSpec InSequence()
        {
            this.AllowParallelism = false;
            return this;
        }

        public override string ToString()
            => $"{this.SystemType.Name}: " +
            $"allow parallelism: {this.AllowParallelism}, " +
            $"requires: [{string.Join(", ", this.RequiredResources)}], " +
            $"produces: [{string.Join(", ", this.ProducedResources)}]";
    }
}
