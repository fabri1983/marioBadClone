using UnityEngine;
using System.Collections;

public class GamepadButton : MonoBehaviour, ITouchListener {
	
	// this modified in inspector window
	public string buttonLabel = "A";
	public bool keepAlive = true;
		
	void Awake () {
		if (keepAlive)
			// keep this game object alive between scenes
			DontDestroyOnLoad(this.gameObject);

		TouchEventManager.Instance.register(this, TouchPhase.Began, TouchPhase.Stationary, TouchPhase.Ended);
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
	
	public GameObject getGameObject () {
		return gameObject;
	}
	
	public Rect getScreenBoundsAA () {
		// this method called only once since its a non destroyable game object
		return guiTexture.GetScreenRect(Camera.main);
	}
	
	public void OnBeganTouch (Touch t) {
		optionSelected();
	}
	
	public void OnStationaryTouch (Touch t) {
		optionSelected();
	}
	
	public void OnEndedTouch (Touch t) {
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
