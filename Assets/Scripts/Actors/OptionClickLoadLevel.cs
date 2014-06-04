using UnityEngine;

public class OptionClickLoadLevel : MonoBehaviour, ITouchListener {
	
	// index of the scene to be loaded
	public int sceneIndex;

	void Awake () {
		bool isSceneOnly = true;
		InputTouchManager.Instance.register(this, isSceneOnly, TouchPhase.Began, TouchPhase.Ended);
	}
	
	void OnDestroy () {
		InputTouchManager.Instance.removeSceneOnlyListener(this);
	}
	
	/**
	 * This only fired on PC
	 */
	void OnMouseUpAsButton () {
		optionSelected();
	}
	
	public GameObject getGameObject () {
		return gameObject;
	}
	
	public void OnBeganTouch (Touch t) {
		optionSelected();
	}
	
	public void OnStationaryTouch (Touch t) {}
	
	public void OnEndedTouch (Touch t) {
		optionSelected();
	}
	
	private void optionSelected() {
		LevelManager.Instance.loadLevel(sceneIndex);
	}
}
