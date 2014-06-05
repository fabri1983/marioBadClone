using UnityEngine;
using System.Collections.Generic;

public sealed class ListenerLists {

	public List<ITouchListener> beganListeners = new List<ITouchListener>(3);
	public List<ITouchListener> stationaryListeners = new List<ITouchListener>(3);
	public List<ITouchListener> endedListeners = new List<ITouchListener>(3);

	
	public void add (ITouchListener listener, TouchPhase[] touchPhases) {
		for (int i=0; i < touchPhases.Length; ++i) {
			TouchPhase phase = touchPhases[i];
			if (TouchPhase.Began.Equals(phase)) {
				beganListeners.Add(listener);
			}
			// stationary and moved seems to be very related
			else if (TouchPhase.Stationary.Equals(phase) || TouchPhase.Moved.Equals(phase)) {
				stationaryListeners.Add(listener);
			}
			else if (TouchPhase.Ended.Equals(phase) || TouchPhase.Canceled.Equals(phase)) {
				endedListeners.Add(listener);
			}
			// add here else if for other phases
		}
	}
	
	public bool remove (ITouchListener listener) {
		
		int id = listener.GetHashCode();
		bool removed = false;
		
		for (int i=0, c=beganListeners.Count; i < c; ++i) {
			/*if (beganListeners[i] == null)
				continue;*/
			if (id == beganListeners[i].GetHashCode()) {
				//beganListeners[i] = null;
				beganListeners.RemoveAt(i);
				removed = true;
			}
		}
	
		for (int i=0, c=stationaryListeners.Count; i < c; ++i) {
			/*if (stationaryListeners[i] == null)
				continue;*/
			if (id == stationaryListeners[i].GetHashCode()) {
				//stationaryListeners[i] = null;
				stationaryListeners.RemoveAt(i);
				removed = true;
			}
		}
	
		for (int i=0, c=endedListeners.Count; i < c; ++i) {
			/*if (endedListeners[i] == null)
				continue;*/
			if (id == endedListeners[i].GetHashCode()) {
				//endedListeners[i] = null;
				endedListeners.RemoveAt(i);
				removed = true;
			}
		}
		
		// add here if else for other traverses for different touch phases
		
		return removed;
	}
	
	public void clear () {
		beganListeners.Clear();
		stationaryListeners.Clear();
		endedListeners.Clear();
	}
}
