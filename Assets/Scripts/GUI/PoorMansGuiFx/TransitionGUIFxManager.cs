using UnityEngine;

/// <summary>
/// This class is intended to register listeners for post transitions effects.
/// The goal is to attach your ITransitionListener script at the end of a current transition effect.
/// </summary>
public class TransitionGUIFxManager {
	
	private static TransitionGUIFxManager instance = null;

	public static TransitionGUIFxManager Instance {
        get {
            if (instance == null) {
				// creates the innner instance
				instance = new TransitionGUIFxManager();
			}
            return instance;
        }
    }

	private TransitionGUIFxManager () {
	}
	
	~TransitionGUIFxManager () {
		instance = null;
	}
	
	public void registerForEndTransition (ITransitionListener listener) {
		// get transitions in the order specify by its implementor
		TransitionGUIFx[] arr = listener.getTransitions();
		if (arr == null)
			return;
		// TODO get the transition with latest priority because it will be the last one
		TransitionGUIFx last = arr[arr.Length-1];
		// add the transition listener
		last.addNextTransition(listener);
	}

	public static TransitionGUIFx[] getTransitionsInOrder (GameObject go, bool inChildren) {
		if (inChildren)
			return go.GetComponentsInChildren<TransitionGUIFx>();
		return go.GetComponents<TransitionGUIFx>();
	}
}
