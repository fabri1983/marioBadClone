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
	
	public static void sortAndChain (Effect[] arr) {
		Array.Sort(arr, priorityComp);
		for (int i=0,c=arr.Length; i < (c-1) && c > 1; ++i)
			arr[i].setNextEffect(arr[i+1]);
	}
	
	public static void registerForEndEffect (IEffectListener listener) {
		Effect[] arr = listener.getEffects();
		if (arr == null)
			return;
		// get the Effect with less priority
		int priority = -1;
		Effect last = null;
		for (int i=0,c=arr.Length; i<c; ++i) {
			if (priority < arr[i].priority) {
				priority = arr[i].priority;
				last = arr[i];
			}
		}
		// add the listener
		if (last != null)
			last.addNextListener(listener);
	}

}
