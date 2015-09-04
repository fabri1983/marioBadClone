using UnityEngine;
using System.Collections;

public class FPSFixed : MonoBehaviour {
	
	public int targetFPS = 60; // use 0 to let Unity handle the fps
	
	void Awake () {
		// doesn't work in Editor Mode
		if (targetFPS > 0f)
			Application.targetFrameRate = targetFPS; // -1 is the default
	}
}
