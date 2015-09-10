using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Touch Event Manager.
/// This class catches the events from touch screen, and call the instances registered to the event trigered.
/// The class which wants to be called when a touch event is triggered needs to implement interface ITouchListener.
/// Also it needs to register it self in a desired event type against this manager.
/// Once the scene is destroyed its game ojects also are destroyed, thus the Input Manager needs to remove the listeners when 
/// game object is destroyed. For that call removeListener().
/// </summary>
public class TouchEventManager {

	private static ListenerLists dynamicListeners = new ListenerLists();
	private static QuadTreeTouchEvent staticQuadTree = new QuadTreeTouchEvent();
	
	/// <summary>
	/// If true then you can touch overlapped game objects (in screen)
	/// If false then once touched a game object for current touch and current phase there is no need to 
	/// continue looking for another touched game object.
	/// </summary>
	private static bool ALLOW_TOUCH_HITS_OVERLAPPED_OBJECTS = false;
	
	private static TouchEventManager instance = null;
	
	public static TouchEventManager Instance {
        get {
            warm();
            return instance;
        }
    }

	public static void warm () {
		// in case the class wasn't instantiated yet from another script
		if (instance == null) {
			// creates the innner instance
			instance = new TouchEventManager();
		}
	}
	
	private TouchEventManager () {
		// instantiates the game object which will do the updating of Unity touch events
		GameObject.Instantiate(Resources.Load("Prefabs/TouchEventUnityUpdater"));
	}

	~TouchEventManager () {
		dynamicListeners.clear();
		staticQuadTree.clear();
		instance = null;
	}

	/// <summary>
	/// Register the specified iListener in the adequate list to be processed on every touch event.
	/// </summary>
	/// <param name='listener'>
	/// The object you are registering. Needed for callback invocations.
	/// </param>
	/// <param name='touchPhases'>
	/// All the touch phases you want your gameobject be registered to.
	/// </param>
	public void register (ITouchListener listener, params TouchPhase[] touchPhases) {
		
		#if TOUCH_EVENT_MANAGER_NO_QUADTREE
		for (int i=0, c=touchPhases.Length; i < c; ++i)
			dynamicListeners.add(listener, touchPhases[i]);
		#else
		if (!listener.isScreenStatic()) {
			for (int i=0, c=touchPhases.Length; i < c; ++i)
				dynamicListeners.add(listener, touchPhases[i]);
		}
		else {
			/// adding the listener's screen rect into the quad tree will return a list of 
			/// as much as 4 ListenerLists elems, since the listener can fall in more than one quadrant
			Rect screenBounds = listener.getScreenBoundsAA();
			ListenerLists[] leaves = staticQuadTree.add(screenBounds);
			for (int i=0, c=leaves.Length; i < c; ++i) {
				// first listenerList element being not valid means next ones are also invalid
				if (leaves[i] == null)
					break;
				for (int j=0, d=touchPhases.Length; j < d; ++j)
					leaves[i].add(listener, touchPhases[j]);
			}
		}
		#endif
	}
	
	/// <summary>
	/// Invoke from OnDestroy() method in your per level game object.
	/// Uses the hash code to identify the listener over the list of many listeners.
	/// </summary>
	public void removeListener (ITouchListener listener) {

		#if TOUCH_EVENT_MANAGER_NO_QUADTREE
		dynamicListeners.remove(listener);
		#else
		if (!listener.isScreenStatic())
			dynamicListeners.remove(listener);
		else {
			// need to remove from every list the listener appears
			ListenerLists[] lls = staticQuadTree.traverse(listener.getScreenBoundsAA());
			for (int i=0,c=lls.Length; i < c; ++i) {
				// first listenerList element being not valid means next ones are also invalid
				if (lls[i] == null)
					break;
				lls[i].remove(listener);
			}
		}
		#endif
	}
	
	/// <summary>
	/// Called form the TouchEventUnityUpdater, process the touches of current frame.
	/// </summary>
	public void processEvents () {
		if (Input.touchCount == 0)
			return;

		#if DEBUG_TOUCH_EVENT
		for (int i=0; i < Input.touchCount; ++i) {
			Touch tch = Input.touches[i];
			Debug.Log(Input.touchCount + ": " + tch.phase + " -> finger " + tch.fingerId);
		}
		#endif
		
		// send events to listeners
		for (int i=0,c=Input.touchCount; i < c; ++i) {
			Touch t = Input.touches[i];
			#if TOUCH_EVENT_MANAGER_NO_QUADTREE
			sendEvent(t, dynamicListeners);
			#else
			// static listeners
			if (sendEvent(t, staticQuadTree.traverse(t.position)))
				continue;
			// dynamic listeners
			sendEvent(t, dynamicListeners);
			#endif
		}
	}
	
	private bool sendEvent (Touch tch, ListenerLists ll) {
		if (ll == null)
			return false;
		
		// NOTE: traverse the arrays and ask if something hitten in the correct order: 
		//    Began, Stationary/Move, Ended, Canceled
		
		ITouchListener[] arrays = null;
		
		// which lists to traverse? according to touch phase
		switch (tch.phase) {
			case TouchPhase.Began:
				arrays = ll.beganListeners;
				break;
			case TouchPhase.Stationary:
			case TouchPhase.Moved:
				arrays = ll.stationaryListeners;
				break;
			default:
				// Ended and Canceled
				arrays = ll.endedListeners;
				break;
			// add here if else for other touch phases
		}
		
		// one loop for scene only, another loop for global
		bool atLeastOneHit = false;
			
		for (int i=0,c=arrays.Length; i < c; ++i) {
			
			ITouchListener listener = arrays[i];
			if (listener == null)
				continue;
			
			bool hitInner = false;
			
			// Use detection as in chipmunk platformer, since here I don't use physx colliders
			// (Or use cpShapeQuerySegment (see online documentation from release, cpShape class))
			// Now testing AABB using the screen rect of the GUI element
			hitInner = listener.getScreenBoundsAA().Contains(tch.position);
			
			if (!hitInner)
				continue;

			// invoke callback
			switch (tch.phase) {
				case TouchPhase.Began:
					listener.OnBeganTouch(tch);
					break;
				case TouchPhase.Stationary:
				case TouchPhase.Moved:
					listener.OnStationaryTouch(tch);
					break;
				// Ended and Canceled
				default:
					listener.OnEndedTouch(tch);
					break;
				// add here if else for other methods depending on touch phases
			}

			if (!ALLOW_TOUCH_HITS_OVERLAPPED_OBJECTS)
				return true;
			atLeastOneHit = true;
		}
		
		if (!ALLOW_TOUCH_HITS_OVERLAPPED_OBJECTS && atLeastOneHit)
			return true;
		
		return false;
	}
}
