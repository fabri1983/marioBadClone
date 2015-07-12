using UnityEngine;
using System.Collections.Generic;

public class ListenerLists {

	public List<ITouchListener> beganListeners;
	public List<ITouchListener> stationaryListeners;
	public List<ITouchListener> endedListeners;
	
	public void add (ITouchListener listener, TouchPhase[] touchPhases) {
		
		for (int i=0; i < touchPhases.Length; ++i) {
			TouchPhase phase = touchPhases[i];
			if (TouchPhase.Began.Equals(phase)) {
				if (beganListeners == null)
					beganListeners = new List<ITouchListener>(3);
				beganListeners.Add(listener);
			}
			// stationary and moved seems to be very related
			else if (TouchPhase.Stationary.Equals(phase) || TouchPhase.Moved.Equals(phase)) {
				if (stationaryListeners == null)
					stationaryListeners = new List<ITouchListener>(3);
				stationaryListeners.Add(listener);
			}
			else if (TouchPhase.Ended.Equals(phase) || TouchPhase.Canceled.Equals(phase)) {
				if (endedListeners == null)
					endedListeners = new List<ITouchListener>(3);
				endedListeners.Add(listener);
			}
			// add here else if for other phases
		}
	}
	
	public void remove (ITouchListener listener) {
		
		int id = listener.GetHashCode();
		
		if (beganListeners != null)
			for (int i=0, c=beganListeners.Count; i < c; ++i) {
				/*if (beganListeners[i] == null)
					continue;*/
				if (id == beganListeners[i].GetHashCode()) {
					//beganListeners[i] = null;
					beganListeners.RemoveAt(i);
					break;
				}
			}
	
		if (stationaryListeners != null)
			for (int i=0, c=stationaryListeners.Count; i < c; ++i) {
				/*if (stationaryListeners[i] == null)
					continue;*/
				if (id == stationaryListeners[i].GetHashCode()) {
					//stationaryListeners[i] = null;
					stationaryListeners.RemoveAt(i);
					break;
				}
			}
		
		if (endedListeners != null)
			for (int i=0, c=endedListeners.Count; i < c; ++i) {
				/*if (endedListeners[i] == null)
					continue;*/
				if (id == endedListeners[i].GetHashCode()) {
					//endedListeners[i] = null;
					endedListeners.RemoveAt(i);
					break;
				}
			}
		// add here if else for other traverses for different touch phases
	}
	
	public void clear () {
		if (beganListeners != null) beganListeners.Clear();
		if (stationaryListeners != null) stationaryListeners.Clear();
		if (endedListeners != null) endedListeners.Clear();
	}
}
