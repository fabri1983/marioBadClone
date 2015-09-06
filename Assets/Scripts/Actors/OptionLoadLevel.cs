using UnityEngine;

public class OptionLoadLevel : MonoBehaviour, ITouchListener, IEffectListener {
	
	public int sceneIndex; // index of the scene to be loaded
	public GUICustomElement actionGuiElem;
	
	private bool selected = false;
	private Rect _screenBounds; // cache for the screen bounds this game object covers
	private BeforeLoadNextScene beforeNextScene;
	
	void Awake () {
		// initialize the screen bounds cache
		_screenBounds.x = -1f;
		// do some setup after finishes all gameobject effects
		EffectPrioritizerHelper.registerAsEndEffect(this as IEffectListener);
		// setup the effects chain
		beforeNextScene = GetComponent<BeforeLoadNextScene>();
		if (beforeNextScene != null)
			beforeNextScene.setSceneIndex(sceneIndex);
	}
	
	void OnDestroy () {
		TouchEventManager.Instance.removeListener(this as ITouchListener);
	}

	void Update () {
		if (selected && Gamepad.Instance.isA())
			doAction();
	}
	
	public bool isScreenStatic () {
		// for event touch listener
		return true;
	}
	
	public GameObject getGameObject () {
		return gameObject;
	}
	
	public Rect getScreenBoundsAA () {
		// checks if the cached size has changed
		if (_screenBounds.x == -1f)
			_screenBounds = GUIScreenLayoutManager.getPositionInScreen(actionGuiElem);
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
	
	private void doAction () {
		if (PauseGameManager.Instance.isPaused())
			return;

		this.enabled = false; // avoid repetead execution when on touching
		
		if (beforeNextScene != null)
			beforeNextScene.execute();
		else
			LevelManager.Instance.loadLevel(sceneIndex);
	}
	
	public Effect[] getEffects () {
		return GetComponentsInChildren<Effect>();
	}
	
	public void onLastEffectEnd () {
		// register with touch event manager once the effect finishes since the touch
		// event depends on final element's position
		TouchEventManager.Instance.register(this as ITouchListener, TouchPhase.Began);
	}

#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER
	void OnGUI () {
		// since this game object has a GUICustomElement script attached to it, for strange a reason no mouse event 
		// is caught, so we need to manually check for the event and fire it here
		Event e = Event.current;
		if (e != null && e.isMouse && e.button == 0 && e.type == EventType.MouseUp) {
			if (GameObjectTools.testHitFromMousePos(actionGuiElem.transform, e.mousePosition))
				doAction();
		}
	}
#endif
}
