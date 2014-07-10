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
	// auxiliar variables
	private Vector2 vec2;
	
	void Awake () {
		if (keepAlive) {
			// keep this game object alive between scenes
			DontDestroyOnLoad(this.gameObject);
		}
		
		TransitionGUIFxManager.Instance.register(this, false);
		
		// calculate scaling if current GUI texture dimension is diferent than 64x64
		float scaleW = guiTexture.pixelInset.width / 64f;
		float scaleH = guiTexture.pixelInset.height / 64f;
		// scale the array of arrows because they were defined in a 64x64 basis
		for (int i=0; i < arrowRects.Length ; ++i) {
			Rect r = arrowRects[i];
			arrowRects[i].Set(r.x * scaleW, r.y * scaleH, r.width * scaleW, r.height * scaleH);
		}
	}
	
	void OnDestroy () {
		//TransitionGUIFxManager.Instance.remove(this);
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
	}
#endif
	
	/**
	 * This only fired on PC
	 */
	void OnMouseDown () {
		vec2.Set(Input.mousePosition.x, Input.mousePosition.y);
		optionSelected(vec2);
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
		guiPos.Set(guiTexture.pixelInset.x, guiTexture.pixelInset.y);
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
