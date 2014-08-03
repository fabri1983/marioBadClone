using UnityEngine;

public class StartLevel : MonoBehaviour {
	
	public int sceneIndex; // index of the scene to be loaded
	public bool enablePlayer = true;
	public GameObject mainCam;
	public GameObject camInFront;
	public Transform min, max; // min and max extent of the level
	
	void Start () {
		
		LevelManager.Instance.setMainCamera(mainCam);
		LevelManager.Instance.setCamerainFront(camInFront);
		LevelManager.Instance.startLevel(sceneIndex, enablePlayer);
	}
}
