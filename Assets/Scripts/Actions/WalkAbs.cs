using UnityEngine;

/// <summary>
/// This class defines common behavior to Move action for enemies and the player.
/// Add as abstract those similar Monobehavior methods you will need to use, call them 
/// from real Monobehavior methods, implement them in subclass.
/// </summary>
public abstract class WalkAbs : MonoBehaviour {
	
	protected bool stop, walking, lookingRight;
	protected float gain;
	protected Jump jump;
	protected Idle idle;
	protected Crouch crouch;
	protected AnimateTiledConfig walkAC;
	protected ChipmunkShape shape;
	
	void Awake () {
		jump = GetComponent<Jump>();
		idle = GetComponent<Idle>();
		crouch = GetComponent<Crouch>();
		shape = GetComponent<ChipmunkShape>();
		walkAC = GetComponentInChildren<WalkAnimConfig>();
		
		lookingRight = true;
		reset();
	}
	
	public abstract void reset ();
	public abstract void walk (float velocity);
	
	protected void _walk (float velocity) {
		//NOTE: remember to set the gain property before calling this method from subclasses
		
		// set the correct sprite animation
		if (!walking && (jump == null || !jump.IsJumping()) && (crouch == null || !crouch.isCrouching())) {
			walkAC.animComp.setFPS(walkAC.animFPS);
			walkAC.animComp.setRowLimits(walkAC.rowStartAnim, walkAC.rowLengthAnim);
			walkAC.animComp.setColLimits(walkAC.maxColsAnimInRow, walkAC.colStartAnim, walkAC.colLengthAnim);
			walkAC.animComp.setPingPongAnim(walkAC.pingPongAnim);
			walkAC.animComp.Play();
		}
		
		walking = true;
		
		// horizontal move
		bool oldLooking = lookingRight;
		// moving right
		if (velocity > 0f)
			lookingRight = true;
		// moving left
		else if (velocity < 0f)
			lookingRight = false;
		
		Vector2 v = shape.body.velocity;
		v.x = gain * velocity;
		shape.body.velocity = v;
		
		// did game object turn around?
		if (oldLooking != lookingRight) {
			Vector3 theScale = transform.localScale;
			theScale.x *= -1f;
			transform.localScale = theScale;
		}
	}
	
	public bool isLookingRight () {
		return lookingRight;
	}
	
	public bool isWalking () {
		return walking;
	}
	
	public void stopWalking () {
		stop = true;
		walking = false;
	}
	
	public void enableWalking () {
		stop = false;
	}
}
