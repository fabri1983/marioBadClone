using UnityEngine;

public class StartLevel : MonoBehaviour {
	
	public int sceneIndex; // index of the scene to be loaded
	public bool enablePlayer = true;
	public GameObject mainCam;
	public GameObject camInFront;
	public Transform min, max; // min and max extent of the level
	
	void Start () {
		// configures current level
		LevelManager.Instance.setMainCamera(mainCam);
		LevelManager.Instance.setCamerainFront(camInFront);
		Rect levelExtent = new Rect(min.position.x, min.position.y, max.position.x, max.position.y);
		LevelManager.Instance.startLevel(sceneIndex, enablePlayer, levelExtent);
	}
}
