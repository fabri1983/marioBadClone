using UnityEngine;
using System.Collections.Generic;

public class BeforeLoadNextSceneManager {

	private List<Effect> listeners = new List<Effect>();
	
	private static BeforeLoadNextSceneManager instance = null;
	
	public static BeforeLoadNextSceneManager Instance {
		get {
			warm();
			return instance;
		}
	}
	
	public static void warm () {
		// in case the class wasn't instantiated yet from another script
		if (instance == null) {
			// creates the innner instance
			instance = new BeforeLoadNextSceneManager();
		}
	}
	
	private BeforeLoadNextSceneManager () {
	}
	
	~BeforeLoadNextSceneManager () {
		listeners.Clear();
		listeners = null;
		instance = null;
	}
	
	public void register(Effect effect) {
		listeners.Add(effect);
	}
	
	public void remove(Effect effect) {
		for (int i=0, c=listeners.Count; i < c; ++i) {
			if (effect.Equals(listeners[i])) {
				listeners[i] = null;
				break;
			}
		}
	}
	
	public List<Effect> getEffects () {
		return listeners;
	}
}
