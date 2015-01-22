using UnityEngine;

public class Chase : MonoBehaviour {
	
	public float speed = 5f;
	
	private float signDir;
	private WalkAbs walk;
	private Patrol patrol;
	private Idle idle;
	private Transform target;
	private bool stop, operable, enableWhenOutOfSensor;
	private ChipmunkBody body;
	
	// Use this for initialization
	void Awake () {
		walk = GetComponent<WalkAbs>();
		patrol = GetComponent<Patrol>();
		idle = GetComponent<Idle>();
		body = GetComponent<ChipmunkBody>();
		stop = true;
		operable = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (stop)
			return;
		// calculate sign direction
		signDir = Mathf.Sign(target.position.x - transform.position.x);
		if (patrol != null)
			patrol.setNewDir(signDir);
		else
			walk.walk(signDir * speed);
	}
	
	public void stopChasing () {
		if (!operable)
			return;
		stop = true;
		if (patrol == null) {
			body.velocity = Vector2.zero;
			walk.stopWalking();
			if (idle != null)
				idle.setIdle(true);
		}
	}
	
	public void enableChasing () {		
		if (!operable)
			return;
		stop = false;
		if (patrol == null)
			walk.enableWalking();
	}
	
	public bool isChasing () {
		return !stop;
	}
	
	public Transform getTarget () {
		return target;
	}
	
	public void setOperable (bool val) {
		operable = val;
	}
	
	public bool isOperable () {
		return operable;
	}
	
	public void enableOperateWhenOutOfSensor () {
		if (!operable)
			return;
		// this behavior is esential for avoiding wall penetration
		operable = false;
		enableWhenOutOfSensor = true;
	}
	
	private void enableOperate () {
		if (enableWhenOutOfSensor) {
			enableWhenOutOfSensor = false;
			operable = true;
		}
	}
	
	public static bool beginCollisionWithScenery (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		// if chasing then avoid wall penetration
		Chase chase = shape1.GetComponent<Chase>();
		if (chase != null && chase.isChasing() && GameObjectTools.isWallHit(arbiter)) {
			chase.stopChasing();
			chase.enableOperateWhenOutOfSensor();
		}

		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next frame.
		return true;
	}
	
	public static bool beginCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		Chase chase = shape1.GetComponent<Chase>();
		Player player = shape2.GetComponent<Player>();
		
		if (player.isDying() || !chase.isOperable())
			return false; // stop collision since this frame
		
		// start chasing player
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
		chase.enableOperate();
	}
}
