using UnityEngine;

/**
 * Moves a transform horizontally. So this script manages if transform is moving left or right.
 */
public class PlayerMove : MoveAbs {
	
	public float animFPSBoost = 8;
	public float speedUpFactor = 1.9f;
	
	private bool lookingRight;
	private bool moving;
	private float gain;
	private PlayerDieAnim dieAnim;
	private Jump jump;
	private Idle idle;
	private Crouch crouch;
	private AnimateTiledConfig moveAC;
	private ChipmunkShape shape;
	
	// Use this for initialization
	void Awake () {
		dieAnim = GetComponent<PlayerDieAnim>();
		jump = GetComponent<Jump>();
		idle = GetComponent<Idle>();
		crouch = GetComponent<Crouch>();
		moveAC = GetComponentInChildren<MoveAnimConfig>();
		shape = GetComponent<ChipmunkShape>();
		
		reset();
	}
	
	public void reset () {
		lookingRight = true;
		if (idle != null) idle.setIdle(true);
		moving = false;
	}
	
	public override void move (Vector2 velocity) {
		move (velocity.magnitude);
	}
	
	public override void move (float velocity) {
		
		if (dieAnim.isDying())
			return;
		
		// start the animation
		if (!moving && !jump.IsJumping() && !crouch.isCrouching()) {
			moveAC.animComp.setRowLimits(moveAC.rowStartAnim, moveAC.rowLengthAnim);
			moveAC.animComp.setColLimits(moveAC.maxColsAnimInRow, moveAC.colStartAnim, moveAC.colLengthAnim);
			moveAC.animComp.setPingPongAnim(moveAC.pingPongAnim);
			moveAC.animComp.Play();
		}
		
		moving = true;
		
		if (Gamepad.isB() || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			gain = speedUpFactor;
			if (moveAC.animComp != null)
				moveAC.animComp.setFPS(animFPSBoost);
		}
		else {
			gain = 1f;
			if (moveAC.animComp != null)
				moveAC.animComp.setFPS(moveAC.animFPS);
		}

		// horizontal move
		if (velocity != 0f) {
			bool oldLookingRight = lookingRight;
			// moving right
			if (velocity > 0f)
				lookingRight = true;
			// moving left
			else if (velocity < 0f)
				lookingRight = false;
			
			Vector2 v = shape.body.velocity;
			v.x = gain * velocity;
			shape.body.velocity = v;
			
			// did Mario turn around?
			if (oldLookingRight != lookingRight) {
				Vector3 theScale = transform.localScale;
				theScale.x *= -1f;
				transform.localScale = theScale;
			}
			
			
		}
	}
	
	public override bool isMoving () {
		return moving;
	}
	
	public override void stopMoving () {
		moving = false;
	}
}
