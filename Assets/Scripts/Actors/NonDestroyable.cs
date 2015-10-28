using UnityEngine;

public class NonDestroyable : MonoBehaviour {
	
	private static bool created = false;
	
	void Awake () {
		if (!created) {
			DontDestroyOnLoad(gameObject);
			created = true;
		}
		else {
			Destroy(gameObject); // duplicate will be destroyed if 'first' scene is reloaded
		}
	}
}
