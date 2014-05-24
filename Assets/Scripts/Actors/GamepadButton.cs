using UnityEngine;
using System.Collections;

public class GamepadButton : TouchListenerAbs {
	
	// this modified in inspector window
	public string buttonLabel = "A";
	public bool isScenelOnly = false;
	public bool propagateAllPhases = false;
	
	void Awake () {
		if (!isScenelOnly)
			// keep this game object alive between scenes
			DontDestroyOnLoad(this.gameObject);

		InputTouchManager.Instance.register(this, isScenelOnly, propagateAllPhases, TouchPhase.Began, TouchPhase.Stationary, TouchPhase.Ended);
	}
	
	/**
	 * This only fired on PC
	 */
	void OnMouseUpAsButton () {
		optionSelected();
	}
	
	/**
	 * This only fired on PC
	 */
	void OnMouseDrag () {
		optionSelected();
	}
	
	public override GameObject getGameObject () {
		return gameObject;
	}
	
	public override void OnBeganTouch (Touch t) {
		optionSelected();
	}
	
	public override void OnStationaryTouch (Touch t) {
		optionSelected();
	}
	
	public override void OnEndedTouch (Touch t) {
		optionSelected();
	}
	
	private void optionSelected() {
#if UNITY_EDITOR
		if (Debug.isDebugBuild)
			Debug.Log(buttonLabel);
#endif
		
		// set the pressed button on GamepadInput manager
		if ("A".Equals(buttonLabel))
			Gamepad.fireButton(Gamepad.BUTTONS.A);
		else if ("B".Equals(buttonLabel))
			Gamepad.fireButton(Gamepad.BUTTONS.B);
	}
}
