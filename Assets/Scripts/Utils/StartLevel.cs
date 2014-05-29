using UnityEngine;

public class StartLevel : MonoBehaviour {
	
	// index of the scene to be loaded
	public int sceneIndex;
	public bool enablePlayer = true;
	public GameObject mainCam;
	public GameObject camInFront;
	
	void Awake () {
		
		LevelManager.Instance.setMainCamera(mainCam);
		LevelManager.Instance.setCamerainFront(camInFront);
		LevelManager.Instance.startLevel(sceneIndex, enablePlayer);
	}
}
