using UnityEngine;
using System.Collections;

public class FPSFixed : MonoBehaviour {
	
	public int targetFPS = 30;
	
	void Awake () {
		// doesn't work in editor mode
		Application.targetFrameRate = targetFPS; // -1 is the default
	}
}
