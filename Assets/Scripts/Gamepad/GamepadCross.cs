using UnityEngine;

public class GamepadCross : MonoBehaviour, ITouchListener, IEffectListener {
	
	public bool isStaticRuntime = true; // true if the game object never translate, even when is initialized
	public bool debugZones = false;
	
	/// Defines the screen position and dimension (width/height) of every arrow in the cross,
	/// relative to the GUI texture of size 64x64. Scale adjustments are apply once the 
	/// game object awakes.
	private Rect[] arrowRects = new Rect[]{
		new Rect(20f, 40f, 24f, 22f), // UP
		new Rect(42f, 20f, 22f, 24f), // RIGHT
		new Rect(20f, 0f, 24f, 22f), // DOWN
		new Rect(0f, 20f, 22f, 24f), // LEFT
		
		new Rect(48f, 46f, 16f, 16f), // UP-RIGHT
		new Rect(0f, 46f, 16f, 16f), // UP-LEFT
		new Rect(48f, 0f, 16f, 16f), // DOWN-RIGHT
		new Rect(0f, 0f, 16f, 16f) // DOWN-LEFT
	};
	
	private Vector2 _guiPosAux; // absolute screen position of gui
	private Rect _screenBounds; // cache for the screen bounds the GUI element covers
	private GUICustomElement guiElem;
	
	void Awake () {
		initialize();
	}

	private void initialize () {
		// calculate scaling if current GUI texture dimension is diferent than 64x64 because
		// the array of arrows were defined in a 64x64 basis
		float scaleW = 1f;
		float scaleH = 1f;

		guiElem = GetComponent<GUICustomElement>();
		Vector2 guiElemSize = guiElem.getSizeInPixels();
		scaleW = guiElemSize.x / 64f;
		scaleH = guiElemSize.y / 64f;
		
		// apply the scale
		for (int i=0; i < arrowRects.Length ; ++i) {
			Rect r = arrowRects[i];
			arrowRects[i].Set(r.x * scaleW, r.y * scaleH, r.width * scaleW, r.height * scaleH);
		}
		
		_screenBounds.x = -1f; // initialize the screen bounds cache
		
		EffectPrioritizerHelper.registerAsEndEffect(this as IEffectListener);
	}
	
	void OnDestroy () {
		TouchEventManager.Instance.removeListener(this as ITouchListener);
	}
	
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER
	void OnGUI () {
		if (debugZones && EventType.Repaint == Event.current.type) {
			// NOTE: use this with no aspect ratio modification. Set it as false in LevelManager
			for (int i=0; i < arrowRects.Length ; ++i) {
				Rect r = arrowRects[i];
				Rect rTarget = new Rect(r.x, Screen.height - r.y - r.height, r.width, r.height);
				GUI.Box(rTarget, GUIContent.none);
			}
		}
		
		// since this game object has a GUICustomElement script attached to it, for strange a reason no mouse event 
		// is caught, so we need to manually check for the event and fire it here
		Event e = Event.current;
		if (e != null && e.isMouse && e.button == 0 && e.type == EventType.MouseDown) {
			if (GameObjectTools.testHitFromMousePos(transform, e.mousePosition)) {
				Vector2 mousePosInverted;
				mousePosInverted.x = e.mousePosition.x;
				// mouse position is in GUI space which has inverted Y axis
				mousePosInverted.y = Screen.height - e.mousePosition.y;
				optionSelected(mousePosInverted);
			}
		}
	}
#endif

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
	
	public void OnBeganTouch (Touch t) {
		optionSelected(t.position);
	}
	
	public void OnStationaryTouch (Touch t) {
		optionSelected(t.position);
	}
	
	public void OnEndedTouch (Touch t) {}
	
	public Effect[] getEffects () {
		return GetComponents<Effect>();
	}
	
	public void onLastEffectEnd () {
		_screenBounds.x = -1f; // reset the cache variable
		getScreenBoundsAA(); // force the recalculation because is used 
		
		// register with touch event manager once the effect finishes since the touch
		// event depends on final element's position
		TouchEventManager.Instance.register(this as ITouchListener, TouchPhase.Began, TouchPhase.Stationary);
	}
	
	private void optionSelected(Vector2 pos) {
		// no allow interaction when game is on pause
		if (PauseGameManager.Instance.isPaused())
			return;
		
		_guiPosAux.x = pos.x - _screenBounds.x;
		_guiPosAux.y = pos.y - _screenBounds.y;
		
		#if DEBUG_GAMEPAD
		//Debug.Log(pos + " -- " + _guiPosAux);
		#endif
		
		// up?
		if (arrowRects[0].Contains(_guiPosAux)) {
			#if DEBUG_GAMEPAD
			Debug.Log("up");
			#endif
			Gamepad.Instance.fireButton(EnumButton.UP);
		}
		// right?
		else if (arrowRects[1].Contains(_guiPosAux)) {
			#if DEBUG_GAMEPAD
			Debug.Log("right");
			#endif
			Gamepad.Instance.fireButton(EnumButton.RIGHT);
		}
		// down?
		else if (arrowRects[2].Contains(_guiPosAux)) {
			#if DEBUG_GAMEPAD
			Debug.Log("down");
			#endif
			Gamepad.Instance.fireButton(EnumButton.DOWN);
		}
		// left?
		else if (arrowRects[3].Contains(_guiPosAux)) {
			#if DEBUG_GAMEPAD
			Debug.Log("left");
			#endif
			Gamepad.Instance.fireButton(EnumButton.LEFT);
		}
		// up-right?
		else if (arrowRects[4].Contains(_guiPosAux)) {
			#if DEBUG_GAMEPAD
			Debug.Log("up-right");
			#endif
			Gamepad.Instance.fireButton(EnumButton.UP);
			Gamepad.Instance.fireButton(EnumButton.RIGHT);
		}
		// up-left?
		else if (arrowRects[5].Contains(_guiPosAux)) {
			#if DEBUG_GAMEPAD
			Debug.Log("up-left");
			#endif
			Gamepad.Instance.fireButton(EnumButton.UP);
			Gamepad.Instance.fireButton(EnumButton.LEFT);
		}
		// up-left?
		else if (arrowRects[6].Contains(_guiPosAux)) {
			#if DEBUG_GAMEPAD
			Debug.Log("down-right");
			#endif
			Gamepad.Instance.fireButton(EnumButton.DOWN);
			Gamepad.Instance.fireButton(EnumButton.RIGHT);
		}
		// up-left?
		else if (arrowRects[7].Contains(_guiPosAux)) {
			#if DEBUG_GAMEPAD
			Debug.Log("down-left");
			#endif
			Gamepad.Instance.fireButton(EnumButton.DOWN);
			Gamepad.Instance.fireButton(EnumButton.LEFT);
		}
	}
}
