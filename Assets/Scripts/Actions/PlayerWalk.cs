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
	
	void Update () {
		if (Mathf.Abs(base.body.velocity.x) < 0.1f)
			base.walking = false;
		else
			base.updateWalk();
	}
	
	public override void reset () {
		Vector2 v = base.body.velocity;
		v.x = 0f;
		base.body.velocity = v;
		base.stop();
	}
	
	public override void walk (float velocity) {
		if (!base.enabled)
			return;
		
		base.gain = 1f; // default value
		// is speed up button being pressed?
		if (Gamepad.isB())
			base.gain = speedUpFactor;
		
		// if user is looking upwards then set the correct sprite animation
		if (lookDirections.isLookingUpwards())
			base.walkAC = walkLookingUpAC;
		else
			base.walkAC = walkAC_orig;
		
		base._walk(velocity);
	}
	
	protected override void stopWalking () {
		if (!base.jump.IsJumping() && !lookDirections.isLookingAnyDirection())
			base.walkAC = walkAC_orig;
	}
}
