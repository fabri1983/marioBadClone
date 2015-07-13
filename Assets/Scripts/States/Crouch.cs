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
		crouchAC = AnimateTiledConfig.getByName(gameObject, EnumAnimateTiledName.Crouch, true);
	}
	
	// Update is called once per frame
	public void crouch () {
		// is it jumping?
		bool jumping = false;
		if (jump != null && jump.isJumping())
			jumping = true;
		// is it moving?
		bool moving = false;
		if (move != null && move.isWalking())
			moving = true;
		
		// if crouching then update accordingly
		if (sneak != null && crouching) {
			// while in the air we can't sneak
			if (jumping) {
				if (sneak.isSneaking()) {
					sneak.stopSneaking();
					crouchAC.setupAndPlay();
				}
			}
			// if not jumping and not moving and sneaking: stop sneaking and do crouch
			else if (!moving) {
				if (sneak.isSneaking()) {
					sneak.stopSneaking();
					crouchAC.setupAndPlay();
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
		crouchAC.setupAndPlay();
	}
	
	public void noCrouch () {
		
		if (sneak != null)
			sneak.stopSneaking();
		
		if (!crouching)
			return;
		
		move.stop(); // this force the reset of the sprite animation
		crouchAC.stop();
		crouching = false;
		
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
