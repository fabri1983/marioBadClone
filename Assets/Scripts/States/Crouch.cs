using UnityEngine;

public class Crouch : MonoBehaviour {
	
	public float crouchColliderProportion = 0.75f;
	
	private bool crouching;
	private float colliderCenterY, centerOffsetY;
	private ChipmunkBoxShape box;
	private Jump jump;
	private Sneak sneak;
	private WalkAbs move;
	private AnimateTiledConfig crouchAC;
	
	void Awake () {
		crouching = false;
		
		// take the collider and some useful values
		box = GetComponent<ChipmunkBoxShape>();
		colliderCenterY = box.center.y;
		centerOffsetY = ((1f - crouchColliderProportion)*0.5f) * box.size.y;
		
		jump = GetComponent<Jump>();
		sneak = GetComponent<Sneak>();
		move = GetComponent<WalkAbs>();
		crouchAC = GetComponentInChildren<CrouchAnimConfig>();
	}
	
	// Update is called once per frame
	public void crouch () {
		// is it jumping?
		bool jumping = false;
		if (jump != null && jump.IsJumping())
			jumping = true;
		// is it moving?
		bool moving = false;
		if (move != null && move.isWalking())
			moving = true;
		
		// if crouching then update accordingly
		if (sneak != null && crouching) {
			// if jumping and sneaking: stop sneaking and do crouch
			if (jumping) {
				if (sneak.isSneaking()) {
					sneak.stopSneaking();
					startAnim();
				}
			}
			// if not jumping and not moving and sneaking: stop sneaking and do crouch
			else if (!moving) {
				if (sneak.isSneaking()) {
					sneak.stopSneaking();
					startAnim();
				}
			}
			// if not jumping and moving: sneak
			else
				sneak.sneak();
		}
		
		// don't update
		if (crouching || jumping)
			return;
		
		crouching = true;
		
		// resize the collider
		Vector3 theSize = box.size;
		theSize.y *= crouchColliderProportion;
		box.size = theSize;
		// transform the collider
		Vector3 theCenter = box.center;
		theCenter.y -= centerOffsetY;
		box.center = theCenter;
		
		// set the correct sprite animation
		startAnim();
	}
	
	private void startAnim () {
		crouchAC.animComp.setFPS(crouchAC.animFPS);
		crouchAC.animComp.setRowLimits(crouchAC.rowStartAnim, crouchAC.rowLengthAnim);
		crouchAC.animComp.setColLimits(crouchAC.maxColsAnimInRow, crouchAC.colStartAnim, crouchAC.colLengthAnim);
		crouchAC.animComp.setPingPongAnim(crouchAC.pingPongAnim);
		crouchAC.animComp.Play();
		crouchAC.working = true;
	}
	
	public void noCrouch () {
		
		if (sneak != null)
			sneak.stopSneaking();
		
		if (!crouching)
			return;
		
		crouching = false;
		crouchAC.working = false;
		
		// transform the collider
		Vector3 theCenter = box.center;
		theCenter.y = colliderCenterY;
		box.center = theCenter;
		// resize the collider
		Vector3 theSize = box.size;
		theSize.y /= crouchColliderProportion;
		box.size = theSize;
	}
	
	public bool isCrouching () {
		return crouching;
	}
}
