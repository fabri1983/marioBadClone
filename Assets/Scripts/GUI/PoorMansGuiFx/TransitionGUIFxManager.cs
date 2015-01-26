using UnityEngine;

/// <summary>
/// This class usefull to register for pre and post transitions GUI events
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
	
	public void registerForEndTransitions (ITransitionListener lastTransition) {
		// get transitions in the order specify by its implementor
		TransitionGUIFx[] arr = lastTransition.getTransitions();
		if (arr == null)
			return;
		// chain all the transitions
		for (int i=1, c=arr.Length; i<c; ++i)
			arr[i-1].setNextTransition(arr[i]);
		// finally add the last transition
		arr[arr.Length-1].setNextTransition(lastTransition);
	}

	public static TransitionGUIFx[] getTransitionsInOrder (GameObject go) {
		return go.GetComponents<TransitionGUIFx>();
	}
}
