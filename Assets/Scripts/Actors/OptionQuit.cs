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
		
		locateButtons(); // locate the buttons
		ScreenLayoutManager.Instance.register(this);
		TransitionGUIFxManager.Instance.register(this, false);
	}
	
	void OnDestroy () {
		TransitionGUIFxManager.Instance.remove(this);
		ScreenLayoutManager.Instance.remove(this);
	}
	
	public void updateSizeAndPosition() {
		locateButtons();
	}
	
	private void locateButtons () {
		rectQuit.Set(Screen.width / 2 - 25 - 50, Screen.height / 2 - 45, 50, 24);
		rectBack.Set(Screen.width / 2 - 25 + 50, Screen.height / 2 - 45, 50, 24);
		rectLevelSel.Set(Screen.width / 2 - 35, Screen.height / 2 + 10, 70, 24);
	}
	
	/**
	 * This only fired on PC
	 */
	void OnMouseUpAsButton () {
		Debug.Log("---------------------");
		optionSelected();
	}
	
	public bool isStatic () {
		return true;
	}
	
	public GameObject getGameObject () {
		return gameObject;
	}
	
	public Rect getScreenBoundsAA () {
		// this method called only once since its a non destroyable game object
		if (guiTexture != null)
			return guiTexture.GetScreenRect(Camera.main);
		else {
			Bounds b = renderer.bounds;
			return new Rect(b.min.x, b.min.y, b.max.x, b.max.y);
		}
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
		TouchEventManager.Instance.register(this, TouchPhase.Ended);
	}
	
	public void reset() {
		showOptions = false;
	}
	
	private void optionSelected() {
		if (showOptions)
			return;
		PauseGameManager.Instance.pause();
		// need to find IFadeable component here because camera can change during scenes
		fader = (IFadeable)Camera.main.GetComponentInChildren(typeof(IFadeable));
		fader.startFading(EnumFadeDirection.FADE_IN);
		showOptions = true;
	}
	
	void OnGUI () {
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
