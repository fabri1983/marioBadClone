using UnityEngine;

/// <summary>
/// This class defines common behavior to Move action for enemies and the player.
/// Add as abstract those similar Monobehavior methods you will need to use, call them 
/// from real Monobehavior methods, implement them in subclass.
/// </summary>
public abstract class WalkAbs : MonoBehaviour {
	
	private static Vector2 VEC2_RIGHT = Vector2.right;
	private static Vector2 VEC2_LEFT = -Vector2.right;
	
	private float _velocity;
	
	protected bool walking, lookingRight;
	protected float gain;
	protected Jump jump;
	protected Idle idle;
	protected Crouch crouch;
	protected AnimateTiledConfig walkAC;
	protected ChipmunkBody body;
	
	void Awake () {
		jump = GetComponent<Jump>();
		idle = GetComponent<Idle>();
		crouch = GetComponent<Crouch>();
		body = GetComponent<ChipmunkBody>();
		walkAC = AnimateTiledConfig.getByName(gameObject, EnumAnimateTiledName.Walk, true);
		
		lookingRight = true;
	}
	
	public abstract void reset ();
	public abstract void walk (float velocity);
	protected abstract void stopWalking ();
	
	void OnDisable () {
		stop();
	}
	
	void Update () {
		if (Mathf.Abs(body.velocity.x) < 0.1f)
			walking = false;
		else
			updateWalk();
	}
	
	public void stop () {
		stopWalking(); // implemented in the sub class
		walking = false;
		walkAC.stop();
	}
	
	protected void _walk (float velocity) {
		//NOTE: remember to set the gain property before calling this method from subclasses
		
		// set the correct sprite animation
		if (!walkAC.isWorking() && (jump == null || !jump.IsJumping()) && (crouch == null || !crouch.isCrouching())) {
			walkAC.setupAndPlay();
		}
		
		_velocity = velocity;
		if (!walking)
			this.enabled = true;
		walking = true;
		
		Vector2 v = body.velocity;
		v.x = gain * velocity;
		body.velocity = v;
		
		updateWalk();
	}
	
	private void updateWalk () {
		bool oldLooking = lookingRight;
		float vx = body.velocity.x;
		if (vx > 0f)
			lookingRight = true;
		else if (vx < 0f)
			lookingRight = false;
		
		// if velocity changed direction then fix sprite direction
		if (oldLooking != lookingRight) {
			Vector3 theScale = transform.localScale;
			theScale.x *= -1f;
			transform.localScale = theScale;
		}
		
		vx = Mathf.Abs(body.velocity.x);
		walkAC.animComp.setFPS(walkAC.animFPS * gain * (Mathf.Min(vx,_velocity) / _velocity));
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
