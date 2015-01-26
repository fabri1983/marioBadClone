using UnityEngine;
using System.Collections;

/// <summary>
/// This script intended for updating the status of touch events comunicating them to the Touch Event Manager.
/// </summary>
public class TouchEventUnityUpdater : MonoBehaviour {
	
	void Awake () {
		DontDestroyOnLoad(gameObject);
		// force to activate multi touch support
		Input.multiTouchEnabled = true;
	}

	void Update () {
		TouchEventManager.Instance.processEvents();
	}

}
