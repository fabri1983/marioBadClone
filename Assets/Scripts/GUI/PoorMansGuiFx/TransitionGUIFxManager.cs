using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class usefull to register for pre and post transitions events
/// </summary>
public class TransitionGUIFxManager : MonoBehaviour {

	private List<TransitionGUIFx> transitions = new List<TransitionGUIFx>();
	
	private static TransitionGUIFxManager instance = null;

	public static TransitionGUIFxManager Instance {
        get {
            if (instance == null) {
				instance = FindObjectOfType(typeof(TransitionGUIFxManager)) as TransitionGUIFxManager;
				if (instance == null) {
					// creates a game object with this script component
					instance = new GameObject("TransitionGUIFxManager").AddComponent<TransitionGUIFxManager>();
				}
			}
            return instance;
        }
    }
	
	void Awake () {
		if (instance != null && instance != this)
			Destroy(this.gameObject);
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
	
	void Start () {
		for (int i=0, c=transitions.Count; i<c; ++i)
			transitions[i].enabled = true;
	}
	
	void OnDestroy () {
		transitions.Clear();
		instance = null;
	}
	
	public void register (ITransitionListener fx, bool managerDoStart) {
		TransitionGUIFx[] arr = fx.getTransitions();
		if (arr == null)
			return;
		for (int i=0, c=arr.Length; i<c; ++i) {
			arr[i].setListener(fx);
			// wheter if the caller chooses to let the manager start its transitions scripts
			if (managerDoStart)
				transitions.Add(arr[i]);
		}
	}
	
	public void remove (ITransitionListener fx) {
		TransitionGUIFx[] arr = fx.getTransitions();
		if (arr == null)
			return;
		for (int i=0, c=arr.Length; i<c; ++i)
			transitions.Remove(arr[i]);
	}
}
