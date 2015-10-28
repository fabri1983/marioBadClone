marioBadClone
=============
![Travis](https://travis-ci.org/fabri1983/marioBadClone.svg?branch=marioTheme)

This is a personal project as proof of several concepts for a 2D platform game created in Unity 3.5 targeting ARMv6 Android platforms.
Currently it uses a lot of Super Mario I sprites, only for testing purposes.
The game is in heavy development phase, only four very small levels playable so far.
See the section TODO list at the end of this document.

Features implemented:
---------------------

- Sprite Tiled Animation:
	Extended version of the Unity's Wiki Animated Tiled Texture script. It allows to display sequence of tiles from a sprite texture.
	In this personal implementation I expose the possibility of handling a single sheet with different sizes of animations, and another approach different than co-routines (don't work in mobile).
	New feature under testing: moving offset calculations into the textured shader.
	
- Touch Event Manager:
	It's a manager that acts as an event listener for Unity touch events. On screen elements subscribe to the manager in order to receive touch events.
	It implements a screen Quadtree solution for fast touch event dispatch according the touch screen position.
	
- Chipmunk Physics 2D:
	Using the Unity's binding for Chipmunk Physics API (Howling Moon Software © 2013). This physic API is implemented for many platforms (android, iOS, windows, linux) giving a great solution for future portability of the game.
	Also implemented a custom solution for managing Unity's layers and Chipmunk layers.
	Visit the project's page: http://chipmunk-physics.net/
	
- Pause Manager:
	Script components who extend the interface IPausable are attached to the Pause manager component.
	Any component registers itself against the Pause manager to receive the notification for pause/resume action.
	The pause action set the MonoBehavior as enabled=false hence no Update() neither FixedUpdate() function is invoked during game loop.

- Custom GUI implementation:
	In Unity 3.5 the GUI components are a built-in feature making hard/impossible to extent or modify them, so I decided to design and implement a custom implementation of GUI components that depends on transform operations.
	It let's you set the mesh, texture, size in pixels or proportional to screen, and the possibility of resizing if screen size changes.
	
- GUI Screen Layout:
	Locate a GUI component in screen, selecting one from nine pre defined locations: top-center, top-left, top-right, middle-center, center-left, center-right, bottom-center, bottom-left, bottom-right.
	It lets you apply manual offsets.
	
- GUI Transition Effects:
	Extending the PoorsManGUIFX implementation for GUI elements transition, this own version handles transform component of custom GUI solution in addition to Unity's GUITexture and GUIText. 
	Basic transition effects, extensible to new effects.
	Visit the project's page: http://unitycoder.com/blog/2014/05/18/gui-transition-effects-open-source/

- Parallax motion for GUI components:
	Added the possibility to apply a displacement operation onto a material so giving the impression of motion.
	Background and foreground components with their respectively shaders. Highly customizable: size, speed, manual or automatic offset, mirroring (handled in the shader).
	
- Reload Scene transition:
	When the scene needs to be re loaded its an in-scene transition.
	Normally when player loses the scene is reloaded as per Application.LoadLevel(currentScene), which triggers an in-game destroying and re allocation of all the scene components.
	Instead, it comes the ReloadableManager. Which lets components (listeners) be registered to receive a callback when player loses and scene is reloaded.
	Each component attends the callback and know how to reset its attributes and locate its transform so the scene seems to be reloaded from scratch.
	
- CollisionGen 2D and Harry Plotter:
	Using these two third party APIs you can generate a mesh from an alpha channel texture and use it as a collider, giving you the possibility to modify it directly from the Editor Scene View.
	Going a little further I applied some modifications to end up with a planar mesh which is then used as renderer mesh. 
	The use of a custom renderer mesh different than a quad mesh serves as an improvement in rendering time because it avoids pixel overdraw in transparent textures.
	
- Minimal pixel overdrawing:
	Using quad meshes for displaying transparent textures involves useless fragment operations on those transparent texels.
	To minimize the fragment operations that would be fully transparent with the background, you can use a mesh that outlines the texture.
	There is a trade-off between the complexity of the outlined mesh and the gain in performance of removing transparent texels with that mesh.
	You will provide more vertices/triangles to the pipeline, and less fragments to process.
	
- CG Shaders:
	Every material has its own CG shader with specific optimizations targeting mobile platform. This allow to remove some assumptions that Unity adds when compiles a shader.


Build and Deployment:
---------------------
Straight forward following Unity's build and run option.
Note: Currently the custom GUI System implementation doesn't work correctly when the game is built and deployed in a device with a different resolution than the default used in editor.
So for example if you want to build and deploy for an Android device with resolution 480x320 you need to set the same resolution in Unity Editor under Window -> Set Custom Game Window option.


TODO list:
----------
- Atlas Texture. Integration with current sprite animation solution.
- Mesh UV displacement instead of material UV. Better performance for batching and draw calls. Reference: Unite 2013 - Optimizing Unity Games for Mobile Platforms.
- Pool of game objects. Using a pre warming phase to avoid mem allocation during gameplay.
- Virtual Textures? will be a nice feature to allow a lot of unique textures. Under researching since seems to be tricky under GLES 2.0.
- 2-channel textures. Interesting article in texture compression from GPU Pro 5 book.
- Zero mem allocation during gameplay. Has only sense if the game uses too many in-game artifacts. I'm aware I'm executing some new instructions every time the scene is reloaded.
- Environment effects: rain, lightings, snow accumulation.


Contact:
--------
fabri1983@gmail.com
pablo.lettieri@etermax.com

