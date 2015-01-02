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

	private static ListenerLists dynamicListeners = new ListenerLists();
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
		}
	}
	
	void Awake () {
		if (instance != null && instance != this)
			Destroy(this.gameObject);
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
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
#if UNITY_EDITOR
#else
		// NOTE: to avoid !IsPlayingOrAllowExecuteInEditMode error in console
		instance = null;
#endif
	}
	
	void Update () {
		processEvents();
	}
	
	private static void processEvents () {
		
		if (Input.touchCount == 0)
			return;

#if UNITY_EDITOR
		for (int i=0; i < Input.touchCount; ++i) {
			Touch tch = Input.touches[i];
			Debug.Log(Input.touchCount + ": " + tch.phase + " -> finger " + tch.fingerId);
		}
#endif
		// send events to listeners
		for (int i=0,c=Input.touchCount; i < c; ++i) {
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
		
		// NOTE: traverse the lists and ask if something hitten in the correct order: 
		//    Began, Stationary/Move, Ended, Canceled
		
		List<ITouchListener> list = null;
		
		// which lists to traverse? according to touch phase
		switch (t.phase) {
			case TouchPhase.Began:
				list = ll.beganListeners;
				break;
			case TouchPhase.Stationary:
			case TouchPhase.Moved:
				list = ll.stationaryListeners;
				break;
			default:
				// Ended and Canceled
				list = ll.endedListeners;
				break;
			// add here if else for other touch phases
		}
		
		if (list == null)
			return false;
		
		// one loop for scene only, another loop for global
		bool atLeastOneHit = false;
			
		for (int i=0,c=list.Count; i < c; ++i) {
			
			ITouchListener listener = list[i];
			/*if (listener == null)
				continue;*/
			
			GameObject go = listener.getGameObject();
			bool hitInner = false;
			
			// check for GUI Texture
			if (go.guiTexture != null)
				hitInner = go.guiTexture.HitTest(t.position);
			// check for GUI Text
			else if (go.guiText != null)
				hitInner = go.guiText.HitTest(t.position);
			// check for game object
			else
				// use detection as in chipmunk platformer, since here I don't use physx colliders
				// or use cpShapeQuerySegment (see online documentation from release, cpShape class)
				hitInner = GameObjectTools.testHitFromScreenPos(go.transform, t.position);
			
			if (!hitInner)
				continue;
			
			// invoke callback
			switch (t.phase) {
				case TouchPhase.Began:
					list[i].OnBeganTouch(t);
					break;
				case TouchPhase.Stationary:
				case TouchPhase.Moved:
					list[i].OnStationaryTouch(t);
					break;
				// Ended and Canceled
				default:
					list[i].OnEndedTouch(t);
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
