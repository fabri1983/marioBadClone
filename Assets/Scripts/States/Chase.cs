using UnityEngine;

public class Chase : MonoBehaviour {
	
	public float speed = 5f;
	
	private float signDir;
	private WalkAbs walk;
	private Patrol patrol;
	private Idle idle;
	private Transform target;
	private bool operable, enableWhenOutOfSensor;
	
	// Use this for initialization
	void Awake () {
		walk = GetComponent<WalkAbs>();
		patrol = GetComponent<Patrol>();
		idle = GetComponent<Idle>();
		operable = true;
		this.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (target == null)
			return;
		
		// calculate sign direction
		signDir = Mathf.Sign(target.position.x - transform.position.x);
		if (patrol != null)
			patrol.setDir(signDir);
		else
			walk.walk(signDir * speed);
	}
	
	public void stop () {
		this.enabled = false;
		if (patrol == null) {
			walk.enabled = false;
			if (idle != null)
				idle.setIdle(true);
		}
	}
	
	public void enable () {	
		this.enabled = true;
		if (patrol == null)
			walk.enabled = true;
	}
	
	public bool isChasing () {
		return this.enabled;
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
	
	public static bool beginCollisionWithAny (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		// if chasing then avoid wall penetration
		Chase chase = shape1.GetComponent<Chase>();
		if (chase != null && chase.isChasing() && GameObjectTools.isWallHit(arbiter)) {
			chase.stop();
			chase.enableOperateWhenOutOfSensor();
		}

		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next fixed step.
		return true;
	}
	
	public static bool beginCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		Chase chase = shape1.getOwnComponent<Chase>();
		Player player = shape2.getOwnComponent<Player>();
		
		if (player.isDying() || !chase.isOperable())
			return false; // stop collision since this frame
		
		// start chasing player
		chase.target = player.transform;
		chase.enable();
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next fixed step.
		return false;
	}
	
	public static void endCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		Chase chase = shape1.getOwnComponent<Chase>();
		chase.stop();
		chase.target = null;
		chase.enableOperate();
	}
}
