using UnityEngine;

public class OptionQuit : MonoBehaviour, ITouchListener, ITransitionListener, IGUIScreenLayout {
	
	private bool showOptions = false;
	private Rect rectQuit, rectBack, rectLevelSel;
	private IFadeable fader;
	
	private static OptionQuit instance = null;
	
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
		if (instance != null && instance != this)
			Destroy(this.gameObject);
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		setupButtons(); // locate the buttons
		GUIScreenLayoutManager.Instance.register(this);
		TransitionGUIFxManager.Instance.registerForEndTransitions(this);
	}
	
	void OnDestroy () {
		GUIScreenLayoutManager.Instance.remove(this);
	}

	public void updateForGUI() {
		//setupButtons();
	}
	
	private void setupButtons () {
		rectQuit.Set(480 / 2 - 25 - 50, 320 / 2 - 45, 50, 24);
		rectBack.Set(480 / 2 - 25 + 50, 320 / 2 - 45, 50, 24);
		rectLevelSel.Set(480 / 2 - 35, 320 / 2 + 10, 70, 24);
	}
	
	public bool isStatic () {
		// for event touch listener
		return true;
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
		optionSelected();
	}
	
	public void OnStationaryTouch (Touch t) {}
	
	public void OnEndedTouch (Touch t) {
		optionSelected();
	}
	
	public TransitionGUIFx[] getTransitions () {
		// return the transitions in an order set from Inspector.
		// Note: to return in a custom order get the transitions array and sort it as desired.
		return TransitionGUIFxManager.getTransitionsInOrder(gameObject);
	}
	
	public void prevTransitionEnd (TransitionGUIFx fx) {
		// register with touch event manager once the transition finishes since the manager
		// depends on final element's position
		TouchEventManager.Instance.register(this, TouchPhase.Began, TouchPhase.Ended);
	}
	
	public void reset() {
		showOptions = false;
	}
	
	public void setFaderFromMainCamera () {
		// need to find IFadeable component here because main camera instance changes during scenes
		fader = Camera.main.GetComponent<CameraFadeable>().getFader();
	}
	
	private void optionSelected() {
		if (showOptions)
			return;
		
		PauseGameManager.Instance.pause();
		fader.startFading(EnumFadeDirection.FADE_IN);
		showOptions = true;
	}
	
	void Update () {
		// back button
		if (Input.GetKeyDown(KeyCode.Escape)) {
			// if options already shown, then hide them
			if (showOptions)
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
		if (!showOptions)
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
		showOptions = false;
		fader.startFading(EnumFadeDirection.FADE_OUT);
		PauseGameManager.Instance.resume();
	}
	
	private void levelSelection () {
		showOptions = false;
		fader.startFading(EnumFadeDirection.FADE_OUT);
		PauseGameManager.Instance.resume();
		LevelManager.Instance.loadLevelSelection();
	}
}
