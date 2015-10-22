using UnityEngine;

public class StartLevel : MonoBehaviour {
	
	public bool enablePlayer = true;
	public Transform min, max; // min and max extent of the level
	
	void Start () {
		// NOTE: this setups current level. Do this only in Start() since all scripts use Awake() to initialize
		
		// create Rect for level extent used for parallax effects
		Rect levelExtent = new Rect(min.position.x, min.position.y, max.position.x, max.position.y);
		
		// setup current level
		LevelManager.Instance.startLevel(Application.loadedLevel, enablePlayer, levelExtent);
	}
}
