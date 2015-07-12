using UnityEngine;

public class OptionQuit : MonoBehaviour, ITouchListener, IEffectListener, IGUIScreenLayout {
	
	private bool showingOptions = false;
	private Rect rectQuit, rectBack, rectLevelSel;
	private IFadeable fader;
	
	private static OptionQuit instance = null;
	private static bool duplicated = false; // usefull to avoid onDestroy() execution on duplicated instances being destroyed
	
	public static OptionQuit Instance {
        get {
            if (instance == null) {
				// Instantiate the entire prefab. 
				// Don't assign to the instance variable because it is then assigned in Awake()
				GameObject.Instantiate(Resources.Load("Prefabs/GUI_Quit"));
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
		setupButtons(); // locate the buttons
		GUIScreenLayoutManager.Instance.register(this as IGUIScreenLayout);
		EffectPrioritizerHelper.registerForEndEffect(this as IEffectListener);
	}
	
	void OnDestroy () {
		// this is to avoid nullifying or destroying static variables. Intance variables can be destroyed before this check
		if (duplicated) {
			duplicated = false; // reset the flag for next time
			return;
		}
		GUIScreenLayoutManager.Instance.remove(this as IGUIScreenLayout);
		TouchEventManager.Instance.removeListener(this as ITouchListener);
	}

	public void updateForGUI() {
		//setupButtons();
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
	
	public GameObject getGameObject () {
		return gameObject;
	}
	
	public Rect getScreenBoundsAA () {
		// if used with a Unity's GUITexture
		if (guiTexture != null)
			return guiTexture.GetScreenRect(Camera.main);
		// here I suppose this game object has attached a GUICustomElement
		else
			return GUIScreenLayoutManager.getPositionInScreen(GetComponent<GUICustomElement>());
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
		// NOTE: this transformation has effect per game loop
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
