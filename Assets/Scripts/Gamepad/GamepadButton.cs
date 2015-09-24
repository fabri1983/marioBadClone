using UnityEngine;
using System.Collections;

public class GamepadButton : MonoBehaviour, ITouchListener, IEffectListener {
	
	// this modified in inspector window
	public EnumButton buttonId = EnumButton.A;
	public bool isStaticRuntime = true;
	
	private GUICustomElement guiElem;
	private Rect _screenBounds; // cache for the screen bounds the GUI element covers
	
	void Awake () {
		initialize();
	}
	
	private void initialize () {
		_screenBounds.x = -1f; // initialize the screen bounds cache
		guiElem = GetComponent<GUICustomElement>();
		EffectPrioritizerHelper.registerAsEndEffect(this as IEffectListener);
	}
	
	void OnDestroy () {
		TouchEventManager.Instance.removeListener(this as ITouchListener);
	}
	
	public bool isScreenStatic () {
		// for event touch listener
		return isStaticRuntime;
	}
	
	public Rect getScreenBoundsAA () {
		// checks if the cached size has changed
		if (_screenBounds.x == -1f)
			_screenBounds = GUIScreenLayoutManager.getPositionInScreen(guiElem);
		return _screenBounds;
	}
	
	public Effect[] getEffects () {
		return GetComponents<Effect>();
	}
	
	public void onLastEffectEnd () {
		_screenBounds.x = -1f; // reset the cache variable
		
		// register with touch event manager once the effect finishes since the touch
		// event depends on final element's position
		TouchEventManager.Instance.register(this as ITouchListener, TouchPhase.Began, TouchPhase.Stationary);
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
		#if DEBUG_GAMEPAD
		Debug.Log(buttonId.ToString());
		#endif
		// set the pressed button on GamepadInput manager
		if (buttonId == EnumButton.A)
			Gamepad.Instance.fireButton(EnumButton.A);
		else if (buttonId == EnumButton.B)
			Gamepad.Instance.fireButton(EnumButton.B);
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
