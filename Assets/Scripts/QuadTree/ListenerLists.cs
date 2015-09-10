using UnityEngine;

public class ListenerLists {

	public ITouchListener[] beganListeners = new ITouchListener[8];
	public ITouchListener[] stationaryListeners = new ITouchListener[3];
	public ITouchListener[] endedListeners = new ITouchListener[2];
	
	public void add (ITouchListener listener, TouchPhase phase) {
		if (TouchPhase.Began == phase) {
			bool inserted = false;
			for (int i=0, c=beganListeners.Length; i < c; ++i) {
				if (beganListeners[i] == null) {
					beganListeners[i] = listener;
					inserted = true;
					break;
				}
			}
			if (!inserted)
				Debug.LogError("Limit of began touch listeners reached!");
		}
		else if (TouchPhase.Stationary == phase || TouchPhase.Moved == phase) {
			bool inserted = false;
			for (int i=0, c=stationaryListeners.Length; i < c; ++i) {
				if (stationaryListeners[i] == null) {
					stationaryListeners[i] = listener;
					inserted = true;
					break;
				}
			}
			if (!inserted)
				Debug.LogError("Limit of stationary touch listeners reached!");
		}
		else if (TouchPhase.Ended == phase || TouchPhase.Canceled == phase) {
			bool inserted = false;
			for (int i=0, c=endedListeners.Length; i < c; ++i) {
				if (endedListeners[i] == null) {
					endedListeners[i] = listener;
					inserted = true;
					break;
				}
			}
			if (!inserted)
				Debug.LogError("Limit of ended touch listeners reached!");
		}
	}
	
	public void remove (ITouchListener listener) {
		int id = listener.GetHashCode();
		
		for (int i=0, c=beganListeners.Length; i < c; ++i) {
			if (beganListeners[i] == null)
				continue;
			if (id == beganListeners[i].GetHashCode()) {
				beganListeners[i] = null;
				break;
			}
		}
		
		for (int i=0, c=stationaryListeners.Length; i < c; ++i) {
			if (stationaryListeners[i] == null)
				continue;
			if (id == stationaryListeners[i].GetHashCode()) {
				stationaryListeners[i] = null;
				break;
			}
		}

		for (int i=0, c=endedListeners.Length; i < c; ++i) {
			if (endedListeners[i] == null)
				continue;
			if (id == endedListeners[i].GetHashCode()) {
				endedListeners[i] = null;
				break;
			}
		}
	}
	
	public void clear () {
		for (int i=0, c=beganListeners.Length; i < c; ++i)
			beganListeners[i] = null;
		for (int i=0, c=stationaryListeners.Length; i < c; ++i)
			stationaryListeners[i] = null;
		for (int i=0, c=endedListeners.Length; i < c; ++i)
			endedListeners[i] = null;
	}
}
