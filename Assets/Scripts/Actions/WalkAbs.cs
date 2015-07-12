using UnityEngine;

/// <summary>
/// This class defines common behavior to Move action for enemies and the player.
/// Add as abstract those similar Monobehavior methods you will need to use, call them 
/// from real Monobehavior methods, implement them in subclass.
/// </summary>
public abstract class WalkAbs : MonoBehaviour {
	
	private static Vector2 VEC2_RIGHT = Vector2.right;
	private static Vector2 VEC2_LEFT = -Vector2.right;
	
	protected float velocity = 0f;
	
	protected bool walking, lookingRight;
	protected float gain = 1f;
	protected Jump jump;
	protected Idle idle;
	protected Crouch crouch;
	protected AnimateTiledConfig walkAC;
	protected ChipmunkBody body;
	protected AirGroundControlUpdater agUpdater;
	
	void Awake () {
		jump = GetComponent<Jump>();
		idle = GetComponent<Idle>();
		crouch = GetComponent<Crouch>();
		body = GetComponent<ChipmunkBody>();
		walkAC = AnimateTiledConfig.getByName(gameObject, EnumAnimateTiledName.Walk, true);
		agUpdater = GetComponent<AirGroundControlUpdater>();
		
		lookingRight = true;
	}
	
	public abstract void reset ();
	public abstract void walk (float velocity);
	protected abstract void stopWalking ();
	
	void OnDisable () {
		stop();
	}
	
	public void stop () {
		stopWalking(); // implemented in the sub class
		walking = false;
		velocity = 0f;
		updateWalk();
		walkAC.stop();
	}
	
	void Update () {
		if (agUpdater != null && Mathf.Abs(agUpdater.groundVelocity.x) < 0.8f)
			stop();
	}
	
	protected void _walk (float vel) {
		//NOTE: remember to set the gain property before calling this method from subclasses
	
		// set the correct sprite animation
		if (!walkAC.isWorking() && (jump == null || !jump.isJumping()) && (crouch == null || !crouch.isCrouching())) {
			walkAC.setupAndPlay();
		}
		
		velocity = vel;
		if (!walking)
			this.enabled = true;
		walking = true;
		
		if (agUpdater != null) {
			agUpdater.setWalkSpeed(gain * velocity);
		} else {
			Vector2 v = body.velocity;
			v.x = gain * vel;
			body.velocity = v;
		}
		
		/*if (jump != null) {
			if (!jump.isJumping())
				updateWalk();
		} else */{
			updateWalk();
		}
	}
	
	private void updateWalk () {
		bool oldLooking = lookingRight;
		if (velocity > 0f)
			lookingRight = true;
		else if (velocity < 0f)
			lookingRight = false;
		
		// if velocity changed direction then fix sprite direction
		if (oldLooking != lookingRight) {
			Vector3 theScale = transform.localScale;
			theScale.x *= -1f;
			transform.localScale = theScale;
		}
		
		float vx = Mathf.Abs(body.velocity.x);
		float velFactor = Mathf.Min(vx, velocity) / velocity;
		walkAC.animComp.setFPS(walkAC.animFPS * gain * velFactor);
	}
	
	public bool isLookingRight () {
		return lookingRight;
	}
	
	public Vector2 getLookingDir () {
		if (lookingRight)
			return VEC2_RIGHT;
		return VEC2_LEFT;
	}
	
	public bool isWalking () {
		return walking;
	}
}
