using UnityEngine;
using System.Collections;

/// <summary>
/// This script intended for updating the status of touch events comunicating them to the Touch Event Manager.
/// </summary>
public class TouchEventUnityUpdater : MonoBehaviour {
	
	private static bool created = false;
	
	private TouchEventManager tm;
	
	void Awake () {
		if (!created) {
			// force to activate multi touch support
			Input.multiTouchEnabled = true;
			DontDestroyOnLoad(gameObject);
			created = true;
		}
		else {
			Destroy(gameObject); // duplicate will be destroyed if 'first' scene is reloaded
		}
	}

	void Update () {
		if (tm == null)
			tm = TouchEventManager.Instance;
		tm.processEvents();
	}

}
