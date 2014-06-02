using UnityEngine;

public class OptionClickQuit : TouchListenerAbs {
	
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
		
		bool isScenelOnly = false;
		bool propagateAllPhases = false;
		InputTouchManager.Instance.register(this, isScenelOnly, propagateAllPhases, TouchPhase.Began, TouchPhase.Ended);
	}
	
	/**
	 * This only fired on PC
	 */
	void OnMouseUpAsButton () {
		optionSelected();
	}
	
	public override GameObject getGameObject () {
		return gameObject;
	}
	
	public override void OnBeganTouch (Touch t) {
		optionSelected();
	}
	
	public override void OnStationaryTouch (Touch t) {}
	
	public override void OnEndedTouch (Touch t) {
		optionSelected();
	}
	
	private void optionSelected() {
		// Quit() doesn't work in editor mode
		Application.Quit();
	}
}
