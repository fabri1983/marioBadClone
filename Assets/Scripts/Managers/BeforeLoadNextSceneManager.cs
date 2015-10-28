using UnityEngine;
using System.Collections.Generic;

public class BeforeLoadNextSceneManager {

	private Effect[] listeners = new Effect[10]; // set size experimentally
	private int indexFirstEmpty = 0;
	
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
		for (int i=0, c=listeners.Length; i < c; ++i)
			listeners[i] = null;
		listeners = null;
		listeners = null;
		instance = null;
	}
	
	public void register(Effect effect) {
		bool inserted = false;
		for (int i=indexFirstEmpty, c=listeners.Length; i < c; ++i) {
			if (listeners[i] == null) {
				listeners[i] = effect;
				inserted = true;
				indexFirstEmpty = i + 1; // I guess next cell is empty
				break;
			}
		}
		if (!inserted)
			Debug.LogError("listeners array out of space. Increment size one unit more.");
	}
	
	public void remove(Effect effect) {
		for (int i=0, c=listeners.Length; i < c; ++i) {
			if (effect.Equals(listeners[i])) {
				listeners[i] = null;
				if (i < indexFirstEmpty)
					indexFirstEmpty = i;
				break;
			}
		}
	}
	
	public Effect[] getEffects () {
		return listeners;
	}
	
	public void executeEffects () {
		for (int i=0,c=listeners.Length; i < c; ++i) {
			if (listeners[i] != null/* && listeners[i].beforeLoadNextScene*/) {
				listeners[i].startEffect();
			}
		}
	}
}
