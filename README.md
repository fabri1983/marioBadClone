marioBadClone
=============

It's a proof of several concepts for a 2.5D platform game created in Unity 3.5 targeting ARMv6 Android platforms.
It uses a lot of Super Mario I sprites.
The game is in heavy development phase, only one example level playable by the moment.

Features implemented:
---------------------

- Touch Event Manager:
	It's a manager that acts as a event listener for Unity touch events. 
	It implements a Quadtree solution for fast touch event dispatch according the screen position.
	
- Chipmunk Physics 2D:
	It uses the Unity binding for Chipmunk Physics API.
	Also implemented a solution for managing Unity's layers and Chipmunk layers.
	
- Pause Manager:
	Script components that extend the interface IPausable are attached to the Pause manager component.
	The component registers it self against the Pause manager to receive the notification for pause/resume action.
	The pause action set the MonoBehavior as enabled=false hence no Update() neither FixedUpdate() function is invoked over it.

- Custom GUI implementation:
	In Unity 3.5 the GUI components aren't efficient for rendering, so I decided to design and implement a custom implementation of GUI components that depends on transform operations.
	It let's you set the mesh, texture, size in pixels or proportional, and the possibility of resizing if screen size changes.

- Parallax motion for GUI components:
	Added the possibility to apply a displacement to a material so giving the impression of motion.
	Background and foreground components with their respectively shaders. Highly customizable: size, speed, manual or automatic offset, mirroring.
	
- GUI Screen Layout:
	Locate a GUI component in screen selecting one from nine pre defined locations: top-center, top-left, top-right, middle-center, center-left, center-right, bottom-center, bottom-left, bottom-right.
	It lets you apply manual offsets.
	
- GUI Transition Effects:
	Extending the PoorsManGUIFX implementation for GUI elements transition, this own version handles transform component additionally to the Unity's GUITexture and GUIText. Basic transition effects.
	
- CollisionGen 2D and Harry Plotter:
	Using these two third party APIs you can generate a mesh from an alpha channel texture that acts as a collider, and you have the possibility to modify it directly from the Editor Scene View.
	Going a little further I applied some modifications to end up with a planar mesh which is then used as renderer mesh (to avoid pixel overdraw with transparent textures).
	
- Minimal pixel overdrawing:
	Using quad meshes for displaying transparent textures involves useless fragment operations on those transparent texels.
	To minimize the fragment operations that would be fully transparent with the background, you can use a mesh that outlines the texture.
	There is a trade-off between the complexity of the outlined mesh and the gain in performance of removing transparent texels with that mesh.
	You will provide more vertices/triangles to the pipeline, and less fragments to process.
	
- CG Shaders:
	Every material has its own CG shader with specific optimizations targeting android platform. This allow to remove some assumptions that Unity adds when compiles a shader.
