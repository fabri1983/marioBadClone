using UnityEngine;

public class OptionClickQuit : MonoBehaviour, ITouchListener {
	
	private static bool appQuit = false;
	private static Rect rectQuit, rectBack;
	
	private static OptionClickQuit instance = null;
	
	public static OptionClickQuit Instance {
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
		// in case the game object wasn't instantiated yet from another script
		if (instance == null) {
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
	
	private void optionSelected() {
		if (appQuit)
			return;
		PauseGameManager.Instance.pause();
		Camera.main.GetComponent<FadeCamera>().startFading(EnumFadeDirection.FADE_IN);
		appQuit = true;
	}
	
	void OnGUI () {
		if (!appQuit)
			return;
		
		if(GUI.Button(rectQuit, "Quit")) {
			Application.Quit(); // doesn't work on Editor mode
			appQuit = false;
			Camera.main.GetComponent<FadeCamera>().startFading(EnumFadeDirection.FADE_OUT);
			PauseGameManager.Instance.resume();
		}
		if(GUI.Button(rectBack, "Back")) {
			appQuit = false;
			Camera.main.GetComponent<FadeCamera>().startFading(EnumFadeDirection.FADE_OUT);
			PauseGameManager.Instance.resume();
		}
	}
}
