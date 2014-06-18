using UnityEngine;

public class Fly : MonoBehaviour {
	
	private float flySpeed, flyRange;
	private bool automaticFly; // keep flying movement?
	private float flyAcum = 0f; // counts the steps for reaching flying interval
	private bool goDown;
	private bool stop;
	private ChipmunkBody body;
	
	void Awake () {
		body = GetComponent<ChipmunkBody>();
		automaticFly = false;
		stop = false;
		goDown = true; // starting going down
	}
	
	// Update is called once per frame
	void Update () {
		if (stop)
			return;
		
		// when automatic fly is on, then the movement is dictated by a linear function
		if (automaticFly) {
			flyAcum += flyRange * Time.deltaTime;
			if (flyAcum >= flyRange) {
				goDown = !goDown;
				flyAcum = 0f;
			}
			fly();
		}
	}
	
	public void fly () {
		stop = false;
		Vector2 v = body.velocity;
		if (goDown) v.y = -flySpeed;
		else v.y = flySpeed;
		body.velocity = v;
	}
	
	public void setSpeed (float _flySpeed) {
		flySpeed = _flySpeed;
	}
	
	public void setAutomaticFly (bool val, float _flyRange) {
		flyRange = _flyRange;
		automaticFly = val;
	}
	
	public void stopFlying () {
		stop = true;
	}
}
