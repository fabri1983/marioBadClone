using UnityEngine;

/// <summary>
/// This class has a built in gravity simulator. So when player isn't in any pltaform with tag Floor the gravity pulls down.
/// It works with rigidbodies, so is possible some sort of bugs appear.
/// </summary>
public class Jump : MonoBehaviour {
	
	private bool foreverJump = false;
	private bool _isJumping = false;
	private float foreverJumpVel = 0f;
	private bool gainApplied = false; // whether a jump gain was or wasn't applied in current jump loop
	private Crouch crouch;
	private AnimateTiledConfig jumpAC;
	private ChipmunkBody body;
	
	void Awake () {
		crouch = GetComponent<Crouch>();
		jumpAC = AnimateTiledConfig.getByName(gameObject, EnumAnimateTiledName.Jump, true);
		body = GetComponent<ChipmunkBody>();
		resetStatus();
	}

	public void resetStatus () {
		_isJumping = true; // initialized as true in case the object is spawned in the air
		gainApplied = true;
	}
	
	void LateUpdate () {
		////////////////////////////////////
		//IMPORTANT: this fixes the crash when assigning the modified velocity to the body along this script
		/*if (_isJumping)
			return;
		Vector2 v = body.velocity;
		body.velocity = v;*/
		////////////////////////////////////
	}

	public void stop () {
		gainApplied = false;
		_isJumping = false;
	}

	public void jump (float jumpVel) {
		if (_isJumping)
			return;
		forceJump(jumpVel);
	}
	
	public void forceJump (float jumpVel) {
		// set the correct sprite animation
		if (crouch == null || !crouch.isCrouching())
			jumpAC.setupAndPlay();
		
		_isJumping = true;

		Vector2 v = body.velocity;
		v.y = jumpVel;
		body.velocity = v;
	}
		
	/// <summary>
	/// Applies a gain factor to current jump velocity. Only once per jump cycle.
	/// </summary>
	/// <param name='factor'>
	/// Factor.
	/// </param>
	public void applyGain (float factor) {
		if (!gainApplied) {
			forceJump(factor * body.velocity.y);
			gainApplied = true;
		}
	}
	
	public bool isJumping () {
		return _isJumping;
	}
	
	public void setForeverJump (bool val) {
		foreverJump = val;
	}
	
	public void setForeverJumpSpeed (float pJumpSpeed) {
		foreverJumpVel = pJumpSpeed;
	}
	
	public static bool beginCollisionWithAny (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		Jump jump = shape1.GetComponent<Jump>();
		// if no Jump component then continue the collision
		if (jump == null)
			return true;

		// if is jumping and hits a wall then continue the collision
		/*if (jump.isJumping && GameObjectTools.isWallHit(arbiter))
			return true;*/
		
		// check if hit its head against something
		if (jump.enabled && GameObjectTools.isCeiling(arbiter)) {
			;
		}
		else if (jump.enabled && GameObjectTools.isGrounded(arbiter)) {
			if (jump.foreverJump)
				jump.forceJump(jump.foreverJumpVel);
			// if it was jumping then reset jump behavior
			else if (jump._isJumping)
				jump.stop();
		}
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next fixed step.
		return true;
	}

	public static void endCollisionWithScenery (ChipmunkArbiter arbiter) {
		/*ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);*/
	}
}
