using UnityEngine;
using System.Collections;

public class ShakeGUIFx : Effect, ITouchListener, IEffectListener
{
	// controls the amount that shake_intensity is decremented each update. It determines if the shake is long or short
	public float shakeDecayStep = 0.5f;
	// determines the initial intensity of the shaking — how much variance to allow in the object's position
	public float shakeIntensity = 4f;
	// true if you want also shake rotation of game object
	public bool allowRotation = false;
#if UNITY_EDITOR
	// set to true if you want to test in gameplay mode
	public bool debug = false;
#endif
	
	private float tempDecay, tempIntensity;
	private Vector3 origPosition;
	private Quaternion origRotation;
	private Quaternion quatTemp;
	private bool allowShake = false;
	private GUICustomElement guiElem;
	private Rect _screenBounds; // cache for the screen bounds the GUI element covers
	
	protected override void ownAwake () {
		_screenBounds.x = -1f; // initialize the screen bounds cache
		guiElem = GetComponent<GUICustomElement>();
		EffectPrioritizerHelper.registerAsEndEffect(this as IEffectListener);
	}
	
	protected override void ownStartEffect () {
		reset();
	}
	
	protected override void ownEndEffect () {
	}
	
	protected override void ownOnDestroy () {
		TouchEventManager.Instance.removeListener(this as ITouchListener);
	}
	
	void Update () {
		if (allowShake)
			shakeTransition();
	}
	
#if UNITY_EDITOR
	void OnGUI () {
		if (!debug)
			return;
		if (GUI.Button (new Rect (20, 40, 80, 20), "Shake"))
			reset();
	}
#endif
	
	private void reset () {
		allowShake = true;
		tempDecay = shakeDecayStep;
		tempIntensity = shakeIntensity;
		// for GUI Custom elements use localPosition
		origPosition = transform.localPosition;
		origRotation = transform.rotation;
	}
	
	private void shakeTransition () {
		if (tempIntensity > 0) {
			transform.localPosition = origPosition + Random.insideUnitSphere * tempIntensity;
			if (allowRotation) {
				quatTemp.Set(
					origRotation.x + Random.Range (-tempIntensity, tempIntensity) * .2f,
					origRotation.y + Random.Range (-tempIntensity, tempIntensity) * .2f,
					origRotation.z + Random.Range (-tempIntensity, tempIntensity) * .2f,
					origRotation.w + Random.Range (-tempIntensity, tempIntensity) * .2f);
				transform.rotation = quatTemp;
			}
			tempIntensity -= tempDecay;
		}
		// once the effect finishes reset to original transform
		else {
			// for GUI Custom elements use localPosition
			transform.localPosition = origPosition;
			transform.rotation = origRotation;
			allowShake = false;
			endEffect();
		}
	}
	
	public bool isScreenStatic () {
		// for event touch listener
		return true;
	}
	
	public Rect getScreenBoundsAA () {		
		// checks if the cached size has changed
		if (_screenBounds.x == -1f)
			_screenBounds = GUIScreenLayoutManager.getPositionInScreen(guiElem);
		return _screenBounds;
	}
	
	public void OnBeganTouch (Touch t) {
		if (PauseGameManager.Instance.isPaused())
			return;
		float temp = startDelaySecs = 0f;
		startEffect();
		startDelaySecs = temp;
	}
	
	public void OnStationaryTouch (Touch t) {
	}
	
	public void OnEndedTouch (Touch t) {
	}
	
	public Effect[] getEffects () {
		return GetComponents<Effect>();
	}
	
	public void onLastEffectEnd () {
		_screenBounds.x = -1f; // reset the cache variable
		
		// register with touch event manager once the effect finishes since the touch
		// event depends on final element's position
		TouchEventManager.Instance.register(this as ITouchListener, TouchPhase.Began);
	}
}
