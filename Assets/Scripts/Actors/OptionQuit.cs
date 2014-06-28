using UnityEngine;

public class OptionQuit : MonoBehaviour, ITouchListener {
	
	private bool showOptions = false;
	private Rect rectQuit, rectBack;
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
		
		rectQuit = new Rect(Screen.width / 2 - 25 - 50, Screen.height / 2 - 25, 50, 24);
		rectBack = new Rect(Screen.width / 2 - 25 + 50, Screen.height / 2 - 25, 50, 24);
		
		TouchEventManager.Instance.register(this, TouchPhase.Ended);
	}
	
	/**
	 * This only fired on PC
	 */
	void OnMouseUpAsButton () {
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
		return guiTexture.GetScreenRect(Camera.main);
	}
	
	public void OnBeganTouch (Touch t) {
		optionSelected();
	}
	
	public void OnStationaryTouch (Touch t) {}
	
	public void OnEndedTouch (Touch t) {
		optionSelected();
	}
	
	public void reset() {
		showOptions = false;
	}
	
	private void optionSelected() {
		if (showOptions)
			return;
		PauseGameManager.Instance.pause();
		// need to find IFadeable component here because camera can change during scenes
		fader = (IFadeable)Camera.main.GetComponent(typeof(IFadeable));
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
	}
}
