using UnityEngine;

public class StartLevel : MonoBehaviour {
	
	public int sceneIndex; // index of the scene to be loaded
	public bool enablePlayer = true;
	public Transform min, max; // min and max extent of the level
	
	void Start () {
		// setup current level
		
		Rect levelExtent = new Rect(min.position.x, min.position.y, max.position.x, max.position.y);
		LevelManager.Instance.startLevel(sceneIndex, enablePlayer, levelExtent);
		
		OptionQuit.Instance.setFaderForMainCamera();
	}
}
