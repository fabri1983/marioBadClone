using UnityEngine;

public class Fly : MonoBehaviour {
	
	public float flyRange = 6f;
	public float flySpeed = 6f;
	
	private float fallSpeed;
	private float flyVel;
	private Transform target; // the tranform which will receive the translate
	private bool automaticFly = false;
	private float flyAcum = 0f; // counts the steps for reaching flying interval
	private bool goDown = true;
	
	// Update is called once per frame
	void Update () {
		
		// only fly if has target which apply the effect to
		if (target == null)
			return;
		
		// when automatic fly is on, then the movement is dictated by a lineal function
		if (automaticFly) {
			flyAcum += flyRange * Time.deltaTime;
			flyVel = (goDown ? -1f : 1f) * flySpeed;
			if (flyAcum >= flyRange) {
				goDown = !goDown;
				flyAcum = 0f;
			}
		}
		// reduce the vertical speed a little bit every frame
		else {
			flyVel -= fallSpeed * Time.deltaTime;
			if (flyVel < -fallSpeed)
				flyVel = -fallSpeed;	
		}
		
		target.Translate(0f, flyVel * Time.deltaTime, 0f);
	}
	
	public void fly (Transform pTarget, float pFlyVel, float pFallSpeed) {
		
		target = pTarget;
		flyVel = pFlyVel;
		fallSpeed = pFallSpeed;
	}
	
	public void setAutomaticFly (bool val) {
		automaticFly = val;
	}
}
