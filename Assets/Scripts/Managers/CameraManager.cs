using UnityEngine;

/**
 * This manager gives access to all the cameras apart from the main camera.
 * Any new camera you add into the scene needs to be added here as a new instance variable 
 * with its getter and setter, and exposes in StartLevel a public variable to be assigned 
 * by the inspector.
 */
public class CameraManager : MonoBehaviour {
	
	private static CameraManager instance = null;
	
	public static CameraManager Instance {
        get {
            if (instance == null) {
				// creates a game object with this script component
				instance = new GameObject("CameraManager").AddComponent<CameraManager>();
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
	
	void OnDestroy () {
		instance = null;
	}
}
