using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Only purpose of this script is acting as a flag for those game objects having Effect scripts attached 
/// and want to prioritize and chain all of them. So is applicable when the game object has more than one 
/// Effect.
/// </summary>
public class EffectPrioritizerChain : MonoBehaviour {

	private Effect[] chain;
	private int startEffect = 0;
	
	void Awake () {
		chain = GetComponents<Effect>();
		startEffect = EffectPrioritizerHelper.sortAndChain(chain);
	}
	
	void Start () {
		// execute the chain effect
		if (startEffect >= 0)
			chain[startEffect].startEffect();
		// then disable this script
		this.enabled = false;
	}
}
