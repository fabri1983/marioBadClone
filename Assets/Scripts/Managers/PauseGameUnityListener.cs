using UnityEngine;

/// <summary>
/// This script repsonsible for listening Unity's OnApplicationPause() event.
/// It lets know the Pause Game Manager when to pause or resume.
/// </summary>
public class PauseGameUnityListener : MonoBehaviour {

	void Awake () {
		DontDestroyOnLoad(gameObject);
	}

	/**
	 * Fired by Unity when the app is going to or coming from background.
	 */
	void OnApplicationPause(bool pauseStatus) {
		// is app going into background?
		if (pauseStatus)
			PauseGameManager.Instance.pause();
		// app is brought back to foreground
		else
			PauseGameManager.Instance.resume();
	}
}
