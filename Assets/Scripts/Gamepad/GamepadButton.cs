using UnityEngine;
using System.Collections;

public class GamepadButton : MonoBehaviour, ITouchListener, ITransitionListener {
	
	// this modified in inspector window
	public string buttonLabel = "A";
	public bool keepAlive = true;
	public bool isStaticRuntime = true;
	
	void Awake () {
		// should we keep this game object alive between scenes
		if (keepAlive)
			DontDestroyOnLoad(this.gameObject);
		
		TransitionGUIFxManager.Instance.register(this, false);
	}
	
	void OnDestroy () {
		TransitionGUIFxManager.Instance.remove(this);
	}
	
	public bool isStatic () {
		// for event touch listener
		return isStaticRuntime;
	}
	
	public GameObject getGameObject () {
		return gameObject;
	}
	
	public Rect getScreenBoundsAA () {
		// This method called only once if the gameobject is a non destroyable game object
		
		// if used with a Unity's GUITexture
		if (guiTexture != null)
			return guiTexture.GetScreenRect(Camera.main);
		// here I suppose this game object has attached a GUICustomElement
		else
			return GUIScreenLayoutManager.positionInScreen(GetComponent<GUICustomElement>());
	}
	
	public TransitionGUIFx[] getTransitions () {
		return GetComponents<TransitionGUIFx>();
	}
	
	public void onEndTransition (TransitionGUIFx fx) {
		// register with touch event manager once the transition finishes since the manager
		// depends on final element's position
		TouchEventManager.Instance.register(this, TouchPhase.Began, TouchPhase.Stationary);
	}
	
	public void OnBeganTouch (Touch t) {
		optionSelected();
	}
	
	public void OnStationaryTouch (Touch t) {
		optionSelected();
	}
	
	public void OnEndedTouch (Touch t) {}
	
	private void optionSelected() {
		if (PauseGameManager.Instance.isPaused())
			return;
#if UNITY_EDITOR
		Debug.Log(buttonLabel);
#endif
		// set the pressed button on GamepadInput manager
		if ("A".Equals(buttonLabel))
			Gamepad.fireButton(Gamepad.BUTTONS.A);
		else if ("B".Equals(buttonLabel))
			Gamepad.fireButton(Gamepad.BUTTONS.B);
	}

#if UNITY_STANDALONE || UNITY_EDITOR
	void OnGUI () {
		// since this game object has a GUICustomElement script attached to it, for strange a reason no mouse event 
		// is caught, so we need to manually check for the event and fire it here
		Event e = Event.current;
		if (e != null && e.isMouse && e.button == 0 && e.type == EventType.MouseUp) {
			if (GameObjectTools.testHitFromMousePos(transform, e.mousePosition))
				optionSelected();
		}
	}
#endif
}
