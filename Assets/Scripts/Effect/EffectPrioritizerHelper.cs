using UnityEngine;

/// <summary>
/// It sorts and chain Effect scripts, and start the chain once is sorted.
/// It also exposes the functionallity to register listeners at the end of an Effect chain.
/// Once the efect chain has finished then the listeners callback is invoked.
/// </summary>
public static class EffectPrioritizerHelper {
	
	public static void sortAndChain () {
		
	}
	
	public static void registerForEndEffect (IEffectListener listener) {
		Effect[] arr = listener.getEffects();
		if (arr == null)
			return;
		
		// TODO sort array by priority from 0 to n
		
		Effect last = arr[arr.Length-1];
		last.addNextListener(listener);
	}

	public static Effect[] getEffects (GameObject go, bool inChildren) {
		if (inChildren)
			return go.GetComponentsInChildren<Effect>();
		return go.GetComponents<Effect>();
	}
}
