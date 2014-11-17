using UnityEngine;

public class OptionQuit : MonoBehaviour, ITouchListener, ITransitionListener, IScreenLayout {
	
	private bool showOptions = false;
	private Rect rectQuit, rectBack, rectLevelSel;
	private IFadeable fader;
	
	private static OptionQuit instance = null;
	
	public static OptionQuit Instance {
        get {
            warm();
            return instance;
        }
    }
	
	public static void warm () {
		// in case the game object wasn't instantiated yet from another script
		if (instance == null) {
			// instantiate the entire prefab. Don't assign to the instance variable because it is then assigned in Awake()
			GameObject.Instantiate(Resources.Load("Prefabs/GUI_Quit"));
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
		TransitionGUIFxManager.Instance.register(this, false);
	}
	
	void OnDestroy () {
		TransitionGUIFxManager.Instance.remove(this);
		GUIScreenLayoutManager.Instance.remove(this);
	}
	
	public void updateForGUI() {
		setupButtons();
	}
	
	private void setupButtons () {
		rectQuit.Set(Screen.width / 2 - 25 - 50, Screen.height / 2 - 45, 50, 24);
		rectBack.Set(Screen.width / 2 - 25 + 50, Screen.height / 2 - 45, 50, 24);
		rectLevelSel.Set(Screen.width / 2 - 35, Screen.height / 2 + 10, 70, 24);
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
		return GetComponents<TransitionGUIFx>();
	}
	
	public void onEndTransition (TransitionGUIFx fx) {
		// register with touch event manager once the transition finishes since the manager
		// depends on final element's position
		TouchEventManager.Instance.register(this, TouchPhase.Began, TouchPhase.Ended);
	}
	
	public void reset() {
		showOptions = false;
	}
	
	public void setFaderForMainCamera () {
		// need to find IFadeable component here because main camera instance changes during scenes
		fader = (IFadeable)Camera.main.GetComponentInChildren(typeof(IFadeable));
	}
	
	private void optionSelected() {
		if (showOptions)
			return;
		
		PauseGameManager.Instance.pause();
		fader.startFading(EnumFadeDirection.FADE_IN);
		showOptions = true;
	}
	
	void OnGUI () {	
#if UNITY_EDITOR
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
		
		if(GUI.Button(rectQuit, "Quit")) {
			Application.Quit(); // doesn't work on Editor mode
#if UNITY_EDITOR
			showOptions = false;
			fader.startFading(EnumFadeDirection.FADE_OUT);
			PauseGameManager.Instance.resume();
#endif
		}
		if(GUI.Button(rectBack, "Back")) {
			showOptions = false;
			fader.startFading(EnumFadeDirection.FADE_OUT);
			PauseGameManager.Instance.resume();
		}
		if(GUI.Button(rectLevelSel, "Levels")) {
			showOptions = false;
			fader.startFading(EnumFadeDirection.FADE_OUT);
			PauseGameManager.Instance.resume();
			LevelManager.Instance.loadLevelSelection();
		}
	}
}
