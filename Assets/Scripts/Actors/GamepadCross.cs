using UnityEngine;

public class GamepadCross : MonoBehaviour, ITouchListener {
	
	public bool isSceneOnly = false;
	
	/// Defines the screen position and dimension (width/height) of every arrow in the cross,
	/// relative to the GUI texture with size 64x64. Scale adjustments are apply once the 
	/// game object awakes.
	private static Rect[] arrowRects = new Rect[]{
		new Rect(16f, 38f, 30f, 24f), // UP
		new Rect(42f, 16f, 22f, 28f), // RIGHT
		new Rect(16f, 0f, 30f, 20f), // DOWN
		new Rect(0, 16f, 22f, 28f) // LEFT
	};
	
	// absolute screen position of gui
	private static Vector2 guiPos;
	// auxiliar
	private Vector2 vec2 = new Vector2();
	
	void Awake () {
		// keep this game object alive between scenes
		DontDestroyOnLoad(this.gameObject);

		InputTouchManager.Instance.register(this, isSceneOnly, TouchPhase.Began, TouchPhase.Stationary);
		
		Rect guiRect = guiTexture.GetScreenRect();
		guiPos = new Vector2(guiRect.x, guiRect.y);
		
		// calculate scaling if current GUI texture dimension is diferent than 64x64
		float scaleW = guiRect.width / 64f;
		float scaleH = guiRect.height / 64f;
		// scale according target resolution (maybe only necessary when using unity remote, I don't know how to detect when going on remote)
		scaleW *= 1f;
		scaleH *= 1f;
		// sacle the array of arrows because they were defined in a 64x64 basis
		for (int i=0; i < arrowRects.Length ; ++i) {
			Rect r = arrowRects[i];
			arrowRects[i].Set(r.x * scaleW, r.y * scaleH, r.width * scaleW, r.height * scaleH);
		}
	}
	
	/**
	 * This only fired on PC
	 */
	void OnMouseUpAsButton () {
		vec2.Set(Input.mousePosition.x, Input.mousePosition.y);
		optionSelected(vec2);
	}
	
	/**
	 * This only fired on PC
	 */
	void OnMouseDrag () {
		vec2.Set(Input.mousePosition.x, Input.mousePosition.y);
		optionSelected(vec2);
	}
	
	public GameObject getGameObject () {
		return gameObject;
	}
	
	public void OnBeganTouch (Touch t) {
		optionSelected(t.position);
	}
	
	public void OnStationaryTouch (Touch t) {
		optionSelected(t.position);
	}
	
	public void OnEndedTouch (Touch t) {}
	
	private static void optionSelected(Vector2 pos) {
		/*if (Debug.isDebugBuild)
			Debug.Log(pos + " -- " + guiPos);*/
		
		// up?
		if (arrowRects[0].Contains(pos - guiPos)) {
#if UNITY_EDITOR
			if (Debug.isDebugBuild)
				Debug.Log("up");
#endif
			Gamepad.fireButton(Gamepad.BUTTONS.UP);
		}
		// right?
		if (arrowRects[1].Contains(pos - guiPos)) {
#if UNITY_EDITOR
			if (Debug.isDebugBuild)
				Debug.Log("right");
#endif
			Gamepad.fireButton(Gamepad.BUTTONS.RIGHT);
		}
		// down?
		if (arrowRects[2].Contains(pos - guiPos)) {
#if UNITY_EDITOR
			if (Debug.isDebugBuild)
				Debug.Log("down");
#endif
			Gamepad.fireButton(Gamepad.BUTTONS.DOWN);
		}
		// left?
		if (arrowRects[3].Contains(pos - guiPos)) {
#if UNITY_EDITOR
			if (Debug.isDebugBuild)
				Debug.Log("left");
#endif
			Gamepad.fireButton(Gamepad.BUTTONS.LEFT);
		}
	}
}
