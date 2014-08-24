using UnityEngine;

public class StartLevel : MonoBehaviour {
	
	public int sceneIndex; // index of the scene to be loaded
	public bool enablePlayer = true;
	public Camera inFrontCam;
	public Transform min, max; // min and max extent of the level
	
	void Start () {
		// setup current level
		
		CameraManager.Instance.setInFrontCam(inFrontCam);
		// add here any new camera the manager needs to keep
		
		Rect levelExtent = new Rect(min.position.x, min.position.y, max.position.x, max.position.y);
		LevelManager.Instance.startLevel(sceneIndex, enablePlayer, levelExtent);
	}
}
