using UnityEngine;
using System.Collections;

public class NonDestroyable : MonoBehaviour {
	
	private static NonDestroyable instance = null;
	
	void Awake () {
		if (instance != null && instance != this) {
			Destroy(gameObject);
		}
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
}
