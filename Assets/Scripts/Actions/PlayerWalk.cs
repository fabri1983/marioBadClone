using UnityEngine;

public class PlayerWalk : WalkAbs {
	
	public float speedUpFactor = 1.9f;
	
	private AnimateTiledConfig walkAC_orig;
	private AnimateTiledConfig walkLookingUpAC;
	private LookDirections lookDirections;
	
	void Start () {
		lookDirections = GetComponent<LookDirections>();
		walkAC_orig = base.walkAC;
		walkLookingUpAC = AnimateTiledConfig.getByName(gameObject, EnumAnimateTiledName.WalkLookUpwards, true);
	}
	
	public override void reset () {
		//if (idle != null) idle.setIdle(true);
		base.walking = false;
		base.stop = false;
	}
	
	public override void walk (float velocity) {
		if (stop)
			return;
		
		base.gain = 1f; // default value
		float velX = base.shape.body.velocity.x;
		
		// when jumping and trying to move in opposite direction, just lessen current velocity
		if (base.jump.IsJumping() && (velX * velocity) < 0f)
			velocity = velX * 0.75f;
		// is speed up button being pressed?
		else if (Gamepad.isB() || (Input.GetButton("Fire1") && Input.touchCount == 0))
			base.gain = speedUpFactor;
		
		// if user is looking upwards then set the correct sprite animation
		if (lookDirections.isLookingUpwards())
			base.walkAC = walkLookingUpAC;
		else
			base.walkAC = walkAC_orig;
		
		base._walk(velocity);
		base.walkAC.animComp.setFPS(base.walkAC.animFPS * base.gain);
	}
	
	public override void stopWalking () {
		base._stopWalking();
		
		if (!base.jump.IsJumping() && lookDirections.isLookingAnyDirection())
			base.walkAC = walkAC_orig;
	}
}
