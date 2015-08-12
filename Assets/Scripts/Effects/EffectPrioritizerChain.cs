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
	
	void Awake () {
		// take all the Effects
		chain = gameObject.GetComponents<Effect>();
		// sort and chain
		EffectPrioritizerHelper.sortAndChain(chain);
	}
	
	void Start () {
		// execute first thus propagating the chain effect
		chain[0].startEffect();
		// then disable this script
		this.enabled = false;
	}
}
