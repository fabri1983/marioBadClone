using UnityEngine;
using System;
using System.Collections;

public class Gamepad : MonoBehaviour {
	
	// 5 is ok for mobile 30 fps, 9 for 60 fps
	public const short HARD_PRESSED_MIN_COUNT = 9;
	
	// keeps track of every button's state
	private static bool[] buttonsState = new bool[System.Enum.GetValues(typeof(EnumButton)).Length];
	// counts how many game loops the button is being pressed;
	private static short[] hardPressedCount = new short[System.Enum.GetValues(typeof(EnumButton)).Length];
	
	private static Gamepad instance = null;
	private static bool duplicated = false; // usefull to avoid onDestroy() execution on duplicated instances being destroyed
	
	private bool triggerEnabled = true;
	
	public static Gamepad Instance {
        get {
            if (instance == null)
				// Instantiate the entire prefab because it has a herarchy. 
				// Don't assign to the instance variable because it is then assigned in Awake()
				GameObject.Instantiate(Resources.Load("Prefabs/GUI_Gamepad"));
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
		resetButtonState();
	}
	
	void OnDestroy () {
		// this is to avoid nullifying or destroying static variables. Intance variables can be destroyed before this check
		if (duplicated) {
			duplicated = false;
			return;
		}
		instance = null;
	}
	
	public void setTouchEnabled (bool value) {
		triggerEnabled = value;
	}
	
	/// <summary>
	/// Resets the state of all buttons.
	/// </summary>
	public void resetButtonState () {
		for (int i=0; i < buttonsState.Length; ++i)
			buttonsState[i] = false;
	}
	
	void LateUpdate () {
		updateHardPressed();
		// IMPORTANT: this should be invoked after all the listeners has executed their callbacks.
		resetButtonState();
	}
	
	/// <summary>
	/// Updates the hard pressed state of every button.
	/// </summary>
	private void updateHardPressed () {
		// increment count for ON buttons. Set to 0 to OFF buttons
		for (int i=0; i < buttonsState.Length; ++i) {
			if (buttonsState[i])
				++hardPressedCount[i];
			else
				hardPressedCount[i] = 0;
		}
	}
	
	/// <summary>
	/// Set the button's state to true (on). If the button does pass the hard pressed test, 
	/// the Gamepad manager keeps track of the situation.
	/// </summary>
	public void fireButton (EnumButton button) {
		buttonsState[(int)button] = true;
	}
	
	/// <summary>
	/// Returns true if the buttons has being pressed for a threshold time enough to 
	/// be considered as hard pressed. Eitherway false.
	/// </summary>
	/// <returns>bool</returns>
	/// <param name='button'>The button enum value</param>
	public bool isHardPressed (EnumButton button) {
		return triggerEnabled && hardPressedCount[(int)button] >= HARD_PRESSED_MIN_COUNT;
	}
	
	public bool isUp() {
		return (triggerEnabled && buttonsState[(int)EnumButton.UP]) || Input.GetAxis("Vertical") > 0.1f;
	}
	
	public bool isDown() {
		return (triggerEnabled && buttonsState[(int)EnumButton.DOWN]) || Input.GetAxis("Vertical") < -0.1f;
	}
	
	public bool isLeft() {
		return (triggerEnabled && buttonsState[(int)EnumButton.LEFT]) || Input.GetAxis("Horizontal") < -0.1f;
	}
	
	public bool isRight() {
		return (triggerEnabled && buttonsState[(int)EnumButton.RIGHT]) || Input.GetAxis("Horizontal") > 0.1f;
	}
	
	public bool isA() {
		return (triggerEnabled && buttonsState[(int)EnumButton.A]) || Input.GetButton("Button A");
	}
	
	public bool isB() {
		return (triggerEnabled && buttonsState[(int)EnumButton.B]) || (Input.GetButton("Button B") && Input.touchCount == 0);
	}
}
