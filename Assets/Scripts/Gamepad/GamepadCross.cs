using UnityEngine;

public class GamepadCross : MonoBehaviour, ITouchListener, ITransitionListener {
	
	public bool keepAlive = true;
	public bool isStaticRuntime = true;
	public bool debugZones = false;
	
	/// Defines the screen position and dimension (width/height) of every arrow in the cross,
	/// relative to the GUI texture of size 64x64. Scale adjustments are apply once the 
	/// game object awakes.
	private static Rect[] arrowRects = new Rect[]{
		new Rect(20f, 40f, 24f, 22f), // UP
		new Rect(42f, 20f, 22f, 24f), // RIGHT
		new Rect(20f, 0f, 24f, 22f), // DOWN
		new Rect(0f, 20f, 22f, 24f), // LEFT
		
		new Rect(48f, 46f, 16f, 16f), // UP-RIGHT
		new Rect(0f, 46f, 16f, 16f), // UP-LEFT
		new Rect(48f, 0f, 16f, 16f), // DOWN-RIGHT
		new Rect(0f, 0f, 16f, 16f) // DOWN-LEFT
	};
	
	// absolute screen position of gui
	private static Vector2 guiPos;
	
	void Awake () {
		if (keepAlive) {
			// keep this game object alive between scenes
			DontDestroyOnLoad(this.gameObject);
		}
		
		TransitionGUIFxManager.Instance.register(this, false);
		
		// calculate scaling if current GUI texture dimension is diferent than 64x64 because
		// the array of arrows because they were defined in a 64x64 basis
		float scaleW = 1f;
		float scaleH = 1f;
		// support Unity's gui elements
		if (guiTexture != null) {
			scaleW = guiTexture.pixelInset.width / 64f;
			scaleH = guiTexture.pixelInset.height / 64f;
		}
		// assuming it has a GUICustomElement
		else {
			GUICustomElement guiElem = GetComponent<GUICustomElement>();
			Vector2 guiElemSize = guiElem.getSizeInPixels();
			scaleW = guiElemSize.x / 64f;
			scaleH = guiElemSize.y / 64f;
		}
		
		// apply the scale
		for (int i=0; i < arrowRects.Length ; ++i) {
			Rect r = arrowRects[i];
			arrowRects[i].Set(r.x * scaleW, r.y * scaleH, r.width * scaleW, r.height * scaleH);
		}
	}
	
	void OnDestroy () {
		TransitionGUIFxManager.Instance.remove(this);
	}
	
#if UNITY_EDITOR
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
		if (e != null && e.isMouse && e.button == 0 && e.type == EventType.MouseUp) {
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
	
	public bool isStatic () {
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
	
	public void OnBeganTouch (Touch t) {
		optionSelected(t.position);
	}
	
	public void OnStationaryTouch (Touch t) {
		optionSelected(t.position);
	}
	
	public void OnEndedTouch (Touch t) {}
	
	public TransitionGUIFx[] getTransitions () {
		return GetComponents<TransitionGUIFx>();
	}
	
	public void onEndTransition (TransitionGUIFx fx) {
		// register with touch event manager once the transition finishes since the manager
		// depends on final element's position
		TouchEventManager.Instance.register(this, TouchPhase.Began, TouchPhase.Stationary);
		// update current gui position cache
		if (guiTexture != null)
			guiPos.Set(guiTexture.pixelInset.x, guiTexture.pixelInset.y);
		else {
			Rect screenBounds = getScreenBoundsAA();
			guiPos.Set(screenBounds.x, screenBounds.y);
		}
	}
	
	private static void optionSelected(Vector2 pos) {
#if UNITY_EDITOR
		//Debug.Log(pos + " -- " + guiPos);
#endif
		
		// up?
		if (arrowRects[0].Contains(pos - guiPos)) {
#if UNITY_EDITOR
			Debug.Log("up");
#endif
			Gamepad.fireButton(Gamepad.BUTTONS.UP);
		}
		// right?
		else if (arrowRects[1].Contains(pos - guiPos)) {
#if UNITY_EDITOR
			Debug.Log("right");
#endif
			Gamepad.fireButton(Gamepad.BUTTONS.RIGHT);
		}
		// down?
		else if (arrowRects[2].Contains(pos - guiPos)) {
#if UNITY_EDITOR
			Debug.Log("down");
#endif
			Gamepad.fireButton(Gamepad.BUTTONS.DOWN);
		}
		// left?
		else if (arrowRects[3].Contains(pos - guiPos)) {
#if UNITY_EDITOR
			Debug.Log("left");
#endif
			Gamepad.fireButton(Gamepad.BUTTONS.LEFT);
		}
		// up-right?
		else if (arrowRects[4].Contains(pos - guiPos)) {
#if UNITY_EDITOR
			Debug.Log("up-right");
#endif
			Gamepad.fireButton(Gamepad.BUTTONS.UP);
			Gamepad.fireButton(Gamepad.BUTTONS.RIGHT);
		}
		// up-left?
		else if (arrowRects[5].Contains(pos - guiPos)) {
#if UNITY_EDITOR
			Debug.Log("up-left");
#endif
			Gamepad.fireButton(Gamepad.BUTTONS.UP);
			Gamepad.fireButton(Gamepad.BUTTONS.LEFT);
		}
		// up-left?
		else if (arrowRects[6].Contains(pos - guiPos)) {
#if UNITY_EDITOR
			Debug.Log("down-right");
#endif
			Gamepad.fireButton(Gamepad.BUTTONS.DOWN);
			Gamepad.fireButton(Gamepad.BUTTONS.RIGHT);
		}
		// up-left?
		else if (arrowRects[7].Contains(pos - guiPos)) {
#if UNITY_EDITOR
			Debug.Log("down-left");
#endif
			Gamepad.fireButton(Gamepad.BUTTONS.DOWN);
			Gamepad.fireButton(Gamepad.BUTTONS.LEFT);
		}
	}
}
