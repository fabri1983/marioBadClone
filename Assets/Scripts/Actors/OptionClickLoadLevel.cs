using UnityEngine;

public class OptionClickLoadLevel : TouchListenerAbs {
	
	// index of the scene to be loaded
	public int sceneIndex;

	void Awake () {
		bool isScenelOnly = true;
		bool propagateAllPhases = false;
		InputTouchManager.Instance.register(this, isScenelOnly, propagateAllPhases, TouchPhase.Began, TouchPhase.Ended);
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
	
	public override GameObject getGameObject () {
		return gameObject;
	}
	
	public override void OnBeganTouch (Touch t) {
		optionSelected();
	}
	
	public override void OnStationaryTouch (Touch t) {}
	
	public override void OnEndedTouch (Touch t) {
		optionSelected();
	}
	
	private void optionSelected() {
		LevelManager.Instance.loadLevel(sceneIndex);
	}
}
