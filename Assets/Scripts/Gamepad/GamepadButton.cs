using UnityEngine;
using System.Collections;

public class GamepadButton : MonoBehaviour, ITouchListener, ITransitionListener {
	
	// this modified in inspector window
	public string buttonLabel = "A";
	public bool keepAlive = true;
	public bool isStaticRuntime = true;
	
	void Awake () {
		if (keepAlive)
			// keep this game object alive between scenes
			DontDestroyOnLoad(this.gameObject);
		TransitionGUIFxManager.Instance.register(this, false);
	}
	
	void OnDestroy () {
		//TransitionGUIFxManager.Instance.remove(this);
	}
	
	/**
	 * This only fired on PC
	 */
	void OnMouseDown () {
		optionSelected();
	}
	
	public bool isStatic () {
		return isStaticRuntime;
	}
	
	public GameObject getGameObject () {
		return gameObject;
	}
	
	public Rect getScreenBoundsAA () {
		// this method called only once since its a non destroyable game object
		return guiTexture.GetScreenRect(Camera.main);
	}
	
	public TransitionGUIFx[] getTransitions () {
		return GetComponents<TransitionGUIFx>();
	}
	
	public void onEndTransition (TransitionGUIFx fx) {
		// register with touch event manager once the transition finishes since the manager
		// depends on final element's position
		TouchEventManager.Instance.register(this, TouchPhase.Began, TouchPhase.Stationary, TouchPhase.Ended);
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
		Debug.Log(buttonLabel);
#endif
		// set the pressed button on GamepadInput manager
		if ("A".Equals(buttonLabel))
			Gamepad.fireButton(Gamepad.BUTTONS.A);
		else if ("B".Equals(buttonLabel))
			Gamepad.fireButton(Gamepad.BUTTONS.B);
	}
}
