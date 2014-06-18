using UnityEngine;

public class Patrol : MonoBehaviour {
	
	public float speed = 6f;
	
	private WalkAbs walk;
	private float dir; // only for direction, must be normalized
	private bool stop;
	
	// Use this for initialization
	void Awake () {
		dir = 1f; // initial normalized direction 
		walk = GetComponent<WalkAbs>();
		stop = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (stop)
			return;
		// always in movement. The only opportunity to stop is when chase action takes place
		walk.walk(dir * speed);
	}
	
	/**
	 * Set direction only (expected to be normalized)
	 */
	public void setNewDir (float pDir) {
		dir = pDir;
	}
	
	public void toogleDir () {
		dir *= -1f;
	}
	
	public void setMovePower (float val) {
		speed = val;
	}
	
	public void stopPatrol () {
		stop = true;
		walk.stopWalking();
	}
	
	public void enablePatrol () {
		stop = false;
		walk.enableWalking();
	}
	
	public static bool beginCollision (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		// change direction of movement whenever hit something like a wall
		if (GameObjectTools.isWallHit(arbiter)) {
			Patrol p1 = shape1.GetComponent<Patrol>();
			if (p1 != null) p1.toogleDir();
			Patrol p2 = shape2.GetComponent<Patrol>();
			if (p2 != null) p2.toogleDir();
		}
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next frame.
		return true;
	}
}
