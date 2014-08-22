using UnityEngine;

public class Bounce : MonoBehaviour {
	
	public float kickedSpeed = 20f;
	
	private Patrol patrol;
	private bool bouncing;
	
	// Use this for initialization
	void Awake () {
		patrol = GetComponent<Patrol>();
		bouncing = false;
	}
	
	public void stop () {
		bouncing = false;
		patrol.stopPatrol();
	}
	
	public void bounce (float dir) {
		bouncing = true;
		// tell patrol to start moving like crazy
		patrol.enablePatrol();
		patrol.setMoveSpeed(kickedSpeed);
		patrol.setNewDir(dir);
	}
	
	public bool isBouncing () {
		return bouncing;
	}
}
