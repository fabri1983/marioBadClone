using UnityEngine;
using System.Collections;

/// <summary>
/// Only purpose of this script is acting as a flag for those game objects having Effect scripts attached 
/// and want to prioritize and chain all of them. So is applicable when the game object has more than one 
/// Effect.
/// </summary>
public class EfectPrioritizer : MonoBehaviour {

	void Awake () {
		this.enabled = false;
	}
}
