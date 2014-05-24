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
	
	// controls what touch faces has been fired in current gameloop
	private static bool[] phasesFired = new bool[System.Enum.GetValues(typeof(TouchPhase)).Length];
	
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
			// creates a game object with this script component.
			instance = new GameObject("InputTouchManager").AddComponent<InputTouchManager>();
			DontDestroyOnLoad(instance);
		}
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
	/// <param name='propagateOtherPhases'>
	/// True for propagating the event in every touch phase added. False for stop on first phase.
	/// </param>
	/// <param name='touchPhases'>
	/// All the touch phases you want your gameobject be registered to.
	/// </param>
	public void register (ITouchListener listener, bool isSceneOnly, bool propagateOtherPhases, params TouchPhase[] touchPhases) {

		listener.setPropagationFlag(propagateOtherPhases);
		
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
			else if (TouchPhase.Ended.Equals(phase)) {
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
	
	void OnDestroy () {
		beganEternalListeners.Clear();
		beganSceneOnlyListeners.Clear();
		stationaryEternalListeners.Clear();
		stationarySceneOnlyListeners.Clear();
		endedEternalListeners.Clear();
		endedSceneOnlyListeners.Clear();
		instance = null;
	}
	
	void Update () {
		processEvents();
	}
	
	private static void processEvents () {
		
		if (Input.touchCount == 0)
			return;
		
		// reset phases fired
		for (int i=0; i < phasesFired.Length; ++i)
			phasesFired[i] = false;
		
		for (int i=0; i < Input.touchCount; ++i) {
			Touch touch = Input.touches[i];
#if UNITY_EDITOR
			if (Debug.isDebugBuild)
				Debug.Log(Input.touchCount + ": " + touch.phase + " -> finger " + touch.fingerId);
#endif
			// not supported yet
			if (TouchPhase.Canceled.Equals(touch.phase))
				continue;
			
			phasesFired[(int)touch.phase] = true;
		}
		
		// reset the processed flag of every listener only for pahses fired
		// NOTE: now the reset happens right after the stopPropagation() is consumed. See explanation inside sendEvent()
		//resetProcessedFlags();
		
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
		else if (TouchPhase.Ended.Equals(t.phase)) {
			listEternal = endedEternalListeners;
			listSceneOnly = endedSceneOnlyListeners;
		}
		// add here if else for other touch phases
		
		// one loop for scene only, another loop for global
		for (int k=0; k < 2; ++k) {
			
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
				
				// does the listener want to stop its propagation on next touch phases?
				if (listener.stopPropagation()) {
					/// reset here instead of calling resetProcessedFlags(). Only valid for two list (eternal and sceneOnly) 
					/// per touch phase. If adding a new list, then this reset 
					listener.setConsumed(false);
					continue;
				}
				
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
					// option 1:
					/*Ray vRay = Camera.main.ScreenPointToRay (t.position);
					RaycastHit vHit;
					if (Physics.Raycast(vRay, out vHit, 50)) {
						if (vHit.collider != null && vHit.collider.name.Equals(go.collider.name))
							hitten = true;
					}*/
					// option 2:
					Vector3 origin = listener.getGameObject().transform.InverseTransformPoint(vRay.origin);
					Vector3 direction = listener.getGameObject().transform.InverseTransformDirection(vRay.direction);
					
					Vector3 zeroCross = origin - direction*(origin.z/direction.z);
					hitten = zeroCross.magnitude < 0.5f;
				}
				
				// hitten? then invoke callback
				if (hitten) {
					// touch event is consumed
					listener.setConsumed(true);
					
					if (TouchPhase.Began.Equals(t.phase))
						list[i].OnBeganTouch(t);
					else if (TouchPhase.Stationary.Equals(t.phase) || TouchPhase.Moved.Equals(t.phase))
						list[i].OnStationaryTouch(t);
					else if (TouchPhase.Ended.Equals(t.phase))
						list[i].OnEndedTouch(t);
					// add here if else for other methods depending on touch phases
				}
			}
		}
	}
	
	/*private static void resetProcessedFlags () {
		
		// only reset the listeners for current fired touch phases
		for (int k = 0; k < phasesFired.Length; ++k) {
			
			if (phasesFired[k] == false)
				continue;
			
			TouchPhase touchPhase = (TouchPhase) k;
			
			if (TouchPhase.Began.Equals(touchPhase)) {
				for (int i=0; i < beganSceneOnlyListeners.Count; ++i) {
					if (beganSceneOnlyListeners[i] == null)
						continue;
					beganSceneOnlyListeners[i].setConsumed(false);
				}
				for (int i=0; i < beganEternalListeners.Count; ++i) {
					beganEternalListeners[i].setConsumed(false);
				}
			}
			else if (TouchPhase.Stationary.Equals(touchPhase) || TouchPhase.Moved.Equals(touchPhase)) {
				for (int i=0; i < stationarySceneOnlyListeners.Count; ++i) {
					if (stationarySceneOnlyListeners[i] == null)
						continue;
					stationarySceneOnlyListeners[i].setConsumed(false);
				}
				for (int i=0; i < stationaryEternalListeners.Count; ++i) {
					stationaryEternalListeners[i].setConsumed(false);
				}
			}
			else if (TouchPhase.Ended.Equals(touchPhase)) {
				for (int i=0; i < endedSceneOnlyListeners.Count; ++i) {
					if (endedSceneOnlyListeners[i] == null)
						continue;
					endedSceneOnlyListeners[i].setConsumed(false);
				}
				for (int i=0; i < endedEternalListeners.Count; ++i) {
					endedEternalListeners[i].setConsumed(false);
				}
			}
		}
	}*/
}
