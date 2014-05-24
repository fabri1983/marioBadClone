using UnityEngine;

public class Crouch : AnimateTiledConfig {
	
	public float crouchProportion = 0.5f;
	
	private bool crouching;
	private float colliderHeightHalf, colliderCenterY, crouchHeight;
	private BoxCollider box;
	private Jump jump;
	private Sneak sneak;
	private MoveAbs move;
	
	void Awake () {
		crouching = false;
		
		// take the collider and some useful values
		box = (BoxCollider)collider;
		colliderHeightHalf = box.size.y * 0.5f;
		colliderCenterY = box.center.y;
		crouchHeight = -0.5f * (colliderHeightHalf - (1f - crouchProportion)*0.5f);
		
		jump = GetComponent<Jump>();
		sneak = GetComponent<Sneak>();
		move = GetComponent<MoveAbs>();
	}
	
	// Update is called once per frame
	public void crouch () {
		
		bool jumping = false;
		if (jump != null && jump.IsJumping())
			jumping = true;
		
		bool moving = false;
		if (move != null && move.isMoving())
			moving = true;
		
		if (sneak != null && crouching) {
			if (jumping) {
				if (sneak.isSneaking()) {
					sneak.stopSneaking();
					startAnim();
				}
			}
			else if (!moving) {
				if (sneak.isSneaking()) {
					sneak.stopSneaking();
					startAnim();
				}
			}
			else
				sneak.sneak();
		}
		
		// don't update
		if (crouching || jumping)
			return;
		
		crouching = true;
		
		// resize the collider
		Vector3 theSize = box.size;
		theSize.y *= crouchProportion;
		box.size = theSize;
		// transform the collider
		Vector3 theCenter = box.center;
		theCenter.y = crouchHeight;
		box.center = theCenter;
		
		// set the correct sprite animation
		startAnim();
	}
	
	private void startAnim () {
		if (animComponent != null) {
			animComponent.setFPS(animFPS);
			animComponent.setRowLimits(rowStartAnim, rowLengthAnim);
			animComponent.setColLimits(maxColsAnimInRow, colStartAnim, colLengthAnim);
			animComponent.setPingPongAnim(pingPongAnim);
			animComponent.Play();
		}
	}
	
	public void noCrouch () {
		
		if (sneak != null)
			sneak.stopSneaking();
		
		if (!crouching)
			return;
		
		crouching = false;
		
		// transform the collider
		Vector3 theCenter = box.center;
		theCenter.y = colliderCenterY;
		box.center = theCenter;
		// resize the collider
		Vector3 theSize = box.size;
		theSize.y /= crouchProportion;
		box.size = theSize;
	}
	
	public bool isCrouching () {
		return crouching;
	}
}
