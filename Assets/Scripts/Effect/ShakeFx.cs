using UnityEngine;
using System.Collections;

public class ShakeFx : Effect
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
	private bool allowShake;
	private Quaternion quatTemp;
	
	protected override void ownAwake () {
		allowShake = false;
	}
	
	protected override void ownEffectStarts () {
		Invoke("reset", startDelaySecs);
	}

#if UNITY_EDITOR
	void OnGUI () {
		if (!debug)
			return;
		if (GUI.Button (new Rect (20, 40, 80, 20), "Shake"))
			reset();
	}
#endif
	
	void Update ()
	{
		if (!allowShake)
			return;
		shake();
	}
	
	private void reset ()
	{
		tempDecay = shake_decay;
		tempIntensity = shake_intensity;
		// for GUI Custom elements use localPosition
		origPosition = transform.localPosition;
		origRotation = transform.rotation;
		allowShake = true;
	}
	
	private void shake ()
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
		}
	}
	
	public bool isFinished () {
		return allowShake;
	}
}
