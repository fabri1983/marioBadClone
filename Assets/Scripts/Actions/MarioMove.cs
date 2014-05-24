using UnityEngine;

/**
 * Moves a transform horizontally. So this script manages if transform is moving left or right.
 */
public class MarioMove : MoveAbs {
	
	public float animFPSBoost = 8;
	public float speedUpFactor = 1.9f;
	
	private bool lookingRight;
	private bool moving;
	private float gain;
	private MarioDieAnim dieAnim;
	private Jump jump;
	private Idle idle;
	private Crouch crouch;
	
	private static Vector3 VECTOR3_RIGHT = Vector3.right;
	private static Vector3 VECTOR3_LEFT = Vector3.left;
	
	// Use this for initialization
	void Awake () {
		dieAnim = GetComponent<MarioDieAnim>();
		jump = GetComponent<Jump>();
		idle = GetComponent<Idle>();
		crouch = GetComponent<Crouch>();
		
		reset();
	}
	
	public void reset () {
		lookingRight = true;
		if (idle != null) idle.setIdle(true);
		moving = false;
	}
	
	public override void move (Vector3 dirAndPow) {
		move (dirAndPow.magnitude);
	}
	
	public override void move (float amount) {
		
		if (dieAnim.isDying())
			return;
		
		// if player wasn't moving then start the animation
		if (!moving) {
			// set the correct sprite animation
			if (animComponent != null && jump != null && !jump.IsJumping()) {
				// if no crouching then start move anim
				if (crouch != null && !crouch.isCrouching()) {
					animComponent.setRowLimits(rowStartAnim, rowLengthAnim);
					animComponent.setColLimits(maxColsAnimInRow, colStartAnim, colLengthAnim);
					animComponent.setPingPongAnim(pingPongAnim);
					animComponent.Play();
				}
			}
		}
		
		moving = true;
		
		if (Gamepad.isB() || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			gain = speedUpFactor;
			if (animComponent != null)
				animComponent.setFPS(animFPSBoost);
		}
		else {
			gain = 1f;
			if (animComponent != null)
				animComponent.setFPS(animFPS);
		}

		// horizontal move
		if (amount != 0f) {
			bool oldLookingRight = lookingRight;
			// moving right
			if (amount > 0f) {
				lookingRight = !lookingRight? true : lookingRight;
				transform.Translate(VECTOR3_RIGHT * amount * gain* Time.deltaTime, Space.World);
			}
			// moving left
			else if (amount < 0f) {
				lookingRight = lookingRight? false : lookingRight;
				transform.Translate(VECTOR3_LEFT * amount * gain * Time.deltaTime);
			}
			// did Mario turn around?
			if (oldLookingRight != lookingRight) {
				transform.Rotate(transform.up, 180f, Space.Self);
				Vector3 theScale = transform.localScale;
				theScale.z *= -1;
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
