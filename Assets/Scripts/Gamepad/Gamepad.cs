using UnityEngine;
using System;
using System.Collections;

public class Gamepad : MonoBehaviour, IEffectListener {
	
	// 5 is ok for mobile 30 fps, 9 for 60 fps
	public const short HARD_PRESSED_MIN_COUNT = 9;
	
	// keeps track of every button's state
	private static bool[] buttonsState = new bool[System.Enum.GetValues(typeof(EnumButton)).Length];
	// counts how many game loops the button is being pressed;
	private static short[] hardPressedCount = new short[System.Enum.GetValues(typeof(EnumButton)).Length];
	
	private static Gamepad instance = null;
	private static bool duplicated = false; // usefull to avoid onDestroy() execution on duplicated instances being destroyed
	
	private SwipeGesture swipeCtrl;
	private float screenLimitPosYup, screenLimitPosYdown;
	private bool touchEnabled = true;
	private bool swipeDown = false;
	private bool swipeUp = false;
	private Vector3 screenPos;
	
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
		EffectPrioritizerHelper.registerAsEndEffect(this as IEffectListener);
		swipeCtrl = GetComponent<SwipeGesture>();
		swipeCtrl.enabled = false;
	}
	
	void OnDestroy () {
		// this is to avoid nullifying or destroying static variables. Intance variables can be destroyed before this check
		if (duplicated) {
			duplicated = false;
			return;
		}
		instance = null;
	}
	
	/// <summary>
	/// Resets the state of all buttons.
	/// </summary>
	public void resetButtonState () {
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
	
	public Effect[] getEffects () {
		return GetComponentsInChildren<Effect>();
	}
	
	public void onLastEffectEnd () {
		// cache for the screen position
		screenPos = Camera.main.WorldToScreenPoint(transform.position);
		// limit the Y axis screen displacement of the game object
		screenLimitPosYup = screenPos.y;
		screenLimitPosYdown = screenPos.y + 80f;
		// setup the swipe gesture control once all the gamepad elements effects finish
		setupSwipeControl();
	}
	
	private void setupSwipeControl () {
		// only setting up the active area
		Rect areaRect = new Rect((Screen.width / 2) - 90, Screen.height - 80, 180f, 80f);
		swipeCtrl.settings.activeArea = areaRect;
		
		swipeCtrl.enabled = true;
		swipeCtrl.setup();
		swipeCtrl.EventOnDownSwipe += () => { swipeUp = true; };
		swipeCtrl.EventOnUpSwipe += () => { swipeDown = true; };
	}
	
	void Update () {
		applyGesture();
	}
	
	void LateUpdate () {
		updateHardPressed();
		// IMPORTANT: this should be invoked after all the listeners has executed their callbacks.
		resetButtonState();
	}
	
	private void applyGesture () {
		if (!swipeUp && !swipeDown)
			return;
		
		Vector3 thePos = transform.position;
		
		if (swipeUp)
			screenPos.y -= 8;
		else if (swipeDown)
			screenPos.y += 8;
		
		if (screenPos.y < screenLimitPosYup) {
			touchEnabled = true;
			swipeUp = false;
			screenPos.y = screenLimitPosYup;
		}
		else if (screenPos.y > screenLimitPosYdown) {
			touchEnabled = false;
			swipeDown = false;
			screenPos.y = screenLimitPosYdown;
		}
		
		thePos.y = Camera.main.ScreenToWorldPoint(screenPos).y;
		transform.position = thePos;
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
	/// <returns>
	/// bool
	/// </returns>
	/// <param name='button'>
	/// The button enum value
	/// </param>
	public bool isHardPressed (EnumButton button) {
		return touchEnabled && hardPressedCount[(int)button] >= HARD_PRESSED_MIN_COUNT;
	}
	
	public bool isUp() {
		return (touchEnabled && buttonsState[(int)EnumButton.UP]) || Input.GetAxis("Vertical") > 0.1f;
	}
	
	public bool isDown() {
		return (touchEnabled && buttonsState[(int)EnumButton.DOWN]) || Input.GetAxis("Vertical") < -0.1f;
	}
	
	public bool isLeft() {
		return (touchEnabled && buttonsState[(int)EnumButton.LEFT]) || Input.GetAxis("Horizontal") < -0.1f;
	}
	
	public bool isRight() {
		return (touchEnabled && buttonsState[(int)EnumButton.RIGHT]) || Input.GetAxis("Horizontal") > 0.1f;
	}
	
	public bool isA() {
		return (touchEnabled && buttonsState[(int)EnumButton.A]) || Input.GetButton("Button A");
	}
	
	public bool isB() {
		return (touchEnabled && buttonsState[(int)EnumButton.B]) || (Input.GetButton("Button B") && Input.touchCount == 0);
	}
}
