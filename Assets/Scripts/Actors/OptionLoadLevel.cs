using UnityEngine;

public class OptionLoadLevel : MonoBehaviour, ITouchListener, ITransitionListener {
	
	// index of the scene to be loaded
	public int sceneIndex;
	
	private Rect _screenBounds; // cache for the screen bounds this game object covers
	
	void Awake () {
		// initialize the screen bounds cache
		_screenBounds.x = -1f;
		
		TransitionGUIFxManager.Instance.registerForEndTransitions(this);
	}
	
	void OnDestroy () {
		TouchEventManager.Instance.removeListener(this);
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
			_screenBounds = GUIScreenLayoutManager.positionInScreen(GetComponent<GUICustomElement>());
		return _screenBounds;
	}
	
	public void OnBeganTouch (Touch t) {
		optionSelected();
	}
	
	public void OnStationaryTouch (Touch t) {}
	
	public void OnEndedTouch (Touch t) {
		optionSelected();
	}
	
	private void optionSelected() {
		if (!PauseGameManager.Instance.isPaused())
			LevelManager.Instance.loadLevel(sceneIndex);
	}
	
	public TransitionGUIFx[] getTransitions () {
		// return the transitions in an order set from Inspector.
		// Note: to return in a custom order get the transitions array and sort it as desired.
		return TransitionGUIFxManager.getTransitionsInOrder(gameObject);
	}
	
	public void prevTransitionEnd (TransitionGUIFx fx) {
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
				optionSelected();
		}
	}
#endif
}
