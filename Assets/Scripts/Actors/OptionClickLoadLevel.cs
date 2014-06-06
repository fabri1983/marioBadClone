using UnityEngine;

public class OptionClickLoadLevel : MonoBehaviour, ITouchListener {
	
	// index of the scene to be loaded
	public int sceneIndex;

	void Awake () {
		TouchEventManager.Instance.register(this, TouchPhase.Ended);
	}
	
	void OnDestroy () {
		TouchEventManager.Instance.removeListener(this);
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
	
	public Rect getScreenBoundsAA () {
		return guiText.GetScreenRect(Camera.main);
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
