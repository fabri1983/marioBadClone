using UnityEngine;
using System.Collections;

public class FPSFixed : MonoBehaviour {
	
	public int targetFPS = 60;
	
	void Awake () {
		// doesn't work in Editor Mode
		Application.targetFrameRate = targetFPS; // -1 is the default
	}
}
