using UnityEngine;

public class OptionQuit : MonoBehaviour, ITouchListener, IEffectListener {
	
	private bool showingOptions = false;
	private Rect rectQuit, rectBack, rectLevelSel;
	private IFadeable fader;
	private Rect _screenBounds; // cache for the screen bounds the GUI element covers
	private GUICustomElement guiElem;
	
	private static OptionQuit instance = null;
	private static bool duplicated = false; // usefull to avoid onDestroy() execution on duplicated instances being destroyed
	
	public static OptionQuit Instance {
        get {
            if (instance == null) {
				// Instantiate the entire prefab. 
				// Don't assign to the instance variable because it is then assigned in Awake()
				GameObject.Instantiate(Resources.Load(KResources.GUI_QUIT));
			}
            return instance;
        }
    }
	
	void Awake () {
		if (instance != null && instance != this) {
			duplicated = true;
			Destroy(this.gameObject);
		}
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
			initialize();
		}
	}
	
	private void initialize () {
		guiElem = GetComponent<GUICustomElement>();
		_screenBounds.x = -1f; // initialize the screen bounds cache
		setupButtons(); // locate the buttons
		EffectPrioritizerHelper.registerAsEndEffect(this as IEffectListener);
	}
	
	void OnDestroy () {
		// this is to avoid nullifying or destroying static variables. Intance variables can be destroyed before this check
		if (duplicated) {
			duplicated = false; // reset the flag for next time
			return;
		}
		TouchEventManager.Instance.removeListener(this as ITouchListener);
	}
	
	private void setupButtons () {
		rectQuit.Set(480 / 2 - 25 - 50, 320 / 2 - 45, 50, 24);
		rectBack.Set(480 / 2 - 25 + 50, 320 / 2 - 45, 50, 24);
		rectLevelSel.Set(480 / 2 - 35, 320 / 2 + 10, 70, 24);
	}
	
	public bool isScreenStatic () {
		// for event touch listener
		return true;
	}
	
	public Rect getScreenBoundsAA () {
		// checks if the cached size has changed
		if (_screenBounds.x == -1f)
			_screenBounds = GUIScreenLayoutManager.getPositionInScreen(guiElem);
		return _screenBounds;
	}
	
	public void OnBeganTouch (Touch t) {
		optionSelected();
	}
	
	public void OnStationaryTouch (Touch t) {}
	
	public void OnEndedTouch (Touch t) {}
	
	public Effect[] getEffects () {
		return GetComponents<Effect>();
	}
	
	public void onLastEffectEnd () {
		_screenBounds.x = -1f; // reset the cache variable
		
		// register with touch event manager once the effect finishes since the touch
		// event depends on final element's position
		TouchEventManager.Instance.register(this as ITouchListener, TouchPhase.Began);
	}
	
	public void reset() {
		showingOptions = false;
	}
	
	public void setFaderFromMainCamera () {
		// need to find IFadeable component here because main camera instance changes during scenes
		fader = Camera.main.GetComponent<CameraFadeable>().getFader();
	}
	
	private void optionSelected() {
		if (showingOptions)
			return;
		
		PauseGameManager.Instance.pause();
		fader.startFading(EnumFadeDirection.FADE_IN);
		showingOptions = true;
	}
	
	void Update () {
		// back button
		if (Input.GetKeyDown(KeyCode.Escape)) {
			// if options already shown, then hide them
			if (showingOptions)
				back();
			else
				optionSelected();
		}
	}
	
	void OnGUI () {	
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER
		// since this game object has a GUICustomElement script attached to it, for strange a reason no mouse event 
		// is caught, so we need to manually check for the event and fire it here
		Event e = Event.current;
		if (e != null && e.isMouse && e.button == 0 && e.type == EventType.MouseUp) {
			if (GameObjectTools.testHitFromMousePos(transform, e.mousePosition))
				optionSelected();
		}
#endif
		if (!showingOptions)
			return;

		// update Unity GUI matrix to allow automatic resizing (only works for Unity GUI elems)
		// NOTE: this transformation has effect per game loop and per monobehaviour script
		GUI.matrix = GUIScreenLayoutManager.unityGUIMatrix;

		if (GUI.Button(rectQuit, "Quit"))
			quit();
		if (GUI.Button(rectBack, "Back"))
			back();
		if (GUI.Button(rectLevelSel, "Levels"))
			levelSelection();
	}
	
	private void quit () {
		Application.Quit(); // doesn't work on Editor mode
#if UNITY_EDITOR
		back();
#endif
	}
	
	private void back () {
		showingOptions = false;
		fader.startFading(EnumFadeDirection.FADE_OUT);
		PauseGameManager.Instance.resume();
	}
	
	private void levelSelection () {
		showingOptions = false;
		fader.startFading(EnumFadeDirection.FADE_OUT);
		PauseGameManager.Instance.resume();
		LevelManager.Instance.loadLevelSelection();
	}
}
