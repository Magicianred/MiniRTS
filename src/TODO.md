# TODO

- Unify geometry generation, let users pass in a vertex declaration and get the vertex and index buffer?

- Create a better multi-threaded render pipeline. Maybe have one worker thread that pushes an entire pipeline to a queue, and then have the workers handling those systems. Uses a semaphore or something to sync? 

- Sunlight and Spotlight both use a shadow map, but the sampling is subtely different because one uses an array and the other not, can we unify this?

# Idea

Make a scene with many asteroids (like 10 variations instanced in different orientations). Put some sort of 'fog' in between to simulate space dust. Take the thickness from this fog from a RT where I draw over black swatches when a rocket goes over it. At sun rays?


- Fog/Dust, Ravendarke at Monogame discord: https://www.slideshare.net/BenjaminGlatzel/volumetric-lighting-for-many-lights-in-lords-of-the-fallen


# WIP - Fog
- Can we add some scattering/*bloom*? <---- BLOOM might actually fake ray scattering a bit?

- Fog added 2 full screen render targets to the LBuffer, can we optimize this
- How can we give more parameters to tweak the fog?
- How can we have different parameters for different volumes (do we need that) at the same time..?
- Right now it only supports concave geometry as a volume?
