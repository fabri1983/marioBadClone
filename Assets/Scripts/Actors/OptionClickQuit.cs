using UnityEngine;

public class OptionClickQuit : InputTouchListenerAbs {
	
	void Awake () {
		// keep this game object alive between scenes
		DontDestroyOnLoad(this.gameObject);
		bool isScenelOnly = false;
		bool propagateAllPhases = false;
		InputTouchManager.Instance.register(this, isScenelOnly, propagateAllPhases, TouchPhase.Began, TouchPhase.Ended);
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
		// Quit() doesn't work in editor mode
		Application.Quit();
	}
}
