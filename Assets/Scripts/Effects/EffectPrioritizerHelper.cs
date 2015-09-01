using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// It sorts and chain Effect scripts, and start the chain once is sorted.
/// It also exposes the functionallity to register listeners at the end of an Effect chain.
/// Once the efect chain has finished then the listeners callback is invoked.
/// </summary>
public static class EffectPrioritizerHelper {
	
	private sealed class PriorityComparator : IComparer<Effect> {
		int IComparer<Effect>.Compare(Effect a, Effect b) {
			int vaPrio = a.priority;
			int vbPrio = b.priority;
			if (vaPrio < vbPrio)
				return -1;
			else if (vaPrio > vbPrio)
				return 1;
			return 0;
		}
	}
	
	private static PriorityComparator priorityComp = new PriorityComparator();
	
	public static int sortAndChain (Effect[] arr) {
		Array.Sort(arr, priorityComp);
		for (int i=0, c=arr.Length; i < (c-1); ) {
			// skip if current effect has set beforeLoadNextScene + true
			if (arr[i].beforeLoadNextScene) {
				++i;
				continue;
			}
			// look for the next effect with beforeLoadNextScene = false
			for (int j=i+1; j < c; ++j) {
				if (arr[j].beforeLoadNextScene)
					continue;
				arr[i].setNextEffect(arr[j]);
				i = j;
				break;
			}
		}
		// traverse again and get the index of the first effect with beforeLoadNextScene = false
		for (int i=0, c=arr.Length; i < c; ++i) {
			if (!arr[i].beforeLoadNextScene)
				return i;
		}
		return -1; // no effect with eforeLoadNextScene = false
	}
	
	/// <summary>
	/// Registers the listener to be executed at the end of all effects the game object contains.
	/// </summary>
	/// <param name="listener">IEffectListener instance</param>
	public static void registerAsEndEffect (IEffectListener listener) {
		Effect[] arr = listener.getEffects();
		if (arr == null)
			return;
		// get the Effect with less priority. It may happens all of them have same priority
		int priority = -1;
		Effect last = null;
		for (int i=0,c=arr.Length; i<c; ++i) {
			if (priority < arr[i].priority && !arr[i].beforeLoadNextScene) {
				priority = arr[i].priority;
				last = arr[i];
			}
		}
		// add the listener
		if (last != null)
			last.addNextListener(listener);
	}

}
