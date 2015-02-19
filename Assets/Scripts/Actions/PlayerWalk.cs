using UnityEngine;

public class PlayerWalk : WalkAbs {
	
	public float speedUpFactor = 1.9f;
	
	private AnimateTiledConfig walkAC_orig;
	private AnimateTiledConfig walkLookingUpAC;
	private LookUpwards lookUpwards;
	
	void Start () {
		lookUpwards = GetComponent<LookUpwards>();
		walkAC_orig = walkAC;
		walkLookingUpAC = AnimateTiledConfig.getByName(gameObject, EnumAnimateTiledName.WalkLookUpwards, true);
	}
	
	public override void reset () {
		//if (idle != null) idle.setIdle(true);
		walking = false;
		stop = false;
	}
	
	public override void walk (float velocity) {
		if (stop)
			return;
		
		gain = 1f; // default value
		float velX = base.shape.body.velocity.x;
		
		// when jumping and trying to move in opposite direction, just lessen current velocity
		if (jump.IsJumping() && (velX * velocity) < 0f)
			velocity = velX * 0.75f;
		// is speed up button being pressed?
		else if (Gamepad.isB() || (Input.GetButton("Fire1") && Input.touchCount == 0))
			gain = speedUpFactor;
		
		// if user is looking upwards then set the correct sprite animation
		if (lookUpwards.isLookingUpwards())
			walkAC = walkLookingUpAC;
		else
			walkAC = walkAC_orig;
		
		base._walk(velocity);
		walkAC.animComp.setFPS(walkAC.animFPS * gain);
	}

	public override void stopWalking () {
		base._stopWalking();
		
		if (!base.jump.IsJumping() && lookUpwards.isLookingUpwards())
			base.walkAC = walkAC_orig;
	}
}
