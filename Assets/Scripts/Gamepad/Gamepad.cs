using UnityEngine;
using System;
using System.Collections;

public class Gamepad : MonoBehaviour {
	
	public enum BUTTONS {
		A, B, UP, RIGHT, DOWN, LEFT
	}
	
	// 5 is ok for mobile 30 fps, 9 for 60 fps
	public const short HARD_PRESSED_MIN_COUNT = 9;
	
	// keeps track of every button's state
	private static bool[] buttonsState = new bool[System.Enum.GetValues(typeof(BUTTONS)).Length];
	// counts how many game loops the button is being pressed;
	private static short[] hardPressedCount = new short[System.Enum.GetValues(typeof(BUTTONS)).Length];
	
	private static Gamepad instance = null;
	
	public static Gamepad Instance {
        get {
            warm();
            return instance;
        }
    }
	
	public static void warm () {
		if (instance == null) {
			// instantiate the entire prefab because it has herarchy. 
			// Don't assign to the instance variable because it is then assigned in Awake()
			GameObject.Instantiate(Resources.Load("Prefabs/Gamepad"));
		}
	}
	
	void Awake () {
		if (instance != null && instance != this)
			Destroy(this.gameObject);
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		initialize();
	}
	
	private void initialize () {
		resetButtonState();
	}
	
	/// <summary>
	/// Resets the state of all buttons.
	/// </summary>
	public static void resetButtonState () {
		for (int i=0; i < buttonsState.Length; ++i)
			buttonsState[i] = false;
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
	
	void OnDestroy () {
#if UNITY_EDITOR
#else
		// NOTE: to avoid !IsPlayingOrAllowExecuteInEditMode error in console:
		instance = null;
#endif
	}
	
	// LateUpdate is called after all Update functions have been called
	void LateUpdate () {
		updateHardPressed();
		// IMPORTANT: this should be invoked after all the listeners has executed their callbacks.
		resetButtonState();
	}
	
	/// <summary>
	/// Set the button's state to true (on). If the button does pass the hard pressed test, 
	/// the Gamepad manager keeps track of the situation.
	/// </summary>
	public static void fireButton (BUTTONS button) {
		buttonsState[(int)button] = true;
	}
	
	/// <summary>
	/// Returns true if the buttons has being pressed for a threshold time enough to 
	/// be considered as hard pressed. Eitherway false.
	/// </summary>
	/// <returns>
	/// bool
	/// </returns>
	/// <param name='button'>
	/// The button enum value
	/// </param>
	public static bool isHardPressed (BUTTONS button) {
		return hardPressedCount[(int)button] >= HARD_PRESSED_MIN_COUNT;
	}
	
	public static bool isUp() {
		return buttonsState[(int)BUTTONS.UP];
	}
	
	public static bool isDown() {
		return buttonsState[(int)BUTTONS.DOWN];
	}
	
	public static bool isLeft() {
		return buttonsState[(int)BUTTONS.LEFT];
	}
	
	public static bool isRight() {
		return buttonsState[(int)BUTTONS.RIGHT];
	}
	
	public static bool isA() {
		return buttonsState[(int)BUTTONS.A];
	}
	
	public static bool isB() {
		return buttonsState[(int)BUTTONS.B];
	}
}
