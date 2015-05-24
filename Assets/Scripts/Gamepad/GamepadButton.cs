using UnityEngine;
using System.Collections;

public class GamepadButton : MonoBehaviour, ITouchListener, IEffectListener {
	
	// this modified in inspector window
	public EnumButton buttonId = EnumButton.A;
	public bool dontDestroy = true; // true for keeping alive between scenes
	public bool isStaticRuntime = true;
	
	void Awake () {
		// should we keep this game object alive between scenes
		if (dontDestroy)
			DontDestroyOnLoad(this.gameObject);
		
		EffectPrioritizerHelper.registerForEndEffect(this);
	}
	
	void OnDestroy () {
		TouchEventManager.Instance.removeListener(this);
	}
	
	public bool isScreenStatic () {
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
			return GUIScreenLayoutManager.getPositionInScreen(GetComponent<GUICustomElement>());
	}
	
	public Effect[] getEffects () {
		// return the transitions in an order set from Inspector.
		// Note: to return in a custom order get the transitions array and sort it as desired.
		return EffectPrioritizerHelper.getEffects(gameObject, false);
	}
	
	public void onLastEffectEnd () {
		// register with touch event manager once the effect finishes since the touch
		// event depends on final element's position
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
		Debug.Log(buttonId.ToString());
#endif
		// set the pressed button on GamepadInput manager
		if (buttonId == EnumButton.A)
			Gamepad.fireButton(EnumButton.A);
		else if (buttonId == EnumButton.B)
			Gamepad.fireButton(EnumButton.B);
	}

#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER
	void OnGUI () {
		// since this game object has a GUICustomElement script attached to it, for strange a reason no mouse event 
		// is caught, so we need to manually check for the event and fire it here
		Event e = Event.current;
		if (e != null && e.isMouse && e.button == 0 && e.type == EventType.mouseDown) {
			if (GameObjectTools.testHitFromMousePos(transform, e.mousePosition))
				optionSelected();
		}
	}
#endif
}
