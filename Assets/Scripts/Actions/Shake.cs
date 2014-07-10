using UnityEngine;
using System.Collections;

public class Shake : MonoBehaviour
{
	// controls is the amount that shake_intensity is decremented each update. It determines if the shake is long or short
	public float shake_decay = 0.5f;
	// determines the initial intensity of the shaking — how much variance to allow in the object position
	public float shake_intensity = 4f;
	// true if you want also shake rotation of game object
	public bool allowRotation = false;
	public float startDelaySecs = 0;
#if UNITY_EDITOR
	// set to true if you want to test in gameplay mode
	public bool debug = false;
#endif
	
	private float tempDecay, tempIntensity;
	private Vector3 originPosition;
	private Quaternion originRotation;
	private bool allowShake = false;
	
	void Awake () {
		if (startDelaySecs > 0f)
			Invoke("reset", startDelaySecs);
	}
	
#if UNITY_EDITOR
	void OnGUI ()
	{
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
	
	public void reset ()
	{
		allowShake = true;
		tempDecay = shake_decay;
		tempIntensity = shake_intensity;
		originPosition = transform.position;
		originRotation = transform.rotation;
	}
	
	public void shake ()
	{
		if (tempIntensity > 0) {
			transform.position = originPosition + Random.insideUnitSphere * tempIntensity;
			if (allowRotation) {
				transform.rotation = new Quaternion (
					originRotation.x + Random.Range (-tempIntensity, tempIntensity) * .2f,
					originRotation.y + Random.Range (-tempIntensity, tempIntensity) * .2f,
					originRotation.z + Random.Range (-tempIntensity, tempIntensity) * .2f,
					originRotation.w + Random.Range (-tempIntensity, tempIntensity) * .2f);
			}
			tempIntensity -= tempDecay;
		}
		// once the effect finishes reset some behavior
		else {
			transform.position = originPosition;
			transform.rotation = originRotation;
			allowShake = false;
		}
	}
	
	public bool isFinished () {
		return allowShake;
	}
}
