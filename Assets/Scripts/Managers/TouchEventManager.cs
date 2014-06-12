using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Touch Event Manager.
/// This class catches the events from touch screen, and call the instances registered to the event trigered.
/// The class which wants to be called when an event is triggered needs to implement interface IInputListener.
/// Also it needs to register it self in a desired event type against this input manager.
/// Once the scene is destroyed its game ojects also are destroyed, thus the Input Manager needs to remove the listeners when 
/// game object is destroyed. For that call removeListener().
/// </summary>
public class TouchEventManager : MonoBehaviour {

	private static ListenerLists dynamicListeners = new ListenerLists(); // it's a struct, no instancing needed
	private static QuadTreeTouchEvent staticQuadTree = new QuadTreeTouchEvent();
	
	/// If true then you can touch overlapped game objects (in screen)
	/// If false then once touched a game object for current touch and current phase there is no need to 
	/// continue looking for another touched game object.
	private static bool ALLOW_TOUCH_HITS_OVERLAPPED_OBJECTS = false;
	
	private static TouchEventManager instance = null;
	
	public static TouchEventManager Instance {
        get {
            warm();
            return instance;
        }
    }

	public static void warm () {
		// in case the game object wasn't instantiated yet from another script
		if (instance == null) {
			// creates a game object with this script component
			instance = new GameObject("TouchEventManager").AddComponent<TouchEventManager>();
			DontDestroyOnLoad(instance);
		}
		// force to activate multi touch support
		Input.multiTouchEnabled = true;
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

		if (!listener.isStatic())
			dynamicListeners.add(listener, touchPhases);
		else {
			/// adding the listener's screen rect into the quad tree will return a list of 
			/// as much as 4 ListenerLists elems, since the listener can fall in more than one quadrant
			ListenerLists[] leaves = staticQuadTree.add(listener.getScreenBoundsAA());
			for (int i=0, c=leaves.Length; i < c; ++i) {
				// first listenerList element being not valid means next ones are also invalid
				if (leaves[i] == null)
					break;
				leaves[i].add(listener, touchPhases);
			}
		}
	}
	
	/// <summary>
	/// Invoke from OnDestroy() method in your per level game object.
	/// Uses the hash code to identify the listener over the list of many listeners.
	/// </summary>
	public void removeListener (ITouchListener listener) {

		if (!listener.isStatic())
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
	}
	
	void OnApplicationQuit () {
		dynamicListeners.clear();
		staticQuadTree.clear();
		
		// NOTE: to avoid !IsPlayingOrAllowExecuteInEditMode error in console:
		//instance = null;
	}
	
	void Update () {
		processEvents();
	}
	
	private static void processEvents () {
		
		if (Input.touchCount == 0)
			return;

#if UNITY_EDITOR
		if (Debug.isDebugBuild) {
			for (int i=0; i < Input.touchCount; ++i)
				Debug.Log(Input.touchCount + ": " + Input.touches[i].phase + " -> finger " + Input.touches[i].fingerId);
		}
#endif
		// send events to listeners
		for (int i=0; i < Input.touchCount; ++i) {
			Touch t = Input.touches[i];
			// static listeners
			if (sendEvent(t, staticQuadTree.traverse(t.position)))
				continue;
			// dynamic listeners
			sendEvent(t, dynamicListeners);
		}
	}
	
	private static bool sendEvent (Touch t, ListenerLists ll) {
		if (ll == null)
			return false;
		
		// NOTE: traverse the lists and ask if something hitten in the correct order: Began, Stationary/Move, Ended, Canceled
		
		List<ITouchListener> list = null;
		
		// which lists to traverse? according to touch phase
		if (TouchPhase.Began.Equals(t.phase))
			list = ll.beganListeners;
		else if (TouchPhase.Stationary.Equals(t.phase) || TouchPhase.Moved.Equals(t.phase))
			list = ll.stationaryListeners;
		else if (TouchPhase.Ended.Equals(t.phase) || TouchPhase.Canceled.Equals(t.phase))
			list = ll.endedListeners;
		// add here if else for other touch phases
		
		// one loop for scene only, another loop for global
		bool atLeastOneHit = false;
			
		for (int i=0; list != null && i < list.Count; ++i) {
			
			ITouchListener listener = list[i];
			if (listener == null)
				continue;
			
			GameObject go = listener.getGameObject();
			bool hitInner = false;
			
			// check for GUI Texture
			if (go.guiTexture != null && go.guiTexture.HitTest(t.position))
				hitInner = true;
			// check for GUI Text
			else if (go.guiText != null && go.guiText.HitTest(t.position))
				hitInner = true;
			// check for game object
			else {
				// use detection as in chipmunk platformer, since here I don't use physx colliders
				// or use cpShapeQuerySegment (see online documentation from release, cpShape class)
			}
			
			// hitten? then invoke callback
			if (hitInner) {
				if (TouchPhase.Began.Equals(t.phase))
					list[i].OnBeganTouch(t);
				else if (TouchPhase.Stationary.Equals(t.phase) || TouchPhase.Moved.Equals(t.phase))
					list[i].OnStationaryTouch(t);
				else if (TouchPhase.Ended.Equals(t.phase))
					list[i].OnEndedTouch(t);
				// add here if else for other methods depending on touch phases
				
				if (!ALLOW_TOUCH_HITS_OVERLAPPED_OBJECTS)
					return true;
				atLeastOneHit = true;
			}
		}
		
		if (!ALLOW_TOUCH_HITS_OVERLAPPED_OBJECTS && atLeastOneHit)
			return true;
		
		return false;
	}
}
