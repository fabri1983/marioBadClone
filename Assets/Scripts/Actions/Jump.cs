using UnityEngine;

/// <summary>
/// This class has a built in gravity simulator. So when player isn't in any pltaform with tag Floor the gravity pulls down.
/// It works with rigidbodies, so is possible some sort of bugs appear.
/// </summary>
public class Jump : AnimateTiledConfig {
	
	private bool isJumping = false;
	private float fallSpeed;
	private float jumpingVel;
	private bool foreverJump = false;
	private float foreverJumpVel = 0f;
	private float foreverFallSpeed = 0f;
	private bool factorApplied = false; // whether a jump factor was or wasn't applied in current jump loop
	
	private Crouch crouch;
	private Idle idle; // when stop jumping we need to set the player to idle behavior
	
	// Use this for initialization
	void Start () {
		idle = GetComponent<Idle>();
		crouch = GetComponent<Crouch>();
		reset();
	}
	
	void Update () {
		// this Update() method for physic's simulation
		
		if (!isJumping)
			return;
		
		// while not touching the ground, reduce the vertical speed a little bit every frame
		jumpingVel -= fallSpeed * Time.deltaTime;
		
		// stop falling speed at certain threshold
		if (jumpingVel < -fallSpeed)
			jumpingVel = -fallSpeed;

		transform.Translate(0f, jumpingVel * Time.deltaTime, 0f);
	}
	
	void OnCollisionEnter (Collision collision) {
		
		if (collision.transform.tag.Equals("Floor")) {
			if (foreverJump) {
				jump(foreverJumpVel, foreverFallSpeed);
				return;
			}
			else {
				isJumping = false;
				factorApplied = false;
				// if it was jumping then set player behavior to idle
				if (idle != null && crouch != null && !crouch.isCrouching())
					idle.setIdle(false);
			}
		}
		// this to avoid enemies that use this jump script to abruptly stop jumping when hitting something special for enemies
		if (!gameObject.tag.Equals("Mario") && (collision.gameObject.layer != LevelManager.FOR_ENEMY_LAYER))
			jumpingVel = 0f;
	}
	
	void OnCollisionStay (Collision collision) {
		
		if (collision.transform.tag.Equals("Floor")) {
			isJumping = false;
			factorApplied = false;
		}
		// this to avoid enemies that use this jump script to abruptly stop jumping when hitting something special for enemies
		if (collision.gameObject.layer != LevelManager.FOR_ENEMY_LAYER)
			jumpingVel = 0f;
	}
	
	void OnCollisionExit (Collision collision) {
		
		// if player is taking off of the floor set jumping to true to simulate gravity pulls down
		if (collision.transform.tag.Equals("Floor") && gameObject.layer != LevelManager.TELEPORT_LAYER)
			isJumping = true;
	}
	
	public void jump (float pJumpVel, float pFallSpeed) {
		
		if (isJumping)
			return;
		
		// set the correct sprite animation
		if (animComponent != null && crouch != null && !crouch.isCrouching()) {
			animComponent.setFPS(animFPS);
			animComponent.setRowLimits(rowStartAnim, rowLengthAnim);
			animComponent.setColLimits(maxColsAnimInRow, colStartAnim, colLengthAnim);
			animComponent.setPingPongAnim(pingPongAnim);
			animComponent.Play();
		}
		
		isJumping = true;
		jumpingVel = pJumpVel;
		fallSpeed = pFallSpeed;
		
		// give an initial jump to take off from floor
		transform.Translate(0f, jumpingVel * Time.deltaTime, 0f);
	}
	
	/// <summary>
	/// Applies a factor to current jump velocity. Only once per jump loop.
	/// </summary>
	/// <param name='factor'>
	/// Factor.
	/// </param>
	public void applyFactor (float factor) {
		if (!factorApplied) {
			jumpingVel *= factor;
			factorApplied = true;
		}
	}
	
	public void reset () {
		isJumping = true; // initialized as true for gravity's simulation 
		factorApplied = false;
		jumpingVel = 0f;
		fallSpeed = 25f;
	}
	
	public bool IsJumping () {
		return isJumping;
	}
	
	public void setForeverJump (bool val) {
		foreverJump = val;
	}
	
	public void setForeverValues (float pJumpSpeed, float pFallSpeed) {
		foreverJumpVel = pJumpSpeed;
		foreverFallSpeed = pFallSpeed;
	}
}
