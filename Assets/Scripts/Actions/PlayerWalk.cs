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
		Vector2 v = base.body.velocity;
		v.x = 0f;
		base.body.velocity = v;
		
		base.stop();
	}
	
	public void setGain (float val) {
		base.gain = val;
	}
	
	public override void walk (float velocity) {
		if (!base.enabled)
			return;
		
		// if user is looking upwards then set the correct sprite animation
		if (lookDirections.isLookingUpwards())
			base.walkAC = walkLookingUpAC;
		else
			base.walkAC = walkAC_orig;
		
		base._walk(velocity);
	}
	
	protected override void stopWalking () {
		if (!base.jump.isJumping() && !lookDirections.isLookingAnyDirection())
			base.walkAC = walkAC_orig;
	}

}
