using UnityEngine;

public class Chase : MonoBehaviour {
	
	public float speed = 5f;
	
	private WalkAbs walk;
	private Idle idle;
	private Transform target;
	private bool stop;
	private ChipmunkBody body;
	
	// Use this for initialization
	void Awake () {
		walk = GetComponent<WalkAbs>();
		idle = GetComponent<Idle>();
		body = GetComponent<ChipmunkBody>();
	}
	
	void Start () {
		stopChasing();
	}
	
	// Update is called once per frame
	void Update () {
		if (stop)
			return;

		// calculate vector direction
		float sign = Mathf.Sign(target.position.x - transform.position.x);
		walk.walk(sign * speed);
	}
	
	public void stopChasing () {
		stop = true;
		walk.stopWalking();
		idle.setIdle(true);
		body.velocity = Vector2.zero;
	}
	
	public void enableChasing () {
		stop = false;
		walk.enableWalking();
	}
	
	public bool isChasing () {
		return !stop;
	}
	
	public Transform getTarget () {
		return target;
	}
	
	public static bool beginCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		Player player = shape2.GetComponent<Player>();
		if (player.isDying())
			return false; // stop collision since this frame

		Chase chase = shape1.GetComponent<Chase>();
		chase.target = player.transform;
		chase.enableChasing();
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next frame.
		return false;
	}
	
	public static void endCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		Chase chase = shape1.GetComponent<Chase>();
		chase.stopChasing();
		chase.target = null;
	}
}
