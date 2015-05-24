using UnityEngine;

public class OptionLoadLevel : MonoBehaviour, ITouchListener, IEffectListener {
	
	public int sceneIndex; // index of the scene to be loaded
	
	private bool selected = false;
	private Rect _screenBounds; // cache for the screen bounds this game object covers
	
	void Awake () {
		_screenBounds.x = -1f; // initialize the screen bounds cache
		EffectPrioritizer.registerForEndEffect(this);
	}
	
	void OnDestroy () {
		TouchEventManager.Instance.removeListener(this);
	}
	
	void Update () {
		optionSelected();
	}
	
	public bool isStatic () {
		// for event touch listener
		return true;
	}
	
	public GameObject getGameObject () {
		return gameObject;
	}
	
	public Rect getScreenBoundsAA () {
		// checks if the cached size has changed
		if (_screenBounds.x == -1f)
			_screenBounds = GUIScreenLayoutManager.getPositionInScreen(GetComponent<GUICustomElement>());
		return _screenBounds;
	}
	
	public void OnBeganTouch (Touch t) {
		doAction();
	}
	
	public void OnStationaryTouch (Touch t) {}
	
	public void OnEndedTouch (Touch t) {}
	
	public void setSelected (bool value) {
		selected = value;
	}
	
	private void optionSelected() {
		if (selected && Gamepad.isA())
			doAction();
	}
	
	private void doAction () {
		if (!PauseGameManager.Instance.isPaused())
			LevelManager.Instance.loadLevel(sceneIndex);
	}
	
	public Effect[] getEffects () {
		// return the transitions in an order set from Inspector.
		// Note: to return in a custom order get the transitions array and sort it as desired.
		return EffectPrioritizer.getEffects(gameObject, false);
	}
	
	public void onLastEffectEnd () {
		// register with touch event manager once the transition finishes since the manager
		// depends on final element's position
		TouchEventManager.Instance.register(this, TouchPhase.Began);
	}

#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER
	void OnGUI () {
		// since this game object has a GUICustomElement script attached to it, for strange a reason no mouse event 
		// is caught, so we need to manually check for the event and fire it here
		Event e = Event.current;
		if (e != null && e.isMouse && e.button == 0 && e.type == EventType.MouseUp) {
			if (GameObjectTools.testHitFromMousePos(transform, e.mousePosition))
				doAction();
		}
	}
#endif
}
