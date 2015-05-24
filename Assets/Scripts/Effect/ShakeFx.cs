using UnityEngine;
using System.Collections;

public class ShakeFx : Effect, ITouchListener, IEffectListener
{
	// controls the amount that shake_intensity is decremented each update. It determines if the shake is long or short
	public float shake_decay = 0.5f;
	// determines the initial intensity of the shaking — how much variance to allow in the object's position
	public float shake_intensity = 4f;
	// true if you want also shake rotation of game object
	public bool allowRotation = false;
	public float startDelaySecs = 0f;
#if UNITY_EDITOR
	// set to true if you want to test in gameplay mode
	public bool debug = false;
#endif
	
	private float tempDecay, tempIntensity;
	private Vector3 origPosition;
	private Quaternion origRotation;
	private Quaternion quatTemp;
	private bool allowShake = false;
	
	protected override void ownAwake () {
		EffectPrioritizerHelper.registerForEndEffect(this);
	}
	
	protected override void ownEffectStarts () {
		Invoke("reset", startDelaySecs);
	}
	
	void OnDestroy () {
		TouchEventManager.Instance.removeListener(this);
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
	
	private void reset ()
	{
		allowShake = true;
		tempDecay = shake_decay;
		tempIntensity = shake_intensity;
		// for GUI Custom elements use localPosition
		origPosition = transform.localPosition;
		origRotation = transform.rotation;
	}
	
	private void shakeTransition ()
	{
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
			effectEnded();
		}
	}
	
	public bool isScreenStatic () {
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
			return GUIScreenLayoutManager.getPositionInScreen(GetComponent<GUICustomElement>());
	}
	
	public void OnBeganTouch (Touch t) {
		executeEffect();
	}
	
	public void OnStationaryTouch (Touch t) {}
	
	public void OnEndedTouch (Touch t) {}
	
	public Effect[] getEffects () {
		// return the transitions in an order set from Inspector.
		// Note: to return in a custom order get the transitions array and sort it as desired.
		return EffectPrioritizerHelper.getEffects(gameObject, false);
	}
	
	public void onLastEffectEnd () {
		// register with touch event manager once the effect finishes since the touch
		// event depends on final element's position
		TouchEventManager.Instance.register(this, TouchPhase.Began);
	}
}
