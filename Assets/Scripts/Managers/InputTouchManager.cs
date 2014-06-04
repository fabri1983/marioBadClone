using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Input manager.
/// This class catches the events from touch screen, and call the instances registered to the event trigered.
/// The class which wants to be called when an event is triggered needs to implement interface IInputListener.
/// Also it needs to register it self in a desired event type against this input manager.
/// There are two types of registration: scene only and eternal. Scene only is when the listener lives only in the scene. 
/// Once the scene is destroyed its game ojects also are destroyed, thus the Input Manager needs to remove those listeners when 
/// scene is destroyed.
/// </summary>
public class InputTouchManager : MonoBehaviour {
	
	private static List<ITouchListener> beganEternalListeners = new List<ITouchListener>(5);
	private static List<ITouchListener> beganSceneOnlyListeners = new List<ITouchListener>(5);
	private static List<ITouchListener> stationaryEternalListeners = new List<ITouchListener>(5);
	private static List<ITouchListener> stationarySceneOnlyListeners = new List<ITouchListener>(5);
	private static List<ITouchListener> endedEternalListeners = new List<ITouchListener>(5);
	private static List<ITouchListener> endedSceneOnlyListeners = new List<ITouchListener>(5);
	
	/// If true then you can touch overlapped game objects (in screen)
	/// If false then once touched a game object for current touch and current phase there is no need to 
	/// continue looking for another touched game object.
	private static bool ALLOW_TOUCH_HITS_OVERLAPPED_OBJECTS = false;
	
	private static InputTouchManager instance = null;
	
	public static InputTouchManager Instance {
        get {
            warm();
            return instance;
        }
    }

	public static void warm () {
		// in case the game object wasn't instantiated yet from another script
		if (instance == null) {
			// creates a game object with this script component
			instance = new GameObject("InputTouchManager").AddComponent<InputTouchManager>();
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
	/// <param name='isSceneOnly'>
	/// True if the registration is only valid during the liveness of the scene. False otherwise.
	/// </param>
	/// <param name='touchPhases'>
	/// All the touch phases you want your gameobject be registered to.
	/// </param>
	public void register (ITouchListener listener, bool isSceneOnly, params TouchPhase[] touchPhases) {
		
		for (int i=0; i < touchPhases.Length; ++i) {
			TouchPhase phase = touchPhases[i];
			if (TouchPhase.Began.Equals(phase)) {
				if (isSceneOnly)
					beganSceneOnlyListeners.Add(listener);
				else
					beganEternalListeners.Add(listener);
			}
			// stationary and moved seems to be very related
			else if (TouchPhase.Stationary.Equals(phase) || TouchPhase.Moved.Equals(phase)) {
				if (isSceneOnly)
					stationarySceneOnlyListeners.Add(listener);
				else
					stationaryEternalListeners.Add(listener);
			}
			else if (TouchPhase.Ended.Equals(phase) || TouchPhase.Canceled.Equals(phase)) {
				if (isSceneOnly)
					endedSceneOnlyListeners.Add(listener);
				else
					endedEternalListeners.Add(listener);
			}
			// add here else if for other phases
		}
	}
	
	/// <summary>
	/// Invoke from OnDestroy() method in your per level game object.
	/// Removes the reference of the listener that only exist per game scene.
	/// Use the hash code to identify the listener over the list of many listeners.
	/// </summary>
	public void removeSceneOnlyListener (ITouchListener listener) {
		
		int id = listener.GetHashCode();
		
		for (int i=0; i < beganSceneOnlyListeners.Count; ++i) {
			if (beganSceneOnlyListeners[i] == null)
				continue;
			if (id == beganSceneOnlyListeners[i].GetHashCode())
				//beganSceneOnlyListeners[i] = null;
				beganSceneOnlyListeners.RemoveAt(i);
		}
	
		for (int i=0; i < stationarySceneOnlyListeners.Count; ++i) {
			if (stationarySceneOnlyListeners[i] == null)
				continue;
			if (id == stationarySceneOnlyListeners[i].GetHashCode())
				//stationarySceneOnlyListeners[i] = null;
				stationarySceneOnlyListeners.RemoveAt(i);
		}
	
		for (int i=0; i < endedSceneOnlyListeners.Count; ++i) {
			if (endedSceneOnlyListeners[i] == null)
				continue;
			if (id == endedSceneOnlyListeners[i].GetHashCode())
				//endedSceneOnlyListeners[i] = null;
				endedSceneOnlyListeners.RemoveAt(i);
		}
		
		// add here if else for other traverses for different touch phases
	}
	
	void OnApplicationQuit () {
		beganEternalListeners.Clear();
		beganSceneOnlyListeners.Clear();
		stationaryEternalListeners.Clear();
		stationarySceneOnlyListeners.Clear();
		endedEternalListeners.Clear();
		endedSceneOnlyListeners.Clear();
		
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
		for (int i=0; i < Input.touchCount; ++i)
			sendEvent(Input.touches[i]);
	}
	
	private static void sendEvent (Touch t) {

		//############################
		// NOTE: traverse the lists and ask if hitten in the correct order: Began, Stationary/Move, Ended, Canceled
		//############################
		
		List<ITouchListener> listEternal = null;
		List<ITouchListener> listSceneOnly = null;
		List<ITouchListener> list = null;
		
		// which lists to traverse? according to touch phase
		if (TouchPhase.Began.Equals(t.phase)) {
			listEternal = beganEternalListeners;
			listSceneOnly = beganSceneOnlyListeners;
		}
		else if (TouchPhase.Stationary.Equals(t.phase) || TouchPhase.Moved.Equals(t.phase)) {
			listEternal = stationaryEternalListeners;
			listSceneOnly = stationarySceneOnlyListeners;
		}
		else if (TouchPhase.Ended.Equals(t.phase) || TouchPhase.Canceled.Equals(t.phase)) {
			listEternal = endedEternalListeners;
			listSceneOnly = endedSceneOnlyListeners;
		}
		// add here if else for other touch phases
		
		// one loop for scene only, another loop for global
		int maxLoops = 2;
		for (int k=0; k < maxLoops; ++k) {
			
			// is scene only list loop
			if (k==0)
				list = listEternal;
			// or eternals list loop
			else
				list = listSceneOnly;
			
			for (int i=0; list != null && i < list.Count; ++i) {
				
				ITouchListener listener = list[i];
				if (listener == null)
					continue;
				
				GameObject go = listener.getGameObject();
				bool hitten = false;
				
				// check for GUI Texture
				if (go.guiTexture != null && go.guiTexture.HitTest(t.position))
					hitten = true;
				// check for GUI Text
				else if (go.guiText != null && go.guiText.HitTest(t.position))
					hitten = true;
				// check for game object
				else {
					// use detection as in chipmunk platformer, since here I don't use physx colliders
				}
				
				// hitten? then invoke callback
				if (hitten) {
					if (TouchPhase.Began.Equals(t.phase))
						list[i].OnBeganTouch(t);
					else if (TouchPhase.Stationary.Equals(t.phase) || TouchPhase.Moved.Equals(t.phase))
						list[i].OnStationaryTouch(t);
					else if (TouchPhase.Ended.Equals(t.phase))
						list[i].OnEndedTouch(t);
					// add here if else for other methods depending on touch phases
					
					if (!ALLOW_TOUCH_HITS_OVERLAPPED_OBJECTS) {
						k = maxLoops;
						break;
					}
				}
			}
		}
	}
}
