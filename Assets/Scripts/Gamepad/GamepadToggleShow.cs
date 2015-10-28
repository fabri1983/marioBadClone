using UnityEngine;
using System.Collections;

public class GamepadToggleShow : MonoBehaviour, IEffectListener, SwipeGestureListener {

	private Gamepad gamepad;
	private SwipeGesture swipeCtrl;
	private float screenLimitPosYup, screenLimitPosYdown;
	private bool swipeDown = false;
	private bool swipeUp = false;
	private Vector3 screenPos;
	private float screenYdisplacement = 8; // pixels
	
	void Awake () {
		EffectPrioritizerHelper.registerAsEndEffect(this as IEffectListener);
		
		swipeCtrl = GetComponent<SwipeGesture>();
		swipeCtrl.enabled = false;
		
		gamepad = GetComponent<Gamepad>();
	}
	
	public Effect[] getEffects () {
		return GetComponentsInChildren<Effect>();
	}
	
	public void onLastEffectEnd () {
		// cache for the screen position
		screenPos = Camera.main.WorldToScreenPoint(transform.position);
		// limit the Y axis screen displacement of the game object
		screenLimitPosYup = screenPos.y;
		screenLimitPosYdown = screenPos.y - 80f - 30f; // had to mod down a little further
		// setup the swipe gesture control once all the gamepad elements effects finish
		setupSwipeControl();
	}
	
	private void setupSwipeControl () {
		// only setting up the active area
		Rect areaRect = new Rect((Screen.width / 2) - 90, Screen.height - 80, 180f, 80f);
		swipeCtrl.settings.activeArea = areaRect;
		
		swipeCtrl.enabled = true;
		swipeCtrl.setup();
		swipeCtrl.registerOnUpSwipe(this as SwipeGestureListener);
		swipeCtrl.registerOnDownSwipe(this as SwipeGestureListener);
	}
	
	void Update () {
		applyGesture();
	}
	
	public void notifyRight () {
	}
	
	public void notifyLeft () {
	}
	
	public void notifyUp () {
		swipeUp = true;
		swipeDown = false;
	}
	
	public void notifyDown () {
		swipeUp = false;
		swipeDown = true;
	}
	
	private void applyGesture () {
		if (!swipeUp && !swipeDown)
			return;
		
		Vector3 thePos = transform.position;
		
		if (swipeUp) {
			screenPos.y += screenYdisplacement;
			if (screenPos.y > screenLimitPosYup) {
				gamepad.setTouchEnabled(true);
				swipeUp = false;
				screenPos.y = screenLimitPosYup;
			}
		}
		else if (swipeDown) {
			screenPos.y -= screenYdisplacement;
			if (screenPos.y < screenLimitPosYdown) {
				gamepad.setTouchEnabled(false);
				swipeDown = false;
				screenPos.y = screenLimitPosYdown;
			}
		}
		
		thePos.y = Camera.main.ScreenToWorldPoint(screenPos).y;
		transform.position = thePos;
	}
}
